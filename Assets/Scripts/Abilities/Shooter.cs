using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapon
{
    Handgun,
    MachineGun
}

public class Shooter : MonoBehaviour
{
    private Camera cam;     // stores camera component

    public GameObject grenadePrefab;
    public float grenadeImpulse = 5.0f;
    public int grenades = 0;
    private float grenadeThrowStart = 0.0f;
    public float maxGrenadeHoldTime = 2.0f;

    public Weapon activeWeapon = Weapon.Handgun;
    public float machineGunCD = 0.1f;
    public float machineGunSpreadFactor = 1.0f;
    public int shotDamage = 1;

    Coroutine machineGunRoutine;

    IEnumerator SphereIndicator(Vector3 pos)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        sphere.transform.localPosition = pos;

        yield return new WaitForSeconds(2);
        Destroy(sphere);
    }

    IEnumerator MachineGunLoop()
    {
        while (true)
        {
            float randomX = Random.Range(-1.0f, 1.0f) * machineGunSpreadFactor;
            float randomY = Random.Range(-1.0f, 1.0f) * machineGunSpreadFactor;

            Vector3 point = new Vector3(cam.pixelWidth / 2 + randomX, cam.pixelHeight / 2 + randomY, 0);

            Fire(point);
            yield return new WaitForSeconds(machineGunCD);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // gets the GameObject's camera component
        cam = GetComponent<Camera>();

        // hide the mouse cursor at the centre of screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnGUI()
    {
        int size = 12;

        // centre of screen and caters for font size
        float posX = cam.pixelWidth / 2 - size / 4;
        float posY = cam.pixelHeight / 2 - size / 2;

        // displays "*" on screen
        GUI.Label(new Rect(posX, posY, size, size), "*");
    }

    // Update is called once per frame
    void Update()
    {
        switch (activeWeapon)
        {
            case Weapon.Handgun:
                InvokeHandgun();
                break;
            case Weapon.MachineGun:
                InvokeMachineGun();
                break;
        }

        if (Input.GetMouseButtonDown(1) && grenades > 0)
        {
            grenadeThrowStart = Time.time;
        }

        if (Input.GetMouseButtonUp(1) && grenades > 0)
        {
            float heldTime = Time.time - grenadeThrowStart;
            float impulseModifier = 1.0f + Mathf.Clamp01( heldTime / maxGrenadeHoldTime);

            print(heldTime.ToString());
            print(impulseModifier.ToString());

            GameObject grenade = Instantiate(grenadePrefab, transform);
            grenade.transform.position = cam.transform.position + cam.transform.forward * 2;

            Rigidbody target = grenade.GetComponent<Rigidbody>();

            Vector3 impulse = cam.transform.forward * grenadeImpulse * impulseModifier;
            target.AddForceAtPosition(impulse, cam.transform.position, ForceMode.Impulse);

            grenades -= 1;
        }
    }

    void Fire(Vector3 point)
    {
        // create a ray from the point in the direction of the camera
        Ray ray = cam.ScreenPointToRay(point);

        RaycastHit hit; // stores ray intersection information

        // ray cast will obtain hit information if it intersects anything
        if (Physics.Raycast(ray, out hit))
        {
            // get the GameObject that was hit
            GameObject hitObject = hit.transform.gameObject;
            Shootable target = hitObject.GetComponent<Shootable>();
            ChangeColor colorTarget = hitObject.GetComponent<ChangeColor>();

            if (target != null)
            {
                target.HitObject(shotDamage);
            } else if (colorTarget != null) 
            {
                //colorTarget.SetRandomColor();
                colorTarget.SwitchColor();
            }
            else
            {
                StartCoroutine(SphereIndicator(hit.point));
            }

                
        }
    }

    void InvokeHandgun()
    {
        // on left mouse button click
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 point = new Vector3(cam.pixelWidth / 2, cam.pixelHeight / 2, 0);
            Fire(point);
        }
    }

    void InvokeMachineGun()
    {
        if (Input.GetMouseButtonDown(0) && machineGunRoutine == null)
        {
            machineGunRoutine = StartCoroutine(MachineGunLoop());
        }

        if(Input.GetMouseButtonUp(0) && machineGunRoutine != null)
        {
            StopCoroutine(machineGunRoutine);
            machineGunRoutine = null;
        }
    }
}
