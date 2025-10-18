using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityController : MonoBehaviour
{
    [SerializeField] UIController controller;
    GameObject player;

    GrapplingHook grapplingHook;
    FPSInput playerControl;



    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        grapplingHook = player.GetComponent<GrapplingHook>();
        playerControl = player.GetComponent<FPSInput>();

        EnableAbilityDoubleJump(playerControl);
        EnableAbilityGrapplingHook(grapplingHook);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableAbilityDoubleJump(bool value)
    {
        if (playerControl != null) 
        { 
            playerControl.doubleJumpActivated = value;
        }

        controller.SwitchDoubleJumpActive(value);
    }

    public void EnableAbilityGrapplingHook(bool value)
    {
        if (grapplingHook != null)
        {
            grapplingHook.enabled = value;
        }

        controller.SwitchGrapplingHookActive(value);
    }
}
