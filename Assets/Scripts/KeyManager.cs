using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
    public string currentKey = "";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool HasKey(string keyColor)
    {
        return currentKey == keyColor;
    }

    public void PickUpKey(string keyColor)
    {
        currentKey = keyColor;
        Debug.Log("Picked up " + keyColor + " key.");
        GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneController>().PickUpKey(keyColor);
    }

    public void UseKey()
    {
        Debug.Log("Used " + currentKey + " key.");
        currentKey = "";
        GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneController>().ResetKeyText();
    }
}
