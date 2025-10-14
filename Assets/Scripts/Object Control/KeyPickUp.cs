using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickUp : MonoBehaviour
{
    public Color keyColor = Color.None;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            KeyManager km = other.GetComponent<KeyManager>();
            if (km != null && km.currentKey == Color.None)
            {
                km.PickUpKey(keyColor);

                // destroy to disable all parts of prefab
                Destroy(gameObject);
            }
        }
    }
}
