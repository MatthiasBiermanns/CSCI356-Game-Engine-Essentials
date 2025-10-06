using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [SerializeField] UIController controller;
    int doorsOpened = 0;
    string currentKey;

    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        currentKey = player.GetComponent<KeyHolder>().currentKey;
        controller.currentKeyTag.text = currentKey;

        controller.doorsOpenedCounter.text = doorsOpened.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void increaseDoorsOpened(int value)
    {
        doorsOpened += value;
        controller.doorsOpenedCounter.text = doorsOpened.ToString();
    }

    public void changeCurrentKey(string key)
    {
        currentKey = key;
        controller.currentKeyTag.text = currentKey;
    }
}
