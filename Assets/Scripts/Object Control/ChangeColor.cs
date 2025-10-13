using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    public UnityEngine.Color currentColor;
    public UnityEngine.Color[] colors = { 
        UnityEngine.Color.white, 
        UnityEngine.Color.green, 
        UnityEngine.Color.yellow, 
        UnityEngine.Color.red, 
        UnityEngine.Color.magenta, 
        UnityEngine.Color.blue,
        UnityEngine.Color.black
    };

    public bool isFreezed = false;

    private List<Collider> collidingObjects = new();

    // Start is called before the first frame update
    void Start()
    {
        currentColor = GetComponent<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        collidingObjects.Add(other);
    }

    public void OnTriggerExit(Collider other)
    {
        collidingObjects.Remove(other);
    }

    public void SwitchColor()
    {
        if (isFreezed) return;

        int index = System.Array.IndexOf(colors, currentColor);
        currentColor = colors[(index + 1) % colors.Length];

        GetComponent<Renderer>().material.color = currentColor;

        // inform potential triggerZones about new color
        foreach (Collider collider in collidingObjects)
        {
            TriggerZone tz = collider.gameObject.GetComponent<TriggerZone>();

            if (tz != null) 
            {
                tz.NotifiedColorChange(gameObject);
            }
        }
    }
}
