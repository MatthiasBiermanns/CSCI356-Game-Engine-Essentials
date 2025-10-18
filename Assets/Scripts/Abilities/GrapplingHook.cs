using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(LineRenderer))]

// set execution after all scripts except fps
[DefaultExecutionOrder(1000)]
public class GrapplingHook : MonoBehaviour
{
    // references
    [SerializeField] private Camera cam;
    [SerializeField] private Transform ropeStart;
    [SerializeField] private LineRenderer line;
    [SerializeField]private FPSInput fps;

    [SerializeField] private KeyCode fireToggleKey = KeyCode.F;
    [SerializeField] private KeyCode reelInKey = KeyCode.P;

    // grapple settings
    [SerializeField] private LayerMask grappleMask = ~0;
    [SerializeField] private float maxRayDistance = 30f;
    [SerializeField] private float minRopeLength = 1.25f;
    [SerializeField] private float ropeLengthBuffer = 0.05f;
    [SerializeField] private float reelInSpeed = 12f;
    [SerializeField] private float tautEpsilon = 0.02f;

    // pull character
    [SerializeField] private float pullSpring = 45f;
    [SerializeField] private float pullDamping = 8f;
    [SerializeField] private float maxPullSpeed = 20f;

    // pull rigidbody
    [SerializeField] float maxTargetAccel = 40f;
    [SerializeField] float maxTargetSpeed = 12f;

    // rope rendering
    [SerializeField] int ropeSegments = 24;
    [SerializeField] float sagSmooth = 16f;
    [SerializeField] float ropeWidth = 0.05f;

    private CharacterController controller;

    // Rope state
    private bool isGrappling;
    private float ropeLength;
    private Vector3 anchorWorld; // if target no rb
    private Rigidbody targetRb;
    private Vector3 targetLocalAnchor;

    // Player pull
    private Vector3 pullVelocity;

    // rendering
    private Vector3[] ropePts;
    private float currentSag;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        fps = GetComponent<FPSInput>();

        // line renderer
        line.useWorldSpace = true;
        line.widthMultiplier = ropeWidth;
        line.positionCount = Mathf.Max(ropeSegments, 8);
        ropePts = new Vector3[line.positionCount];

        // disable line
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

        // update rope anchors
        Vector3 start = ropeStart.position;
        Vector3 anchor = GetAnchorWorld();
        Vector3 toAnchor = anchor - start;
        float distance = toAnchor.magnitude;

        // shorten rope
        if (Input.GetKey(reelInKey))
            ropeLength = Mathf.Max(ropeLength - reelInSpeed * Time.deltaTime, minRopeLength);

        // check if rope is taut --> creates pull
        bool taut = distance > ropeLength + tautEpsilon;

        // calculate pull direction
        Vector3 pullDirection = distance > 1e-4f ? (toAnchor / distance) : Vector3.zero;

        // projection on rope axis
        float velocityAlongRope = Vector3.Dot(pullVelocity, pullDirection);

        if (taut)
        {
            float stretch = distance - ropeLength;

            // calculate accelleration (hooks law)
            float accellerationAlongRope = pullSpring * stretch - pullDamping * velocityAlongRope;
            
            // calculate actual, frame indipendent pull 
            pullVelocity += pullDirection * (accellerationAlongRope * Time.deltaTime);

            // avoid explosion by clamping
            float newVelocityAlongRope = Mathf.Clamp(Vector3.Dot(pullVelocity, pullDirection), -maxPullSpeed, maxPullSpeed);
            
            // recalculate
            // remove old velocityAlongRope and add clamped one
            pullVelocity = (pullVelocity - pullDirection * velocityAlongRope) + pullDirection * newVelocityAlongRope;
        }
        else
        {
            // rope is loose

            // slowly decay pull
            float decay = Mathf.Min(Mathf.Abs(velocityAlongRope), pullDamping * Time.deltaTime * 2f);
            pullVelocity -= pullDirection * Mathf.Sign(velocityAlongRope) * decay;
        }

