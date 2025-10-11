using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private Camera[] cams;

    public KeyCode cameraSwitchKey = KeyCode.F5;

    private int currentCam = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(cameraSwitchKey))
        {
            cams[currentCam].enabled = false;

            // deactivate y pitch change in 3rd person
            if ( currentCam == 0 )
            {
                cams[currentCam].GetComponent<MouseLook>().enabled = false;
            }

            currentCam = (currentCam + 1) % cams.Length;
            cams[currentCam].enabled = true;

            // activate pitch in 1st person
            if (currentCam == 0)
            {
                cams[currentCam].GetComponent<MouseLook>().enabled = true;
            }
        }
    }
}
