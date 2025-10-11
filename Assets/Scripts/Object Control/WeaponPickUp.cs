using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum WeaponType
{
    Handgun,
    MachineGun,
    Grenade,
}

public class WeaponPickUp : MonoBehaviour
{
    private GameObject shooterCam;
    public WeaponType type;

    // Start is called before the first frame update
    void Start()
    {
        shooterCam = GameObject.FindWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // check, that no other object trigger the pickup
        if (!other.CompareTag("Player"))
        {
            return;
        }

        Shooter shooterComponent = shooterCam.GetComponent<Shooter>();

        switch (type)
        {
            case WeaponType.Handgun:
                shooterComponent.activeWeapon = Weapon.Handgun;
                break;
            case WeaponType.MachineGun:
                shooterComponent.activeWeapon = Weapon.MachineGun;
                break;
            case WeaponType.Grenade:
                shooterComponent.grenades += 5;
                break;
        }

        Destroy(this.gameObject);
    }
}
