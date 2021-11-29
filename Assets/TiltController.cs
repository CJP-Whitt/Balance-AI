using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltController : MonoBehaviour
{
    [Tooltip("Frame rigidbody")]
    public Rigidbody rb;
    [Tooltip("Gain for torque control")]
    public float frameTorqueGain;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per physics frame
    void FixedUpdate()
    {
        int forward = Input.GetKey("f") ? 1 : 0;
        int backward = Input.GetKey("b") ? 1 : 0;

        float frameTorque = frameTorqueGain * (forward - backward);
        rb.AddTorque(frameTorque, 0, 0);

    }

}
