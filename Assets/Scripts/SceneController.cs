using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [SerializeField] UIController controller;
    public string currentKey = "None";

    // Start is called before the first frame update
    void Start()
    {
        controller.currentKeyText.text = "Current Key: " + currentKey;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PickUpKey(string key)
    {
        controller.currentKeyText.text = "Current Key: " + key;
    }

    public void ResetKeyText()
    {
        controller.currentKeyText.text = "Current Key: None";
    }

}
