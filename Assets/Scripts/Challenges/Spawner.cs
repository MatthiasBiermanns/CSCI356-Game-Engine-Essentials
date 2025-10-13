using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject keyPrefab;
    [SerializeField] Vector3 keyPosition;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnKey(string color)
    {
        Color keyColor = Enums.StringToColor(color);
        GameObject newKey = Instantiate(keyPrefab, keyPosition, Quaternion.identity); ;

        newKey.GetComponent<Renderer>().material.color = Enums.ColorToUnityColor(keyColor);
        newKey.GetComponent<KeyPickUp>().keyColor = keyColor;
    }
}
