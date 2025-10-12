using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Color
{
    None,
    White,
    Green,
    Yellow,
    Red,
    Magenta,
    Blue,
}

public class Enums : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static string ColorToString(Color color)
    {
        return color.ToString();
    }

    public static Color StringToColor(string color)
    {
        switch (color)
        {
            case "None":
                return Color.White;
            case "White":
                return Color.White;
            case "Green":
                return Color.White;
            case "Yellow":
                return Color.White;
            case "Red":
                return Color.White;
            case "Magenta":
                return Color.White;
            case "Blue":
                return Color.White;
            default:
                return Color.None;
        }
    }

    public static UnityEngine.Color ColorToUnityColor (Color color)
    {
        switch (color)
        {
            case Color.White:
                return UnityEngine.Color.white;
            case Color.Green:
                return UnityEngine.Color.green;
            case Color.Yellow:
                return UnityEngine.Color.yellow;
            case Color.Red:
                return UnityEngine.Color.red;
            case Color.Magenta:
                return UnityEngine.Color.magenta;
            case Color.Blue:
                return UnityEngine.Color.blue;
            default:
                return UnityEngine.Color.black;
        }
    }
}
