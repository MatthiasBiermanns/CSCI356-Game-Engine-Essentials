using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickUp : MonoBehaviour
{
    public string keyColor;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        print("collision detected");
        
        // check, that no other object trigger the pickup
        if (!other.CompareTag("Player"))
        {
            return;
        }

        KeyHolder holder = other.GetComponent<KeyHolder>();

        if (holder != null)
        {
            holder.changeKey(keyColor);
            Destroy(this.gameObject);
        }
    }
}
