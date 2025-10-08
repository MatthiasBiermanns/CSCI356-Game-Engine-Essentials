using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collidable : MonoBehaviour
{
    private GameObject player;
    private float respawnTime = 15.0f;

    private Collider col;
    private Renderer rend;

    public string keyColor = "";

    public Challenge challenge;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        col = GetComponent<Collider>();
        rend = GetComponent<Renderer>();
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
            if (km != null && km.currentKey == "")
            {
                km.PickUpKey(keyColor);
                if (challenge != null)
                {
                    challenge.CompleteChallenge();
                }
                StartCoroutine(Respawn());
            }
        }
        
    }
    IEnumerator Respawn()
    {
        col.enabled = false;
        rend.enabled = false;
        yield return new WaitForSeconds(respawnTime);
        col.enabled = true;
        rend.enabled = true;
    }
}
