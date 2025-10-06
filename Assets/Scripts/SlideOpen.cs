using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SlideOpen : MonoBehaviour
{
    public string color;

    float openZ;
    float closedZ; 

    public float speed = 1.0f;

    public float closeDelay = 5.0f;
    public float closeDistance = 7.5f;

    float direction = -1;
    float interpolate = 0;
    bool isMoving = false;

    GameObject player;
    GameObject controller;
    Spawner spawner;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        controller = GameObject.FindGameObjectWithTag("GameController");
        spawner = controller.GetComponent<Spawner>();

        openZ = transform.localPosition.z + 1.5f;
        closedZ = transform.localPosition.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            // calculate the interpolation amount
            interpolate += direction * speed * Time.deltaTime;

            // if door completely opened
            if (interpolate > 1)
            {
                interpolate = 1;
                isMoving = false;
                StartCoroutine(WaitAndClose());
            }
            // if door completely closed
            else if (interpolate < 0)
            {
                interpolate = 0;
                isMoving = false;
            }
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, Mathf.Lerp(closedZ, openZ, interpolate));
        }

    }

    public void Open()
    {
        isMoving = true;
        direction = 1;
    }

    public void Close()
    {
        isMoving = true;
        direction = -1;

        spawner.spawn(color);
    }

    private IEnumerator WaitAndClose()
    {
        // Warten bis Spieler außerhalb der Distanz ist
        while (player != null && Vector3.Distance(player.transform.position, transform.position) <= closeDistance)
        {
            yield return null;
        }

        // Dann 5 Sekunden warten
        yield return new WaitForSeconds(closeDelay);

        Close();
    }
}
