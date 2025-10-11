using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    public UnityEngine.Color currentColor;
    public UnityEngine.Color[] colors = { UnityEngine.Color.white, UnityEngine.Color.green, UnityEngine.Color.yellow, UnityEngine.Color.red, UnityEngine.Color.magenta, UnityEngine.Color.blue};
    public Challenge challenge;

    public bool isFreezed = false;


    public void SwitchColor()
    {
        if (isFreezed) return;

        if (currentColor == colors[colors.Length - 1])
        {
            currentColor = colors[0];
            if (challenge != null)
            {
                challenge.CompleteChallenge();
            }
        }
        else
        {
            int index = System.Array.IndexOf(colors, currentColor);
            currentColor = colors[index + 1];
        }
        GetComponent<Renderer>().material.color = currentColor;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentColor = GetComponent<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
