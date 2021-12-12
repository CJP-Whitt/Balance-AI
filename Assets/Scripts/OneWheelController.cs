using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWheelController : MonoBehaviour
{   
    // *** Public user defined parameters, and recomended values ***

    [Header("Wheel Specific Params")]
    [Tooltip("Wheel rigidbody")] public Rigidbody rbWheel; 
    // Regulates scale of motor rb & collider
    [Tooltip("XYZ wheel scale range")] [Range(0.8f, 1f)] public float rbWheel_scale_min; // .9
    [Tooltip("XYZ wheel scale range")] [Range(1f, 1.5f)] public float rbWheel_scale_max; // 1.4
    
    // Regulates max rotational speed of agent
    [Tooltip("Max angular velocity")] [Range(50, 100)] public float ang_vel_max; // 75
    
    // Regulates torque of motor
    [Tooltip("Min gain for motor torque control")] [Range(75, 100)] public float motor_torque_gain_min; // 100
    [Tooltip("Max gain for motor torque control")] [Range(100, 200)] public float motor_torque_gain_max; // 200
    

    [Header("Frame Specific Params")]
    [Tooltip("Frame rigidbody")] public Rigidbody rbFrame;
    // Frame COM Y offset range
    [Tooltip("Center of mass Y offset min of frame")] [Range(.1f, .5f)] public float com_y_min; // 0.5
    [Tooltip("Center of mass Y offset max of frame")] [Range(.5f, 2f)] public float com_y_max; // 2

    // Frame COM Z initial offset range and running range
    [Tooltip("Center of mass Z offset min of frame")] [Range(-0.5f, -0.1f)] public float com_z_min; // -0.2
    [Tooltip("Center of mass Z offset max of frame")] [Range(0.1f, 0.5f)] public float com_z_max; // 0.2
    //***TODO: Rider roll simulation***

    // Regulates tilt of frame/rider
    [Tooltip("Min gain for frame torque control")] [Range(.001f, .005f)] public float frame_torque_gain_min; // .005
    [Tooltip("Max gain for frame torque control")] [Range(.005f, .015f)] public float frame_torque_gain_max; // .01

        
    // Simulated rider weight, at COM: [50kg, 100kg] *actually rbFrame mass
    [Tooltip("Rider mass min")] [Range(50, 75)] public int rider_mass_min; // 75
    [Tooltip("Rider mass max")] [Range(75, 100)] public int rider_mass_max; // 100
       
    
    [Header("Miscallaneous")]
    [Tooltip("Draw gizmos?")] public bool draw_gizmos;

    private float motor_torque_gain;
    private float frame_torque_gain;


    // Agent state global tool vars
    private float acceleration;
    private float last_velocity = 0;
    private float frontHitDist;
    private float backHitDist;




    // Start is called before the first frame update
    void Start()
    {
        // OneWheel properties
        motor_torque_gain = UnityEngine.Random.Range(motor_torque_gain_min, motor_torque_gain_max); // Randomize motor stength, simulates motor stength differences
        rbWheel.maxAngularVelocity = ang_vel_max; // Max rotational speed of wheel, simulates different max speeds of motors
        rbWheel.transform.localScale = rbWheel.transform.localScale * UnityEngine.Random.Range(rbWheel_scale_min, rbWheel_scale_max); // Randomize wheel scale: Simulates different tire sizes
        //***TODO: Calculate max angle***

        // Rider simulation properties
        frame_torque_gain = UnityEngine.Random.Range(frame_torque_gain_min, frame_torque_gain_max); // Randomize rider tilt rate of change, simualtes rider lean speed.
        rbFrame.mass = UnityEngine.Random.Range(rider_mass_min, rider_mass_max + 1); // Randomize frame mass within range, simulates rider mass
        rbFrame.centerOfMass = new Vector3(0, UnityEngine.Random.Range(com_y_min, com_y_max), UnityEngine.Random.Range(com_z_min, com_z_max)); // Randomize COM Y offset: Simulates riders COM 
        //***TODO: Rider roll simulation***

        
    }

    // Update is called once per physics frame
    void FixedUpdate()
    {
        // Wheel acceleration calc
        acceleration = (rbWheel.velocity.z - last_velocity) / Time.deltaTime;
        last_velocity = rbWheel.velocity.z;

        // Motor physics control
        float motorTorque = motor_torque_gain * Input.GetAxis("Horizontal");
        rbWheel.AddTorque(motorTorque, 0, 0);

        // Frame physics control
        int forward = Input.GetKey("f") ? 1 : 0;
        int backward = Input.GetKey("b") ? 1 : 0;
        float com_tilt = frame_torque_gain * (forward - backward);
        if (com_tilt > 0 && rbFrame.centerOfMass.z < com_z_max)
        {
            rbFrame.centerOfMass = new Vector3(0, rbFrame.centerOfMass.y, rbFrame.centerOfMass.z + com_tilt);
        }
        if (com_tilt < 0 && rbFrame.centerOfMass.z > com_z_min)
        {
            rbFrame.centerOfMass = new Vector3(0, rbFrame.centerOfMass.y, rbFrame.centerOfMass.z + com_tilt);
        }
        //***TODO: Add simualted rider movement for training (sin, cos, tan)

        // Parralelness observatioins
        int layer_mask = 1 << 6; // Only collide with floor
        RaycastHit frontRayHit;
        RaycastHit backRayHit;
        if (Physics.Raycast(rbFrame.transform.position + rbFrame.transform.forward * .3f, -rbFrame.transform.up, out frontRayHit, maxDistance: 1f, layerMask: layer_mask))
        {
            frontHitDist = frontRayHit.distance;
        } else
        {
            frontHitDist = 2.5f;
        }
        if (Physics.Raycast(rbFrame.transform.position - rbFrame.transform.forward * .3f, -rbFrame.transform.up, out backRayHit, maxDistance: 1f, layerMask: layer_mask))
        {
            backHitDist = backRayHit.distance;
        }
        else
        {
            backHitDist = 2.5f;
        }
        Ray frontRay = new Ray(rbFrame.transform.position + rbFrame.transform.forward * .3f, -rbFrame.transform.up);
        Ray backRay = new Ray(rbFrame.transform.position - rbFrame.transform.forward * .3f, -rbFrame.transform.up);
        Debug.DrawRay(frontRay.origin, frontRay.direction * frontHitDist, Color.red);
        Debug.DrawRay(backRay.origin, backRay.direction * backHitDist, Color.red);

    }

    private void OnGUI()
    {   
        // Screen readouts
        GUILayout.Label("WHEEL: \n" + 
                        "Velocity: " + Math.Round(rbWheel.velocity.z * 3600/1000, 2)  + "km/h\n" +
                        "Acceleration: " + Math.Round(acceleration, 2) + "m/s^2\n" +
                        "RPM: " + Math.Round(rbWheel.angularVelocity.x * 60 /(2 * Mathf.PI), 2) + "rpm\n" +
                        "FRAME: \n" +
                        "Pitch: " + Math.Round(rbFrame.rotation.x * 180f, 3) + "deg\n" +
                        "FrontDist: " + Math.Round(frontHitDist, 3) + "m\n" +
                        "BackDist: " + Math.Round(backHitDist, 3) + "m\n" +
                        "COM Y Offset: " + Math.Round(rbFrame.centerOfMass.y, 2) + "m\n" +
                        "COM Z Offset: " + Math.Round(rbFrame.centerOfMass.z, 2) + "m\n"
            );
    }

    private void OnDrawGizmos()
    {
        if (draw_gizmos) {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(rbFrame.transform.position + rbFrame.transform.rotation * rbFrame.centerOfMass, 0.1f);
        }
    }
}
