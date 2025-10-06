using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyHolder : MonoBehaviour
{
    public static string NoKeyString = "none";

    public string currentKey = KeyHolder.NoKeyString;
    public float openingRange = 5.0f;

    GameObject controller;
    SceneController sceneController;
    Spawner spawner;

    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        sceneController = controller.GetComponent<SceneController>();
    }

    // Update is called once per frame
    void Update()
    {
        if( Input.GetKeyDown(KeyCode.C))
        { 

            if (currentKey != KeyHolder.NoKeyString)
            {
                Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, openingRange);

                foreach (Collider collider in nearbyObjects)
                {
                    SlideOpen door = collider.GetComponent<SlideOpen>();

                    if (door != null && door.color == currentKey)
                    {
                        door.Open();
                        changeKey(KeyHolder.NoKeyString);
                        sceneController.increaseDoorsOpened(1);

                        break;
                    }
                }
            }
        }
    }

    public void changeKey(string keyColor)
    {
        if (currentKey != KeyHolder.NoKeyString)
        {
            spawner.spawn(currentKey);
        }
        currentKey = keyColor;

        if (sceneController != null)
        {
            sceneController.changeCurrentKey(currentKey);
        }
    }
}
