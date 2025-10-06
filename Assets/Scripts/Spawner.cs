using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // spawn area
    public Vector2 xRange = new Vector2(-14.0f, 14.0f);
    public float[] allowedHeightsGB = new float[] { 1.0f };
    public float[] allowedHeightsRed = new float[] { 6.5f };
    public Vector2 zRange = new Vector2(-14.0f, 14.0f);


    // spawn behavior
    public float initialDelay = 1.0f;
    public float respawnDelay = 5.0f;

    // spawn objects
    public GameObject keyPrefabGreen;
    public GameObject keyPrefabBlue;
    public GameObject keyPrefabRed;

    IEnumerator SpawnPickUp(string color, float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject pickUp;
        bool isRed = false;

        switch (color)
        {
            case "Green":
                pickUp = keyPrefabGreen;
                break;
            case "Blue":
                pickUp = keyPrefabBlue;
                break;
            case "Red":
                pickUp = keyPrefabRed;
                isRed = true;
                break;
            default:
                pickUp = keyPrefabGreen;
                break;
        }

        Instantiate(pickUp, findValidPosition(isRed), Quaternion.identity);
    }


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnPickUp("Green", initialDelay));
        StartCoroutine(SpawnPickUp("Blue", initialDelay));
        StartCoroutine(SpawnPickUp("Red", initialDelay));
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void spawn(string color)
    {
        StartCoroutine(SpawnPickUp(color, respawnDelay));
    }

    Vector3 findValidPosition(bool isRed)
    {
        while (true)
        {
            float x = Random.Range(xRange.x, xRange.y);
            float z = Random.Range(zRange.x, zRange.y);
            float y = allowedHeightsGB[Random.Range(0, allowedHeightsGB.Length)];

            if (isRed)
            {
                y = allowedHeightsRed[Random.Range(0, allowedHeightsRed.Length)];
            }

            Vector3 candidate = new Vector3(x, y, z);

            bool hasGround = Physics.Raycast(candidate, Vector3.down, 1.0f);
            bool overlaps = Physics.CheckSphere(candidate, 0.5f);

            if (hasGround && !overlaps)
            {
                return candidate;
            }
        }
    }
}
