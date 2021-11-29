using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWheelController : MonoBehaviour
{   
    [Header("Wheel Specific")]
    [Tooltip("Wheel rigidbody")]
    public Rigidbody rb_wheel;
    [Tooltip("XYZ Scale for wheel rb & collider")]
    public float rb_wheel_scale;
    [Tooltip("Max angular velocity, default=7")]
    public float maxAngularVel;
    [Tooltip("Gain for torque control")]
    public float motorTorqueGain;

    [Header("Frame Specific")]
    [Tooltip("Frame rigidbody")]
    public Rigidbody rb_frame;
    [Tooltip("Center of mass Y of frame")]
    public float center_of_mass_y;
    [Tooltip("Gain for frame torque control")]
    public float frameTorqueGain;
    

    [Header("Miscallaneous")]
    [Tooltip("Draw gizmos? BOOL")]
    public bool draw_gizmos;


    private float acceleration;
    private float last_velocity = 0;


    // Start is called before the first frame update
    void Start()
    {   
        // OneWheel properties
        rb_wheel.maxAngularVelocity = maxAngularVel; // Max rotational speed of wheel
        rb_wheel.transform.localScale = rb_wheel.transform.localScale * rb_wheel_scale;

        // Rider simulation properties
        rb_frame.centerOfMass = new Vector3(0, center_of_mass_y, 0); // Starting frame COM Y offset: Simulates riders COM
        //TODO: Rider roll simulation
    }

    // Update is called once per physics frame
    void FixedUpdate()
    {
        // Wheel acceleration calc
        acceleration = (rb_wheel.velocity.z - last_velocity) / Time.deltaTime;
        last_velocity = rb_wheel.velocity.z;

        // Motor physics control
        float motorTorque = motorTorqueGain * Input.GetAxis("Horizontal");
        rb_wheel.AddTorque(motorTorque, 0, 0);

        // Frame physics control
        int forward = Input.GetKey("f") ? 1 : 0;
        int backward = Input.GetKey("b") ? 1 : 0;
        float com_tilt = frameTorqueGain * (forward - backward);
        rb_frame.centerOfMass = new Vector3(0, center_of_mass_y, rb_frame.centerOfMass.z + com_tilt);

    }

    private void OnGUI()
    {   
        // Screen readouts
        GUILayout.Label("Velocity: " + Math.Round(rb_wheel.velocity.z * 3600/1000, 2)  + "km/h\n" +
                        "Acceleration: " + Math.Round(acceleration, 2) + "m/s^2\n" +
                        "Wheel RPM: " + Math.Round(rb_wheel.angularVelocity.x * 60 /(2 * Mathf.PI), 2) + "rpm\n" +
                        "Frame RPM: " + Math.Round(rb_frame.angularVelocity.x * 60/(2*Mathf.PI), 2)  + "rpm\n"
            );
    }

    private void OnDrawGizmos()
    {
        if (draw_gizmos) {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(rb_frame.transform.position + rb_frame.transform.rotation * rb_frame.centerOfMass, 0.1f);
        }
    }
}
