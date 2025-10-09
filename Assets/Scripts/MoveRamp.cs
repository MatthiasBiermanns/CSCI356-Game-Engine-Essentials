using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MoveRamp : MonoBehaviour
{
    public string color;

    float openZ;
    float closedZ;

    public float raiseAmount = 1.6f;
    public float speed = 1.0f;

    private float downY;
    private float upY;

    private int direction = 1; // 1 for up, -1 for down
    private float t = 0f;
    private bool isMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        upY = transform.localPosition.y + raiseAmount;
        downY = transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            t += direction * speed * Time.deltaTime;

            if (t >= 1f)
            {
                t = 1f;
                isMoving = false;
            }
            else if (t <= 0f)
            {
                t = 0f;
                isMoving = false;
            }
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(downY, upY, t), transform.localPosition.z);
        }

    }

    public void MoveUp()
    {
        isMoving = true;
        direction = 1;
    }

    public void MoveDown()
    {
        isMoving = true;
        direction = -1;
    }

}