        // pull rigidbody
        if (targetRb != null && !targetRb.isKinematic && taut)
        {
            // calculate velocity along rope for target
            float velocityTargetAlongRope = Vector3.Dot(targetRb.velocity, -pullDirection);

            float targetStretch = distance - ropeLength;
            float accellerationAlongRopeTarget = pullSpring * targetStretch - pullDamping * velocityTargetAlongRope;

            // clamp accellaration to avoid explosion
            accellerationAlongRopeTarget = Mathf.Clamp(accellerationAlongRopeTarget, -maxTargetAccel, maxTargetAccel);
            targetRb.AddForce(-pullDirection * accellerationAlongRopeTarget, ForceMode.Acceleration);

            // apply speed limit
            float velocityAfter = Vector3.Dot(targetRb.velocity, -pullDirection);
            if (velocityAfter > maxTargetSpeed)
            {
                float excess = velocityAfter - maxTargetSpeed;

                // remove excess
                // -pullDirection is target direction --> pullDirection negates this
                targetRb.velocity += pullDirection * excess;
            }
        }

        // render rope
        RenderRope(start, anchor, distance);
    }

    private void LateUpdate()
    {
        if (fps != null && isGrappling)
        {
            // if rope is taut, negate gravity
            bool ropeTaut = Vector3.Distance(
                (ropeStart ? ropeStart.position : cam.transform.position),
                GetAnchorWorld()
            ) >= ropeLength - 0.05f;  // Toleranz

            fps.suspendGravity = ropeTaut;

            // transfer grappling velocity to fps
            fps.externalVelocity += pullVelocity;
        }
    }

    void FireHook()
    {
        Vector3 center = new Vector3(cam.pixelWidth / 2f, cam.pixelHeight / 2f, 0f);
        Ray ray = cam.ScreenPointToRay(center);

        // check if something hit
        if (!Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, grappleMask, QueryTriggerInteraction.Ignore))
            return;

        isGrappling = true;
        pullVelocity = Vector3.zero;
        currentSag = 0f;

        // apply buffer
        ropeLength = Vector3.Distance(transform.position, hit.point) * (1.0f + ropeLengthBuffer);


        targetRb = hit.rigidbody;
        
        // if rb hit, work with localAnchor
        if (targetRb != null)
        {
            targetLocalAnchor = targetRb.transform.InverseTransformPoint(hit.point);
        }
        else
        {
            anchorWorld = hit.point;
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

    void RenderRope(Vector3 start, Vector3 end, float currentDist)
    {
        float slack = Mathf.Max(0f, ropeLength - currentDist);

        // calculate approximate sag of rope
        // L ? d + (8*h^2)/(3*d)  =>  h = sqrt( (3*d*slack)/8 )
        float targetSag = 0f;
        if (currentDist > 1e-4f && slack > 0f)
            targetSag = Mathf.Sqrt((3f * currentDist * slack) / 8f);

        float lerp = 1f - Mathf.Exp(-sagSmooth * Time.deltaTime);
        currentSag = Mathf.Lerp(currentSag, targetSag, lerp);

        // calculate using bezier
        BuildBezierWithSag(start, end, currentSag, ropePts);
        line.positionCount = ropePts.Length;
        line.SetPositions(ropePts);
    }

    void BuildBezierWithSag(Vector3 startPosition, Vector3 endPosition, float sagHeigth, Vector3[] ropePoints)
    {
        Vector3 direction = endPosition - startPosition;
        float distance = direction.magnitude;
        if (distance < 1e-5f)
        {
            for (int i = 0; i < ropePoints.Length; i++) ropePoints[i] = startPosition;
            return;
        }

        Vector3 forward = direction / distance;
        Vector3 gravity = Physics.gravity.sqrMagnitude > 0 ? Physics.gravity.normalized : Vector3.down;
        Vector3 down = Vector3.ProjectOnPlane(gravity, forward).normalized;
        if (down.sqrMagnitude < 1e-6f) down = Vector3.up;

        Vector3 mid = (startPosition + endPosition) * 0.5f;
        Vector3 ctrl = mid + down * sagHeigth;

        int n = ropePoints.Length;
        for (int i = 0; i < n; i++)
        {
            float t = i / (n - 1f);
            ropePoints[i] = (1 - t) * (1 - t) * startPosition + 2 * (1 - t) * t * ctrl + t * t * endPosition;
        }
    }
}