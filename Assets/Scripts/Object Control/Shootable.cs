using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class Shootable : MonoBehaviour
{
    public int health = 10;
    
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HitObject(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
            
        } else
        {
            transform.localScale = transform.localScale * Mathf.Pow(0.9f, damage);
        }
    }
}
