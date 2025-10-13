using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject simpleKeyPrefab;
    [SerializeField] GameObject detailedKeyPrefab;

    [SerializeField] Material redMaterial;
    [SerializeField] Material greenMaterial;
    [SerializeField] Material blueMaterial;
    [SerializeField] Material blackMaterial;

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

        bool useDetailedPrefab = keyColor == Color.Red ||
                                keyColor == Color.Green ||
                                keyColor == Color.Blue ||
                                keyColor == Color.Black;
        GameObject newKey;

        if (useDetailedPrefab)
        {
            newKey = Instantiate(detailedKeyPrefab, keyPosition, Quaternion.identity);

            newKey.GetComponentsInChildren<Renderer>()
                .First((Renderer r) => r.name == "sm_key_01")
                .material = ColorToKeyMaterial(keyColor);
            //newKey.GetComponentInChildren<Renderer>().material = ColorToKeyMaterial(keyColor);
        } else
        {
            newKey = Instantiate(simpleKeyPrefab, keyPosition, Quaternion.identity);

            newKey.GetComponent<Renderer>().material.color = Enums.ColorToUnityColor(keyColor);
        }
        
        newKey.GetComponent<KeyPickUp>().keyColor = keyColor;
    }

    public Material ColorToKeyMaterial(Color color)
    {
        switch (color)
        {
            case Color.Red:
                return redMaterial;
            case Color.Green:
                return greenMaterial;
            case Color.Blue:
                return blueMaterial;
            case Color.Black:
                return blackMaterial;
            default:
                return blackMaterial;
        }
    }
}
