using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameCollisionDetection : MonoBehaviour
{

    public GameObject OneWheelContainer;

    private OneWheelController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = OneWheelContainer.GetComponent<OneWheelController>();
    }


    void OnCollisionEnter(Collision hit)
    {
        Debug.Log("Collision Detected");
        controller.reset_agent();
    }
}
