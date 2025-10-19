using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HelpTextState
{
    ActiveIncomplete,
    ActiveComplete,
    InactiveIncomplete,
    InactiveComplete,
}

public enum Color
{
    None,
    White,
    Green,
    Yellow,
    Red,
    Magenta,
    Blue,
    Black,
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
            case "White":
                return Color.White;
            case "Green":
                return Color.Green;
            case "Yellow":
                return Color.Yellow;
            case "Red":
                return Color.Red;
            case "Magenta":
                return Color.Magenta;
            case "Blue":
                return Color.Blue;
            case "Black":
                return Color.Black;
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
            case Color.Black:
                return UnityEngine.Color.black;
            default:
                return UnityEngine.Color.black;
        }
    }
}
