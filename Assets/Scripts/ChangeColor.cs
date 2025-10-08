using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    public Color currentColor;
    public Color[] colors = { Color.white, Color.blue, Color.green, Color.red, Color.yellow };
    public Challenge challenge;

    public void SetRandomColor()
    {
        Color random = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        GetComponent<Renderer>().material.color = random;
    }

    public void SwitchColor()
    {
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

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
