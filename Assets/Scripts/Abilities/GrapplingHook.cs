using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(LineRenderer))]

[DefaultExecutionOrder(1000)]
public class GrapplingHook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam;
    [SerializeField] private Transform ropeStart;      // z.B. Waffenmündung; sonst Kamera
    [SerializeField] private LineRenderer line;
    [SerializeField]private FPSInput fps;

    [Header("Input")]
    [SerializeField] private KeyCode fireToggleKey = KeyCode.F;
    [SerializeField] private KeyCode reelInKey = KeyCode.P;

    [Header("Grapple Settings")]
    [SerializeField] private LayerMask grappleMask = ~0;
    [SerializeField] private float maxRayDistance = 30f;
    [SerializeField] private float minRopeLength = 1.25f;   // unter diese Länge reelst du nicht
    [SerializeField] private float ropeLengthBuffer = 0.05f;   // unter diese Länge reelst du nicht
    [SerializeField] private float reelInSpeed = 12f;     // m/s Seilverkürzung
    [SerializeField] private float tautEpsilon = 0.02f;   // Toleranz, wann “straff”

    [Header("Pull Dynamics (CharacterController)")]
    [SerializeField] private float pullSpring = 45f;     // Federstärke (nur entlang Seil)
    [SerializeField] private float pullDamping = 8f;      // Dämpfung (nur entlang Seil)
    [SerializeField] private float maxPullSpeed = 20f;     // cap der Zuggeschwindigkeit

    [Header("Target Rigidbody Pull (optional)")]
    [SerializeField] private bool pullRigidbodies = true;
    [SerializeField] private float targetSpring = 30f;     // Feder fürs Ziel (nur entlang Seil)
    [SerializeField] private float targetDamper = 6f;      // Dämpfung fürs Ziel
    [SerializeField] private float maxTargetAccel = 40f;     // m/s^2 cap
    [SerializeField] private float maxTargetSpeed = 12f;     // m/s cap (nur entlang Seil)

    [Header("Rope Visuals")]
    [SerializeField] private int ropeSegments = 24;
    [SerializeField] private float sagSmooth = 16f;     // visuelle Glättung
    [SerializeField] private float ropeWidth = 0.05f;

    private CharacterController controller;

    // Rope state
    private bool isGrappling;
    private float ropeLength;                 // feste Länge; ändert sich nur beim Reel-In
    private Vector3 anchorWorld;               // falls kein RB
    private Rigidbody targetRb;                // RB wenn vorhanden
    private Vector3 targetLocalAnchor;        // lokaler Anker im Ziel-RB

    // Player pull (nur Grapple-Anteil)
    private Vector3 pullVelocity;              // NUR entlang/nah der Seilrichtung verändern

    // rendering
    private Vector3[] ropePts;
    private float currentSag;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        fps = GetComponent<FPSInput>();

        line.useWorldSpace = true;
        line.widthMultiplier = ropeWidth;
        line.positionCount = Mathf.Max(ropeSegments, 8);
        ropePts = new Vector3[line.positionCount];
        line.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(fireToggleKey))
        {
            if (isGrappling) ReleaseHook();
            else FireHook();
        }

        if (!isGrappling) return;

        // 1) Anchor updaten (folgt RB, falls vorhanden)
        Vector3 start = ropeStart.position;
        Vector3 anchor = GetAnchorWorld();
        Vector3 toAnchor = anchor - start;
        float dist = toAnchor.magnitude;

        // 2) Reel-In: Seillänge nur hier (pro Frame) verkürzen
        if (Input.GetKey(reelInKey))
            ropeLength = Mathf.Max(ropeLength - reelInSpeed * Time.deltaTime, minRopeLength);

        // 3) Zug nur wenn straff
        bool taut = dist > ropeLength + tautEpsilon;

        // ACHSENSAUBER: alles auf der Seilachse rechnen
        Vector3 dir = dist > 1e-5f ? (toAnchor / dist) : Vector3.zero;

        // --- Spieler-Zug (nur ent/along Seil) ---
        // projiziere die aktuelle pullVelocity auf die Seilachse
        float vAlong = Vector3.Dot(pullVelocity, dir);

        if (taut)
        {
            float stretch = dist - ropeLength;                  // > 0
            float accelAlong = pullSpring * stretch - pullDamping * vAlong;
            // integriere NUR entlang der Seilachse
            pullVelocity += dir * (accelAlong * Time.deltaTime);

            // cap nur entlang der Achse
            float newAlong = Mathf.Clamp(Vector3.Dot(pullVelocity, dir), -maxPullSpeed, maxPullSpeed);
            // setze die Along-Komponente; erhalte die orthogonale Komponente (klein)
            pullVelocity = (pullVelocity - dir * Vector3.Dot(pullVelocity, dir)) + dir * newAlong;
        }
        else
        {
            // wenn locker: along-Komponente sanft abbauen
            float decay = Mathf.Min(Mathf.Abs(vAlong), pullDamping * Time.deltaTime * 2f);
            pullVelocity -= dir * Mathf.Sign(vAlong) * decay;
        }

        // 5) Ziel-Rigidbody nur bei Spannung ziehen – streng ent/along Achse, gedämpft & gecappt
        if (pullRigidbodies && targetRb != null && !targetRb.isKinematic && taut)
        {
            // Geschwindigkeit des Ziels entlang der Zugrichtung (zum Spieler hin)
            float vTargetAlong = Vector3.Dot(targetRb.velocity, -dir); // -dir zeigt zum Spieler

            float tStretch = dist - ropeLength; // > 0
            float a = targetSpring * tStretch - targetDamper * vTargetAlong; // gewünschte Beschl.

            a = Mathf.Clamp(a, -maxTargetAccel, maxTargetAccel);
            targetRb.AddForce(-dir * a, ForceMode.Acceleration);

            // Speed-Cap entlang der Achse
            float vAfter = Vector3.Dot(targetRb.velocity, -dir);
            if (vAfter > maxTargetSpeed)
            {
                float excess = vAfter - maxTargetSpeed;
                // reduziere nur die along-Komponente
                targetRb.velocity += dir * excess; // da vAlong auf -dir war
            }
        }

        // 6) Render (längengetreu, Anchor folgt)
        RenderRope(start, anchor, dist);
    }

    private void LateUpdate()
    {
        if (fps != null && isGrappling)
        {
            // Wenn das Seil straff ist, Gravitation aussetzen
            bool ropeTaut = Vector3.Distance(
                (ropeStart ? ropeStart.position : cam.transform.position),
                GetAnchorWorld()
            ) >= ropeLength - 0.05f;  // Toleranz

            fps.suspendGravity = ropeTaut;

            // additiv die Grapple-Velocity übergeben
            fps.externalVelocity += pullVelocity;
        }
    }

    void FireHook()
    {
        Vector3 center = new Vector3(cam.pixelWidth / 2f, cam.pixelHeight / 2f, 0f);
        Ray ray = cam.ScreenPointToRay(center);

        if (!Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, grappleMask, QueryTriggerInteraction.Ignore))
            return;

        isGrappling = true;
        pullVelocity = Vector3.zero;
        currentSag = 0f;

        ropeLength = Vector3.Distance(transform.position, hit.point) * (1.0f + ropeLengthBuffer);

        targetRb = hit.rigidbody;
        if (targetRb != null)
        {
            targetLocalAnchor = targetRb.transform.InverseTransformPoint(hit.point);
        }
        else
        {
            anchorWorld = hit.point; // Weltpunkt bleibt fix
        }

        line.enabled = true;
    }

    void ReleaseHook()
    {
        isGrappling = false;
        line.enabled = false;
        pullVelocity = Vector3.zero;
        targetRb = null;
        currentSag = 0f;

        if (fps != null) fps.suspendGravity = false;
    }

    Vector3 GetAnchorWorld()
    {
        if (targetRb != null)
            return targetRb.transform.TransformPoint(targetLocalAnchor);
        return anchorWorld;
    }

    // ---------- Rope Rendering: Kurve mit passender Länge ----------
    void RenderRope(Vector3 start, Vector3 end, float currentDist)
    {
        float slack = Mathf.Max(0f, ropeLength - currentDist);

        // L ? d + (8*h^2)/(3*d)  =>  h = sqrt( (3*d*slack)/8 )
        float targetSag = 0f;
        if (currentDist > 1e-4f && slack > 0f)
            targetSag = Mathf.Sqrt((3f * currentDist * slack) / 8f);

        float lerp = 1f - Mathf.Exp(-sagSmooth * Time.deltaTime);
        currentSag = Mathf.Lerp(currentSag, targetSag, lerp);

        BuildBezierWithSag(start, end, currentSag, ropePts);
        line.positionCount = ropePts.Length;
        line.SetPositions(ropePts);
    }

    void BuildBezierWithSag(Vector3 start, Vector3 end, float h, Vector3[] buf)
    {
        Vector3 dir = end - start;
        float d = dir.magnitude;
        if (d < 1e-5f)
        {
            for (int i = 0; i < buf.Length; i++) buf[i] = start;
            return;
        }

        Vector3 forward = dir / d;
        Vector3 gravity = Physics.gravity.sqrMagnitude > 0 ? Physics.gravity.normalized : Vector3.down;
        Vector3 down = Vector3.ProjectOnPlane(gravity, forward).normalized;
        if (down.sqrMagnitude < 1e-6f) down = Vector3.up;

        Vector3 mid = (start + end) * 0.5f;
        Vector3 ctrl = mid + down * h;

        int n = buf.Length;
        for (int i = 0; i < n; i++)
        {
            float t = i / (n - 1f);
            buf[i] = (1 - t) * (1 - t) * start + 2 * (1 - t) * t * ctrl + t * t * end;
        }
    }
}