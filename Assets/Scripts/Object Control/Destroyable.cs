using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class Destroyable : MonoBehaviour
{
    public float minSurvivingTime = 0.2f;
    public float maxSurvivingTime = 1.5f;
    public float suriveChance = 0.7f;

    IEnumerator DestroyObject(GameObject gObj)
    {
        yield return new WaitForSeconds(Random.Range(minSurvivingTime, maxSurvivingTime));
        Destroy(gObj);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HitWithExplosion()
    {
        //50% chance to get destroyed, otherwise just fly away
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.None;

        if (Random.Range(0f, 1f) > suriveChance)
        {
            StartCoroutine(DestroyObject(gameObject));
        }
    }
}
