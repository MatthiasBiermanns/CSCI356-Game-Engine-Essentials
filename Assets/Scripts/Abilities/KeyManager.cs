using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
    public Color currentKey = Color.None;
    public float openingRange = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (currentKey != Color.None)
            {
                Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, openingRange);

                foreach (Collider collider in nearbyObjects)
                {
                    SlideOpen door = collider.GetComponent<SlideOpen>();

                    if (door != null)
                    {
                        if (door.UnlockDoor(currentKey))
                        {
                            UseKey();
                            break;
                        }
                    }
                }
            }
        }
    }

    public bool HasKey(Color keyColor)
    {
        return currentKey == keyColor;
    }

    public void PickUpKey(Color keyColor)
    {
        currentKey = keyColor;
        Debug.Log("Picked up " + keyColor + " key.");
        GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneController>().PickUpKey(keyColor);
    }

    public void UseKey()
    {
        Debug.Log("Used " + currentKey + " key.");
        currentKey = Color.None;
        GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneController>().ResetKeyText();
    }
}
