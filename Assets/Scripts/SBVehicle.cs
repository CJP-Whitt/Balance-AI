using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*************************************************************************************
*   Class: SBVehicle
*   - Class to represent a Self Balancing Vehicle and simualted rider in Unity
*   - Tuple properties are <min, max> format
*************************************************************************************/
public class SBVehicle // Self balancing vehicle class
{
    public Tuple<float, float> motorTorqueGainRange { get;  set; }
    public Tuple<float, float> frameTorqueGainRange { get;  set; }
    public Tuple<float, float> wheelScaleRange;
    //public Tuple<float, float> comXAxisRange;
    public Tuple<float, float> comYAxisRange;
    public Tuple<float, float> comZAxisRange;
    public float frontClearance { get; set; }
    public float rearClearance { get; set; } 
    public int rpmMax { get; set; }
    public float acceleration { get; set; }
    public float velocity { get; set; }
    public float lastVelocity { get; set; }
    public Tuple<int, int> riderMassRange { get; set; }
    public int floorLayerNum { get; set; }

    private Rigidbody wheel;
    private Rigidbody frame;
    private Collider frameCollider;
    private Vector3 wheelPosStart;
    private Quaternion wheelRotStart;
    private Vector3 wheelScaleStart;
    private Vector3 framePosStart;
    private Quaternion frameRotStart;
    private float motorTorqueGain;
    private float frameTorqueGain;
    private float comXAxis;
    private float comYAxis;
    private float comZAxis;

    /*************************************************************************************
    *   Constructor: SBVehicle 
    *   - Initializes variables and records birth state
    *************************************************************************************/
    public SBVehicle(Rigidbody w, Rigidbody f)
    {
        wheel = w;
        frame = f;
        wheel.maxAngularVelocity = 100; // Hard code to 100

        // Save initial vehicle state
        wheelPosStart = wheel.position;
        wheelRotStart = wheel.rotation;
        framePosStart = frame.position;
        frameRotStart = frame.rotation;

        Update();
    }

    /*************************************************************************************
    *   Function: Update 
    *   - Updates vehicle state
    *************************************************************************************/
    public void Update() // Update vehicle 
    {

        // 1. Motor physics control
        float motorTorque = motorTorqueGain * Input.GetAxis("Horizontal");
        wheel.AddTorque(motorTorque, 0, 0);

        // 2. Frame physics control
        int forward = Input.GetKey("f") ? 1 : 0;
        int backward = Input.GetKey("b") ? 1 : 0;
        float com_tilt = frameTorqueGain * (forward - backward);
        if (com_tilt > 0 && frame.centerOfMass.z < comZAxisRange.Item1)
        {
            frame.centerOfMass = new Vector3(0, frame.centerOfMass.y, frame.centerOfMass.z + com_tilt);
        }
        if (com_tilt < 0 && frame.centerOfMass.z > comZAxisRange.Item2)
        {
            frame.centerOfMass = new Vector3(0, frame.centerOfMass.y, frame.centerOfMass.z + com_tilt);
        }
        // TODO: Add simualted rider movement for training (sin, cos, tan, log, ln)

        // 3. Parralelness Observation
        int layerMask = 1 << floorLayerNum; // Only collide with floor
        RaycastHit frontRayHit;
        RaycastHit backRayHit;
        if (Physics.Raycast(frame.transform.position + frame.transform.forward * .3f, -frame.transform.up, out frontRayHit, maxDistance: 1f, layerMask: layerMask))
        {
            frontClearance = frontRayHit.distance;
        }
        else
        {
            frontClearance = 2.5f;
        }
        if (Physics.Raycast(frame.transform.position - frame.transform.forward * .3f, -frame.transform.up, out backRayHit, maxDistance: 1f, layerMask: layerMask))
        {
            rearClearance = backRayHit.distance;
        }
        else
        {
            rearClearance = 2.5f;
        }
        Ray frontRay = new Ray(frame.transform.position + frame.transform.forward * .3f, -frame.transform.up);
        Ray backRay = new Ray(frame.transform.position - frame.transform.forward * .3f, -frame.transform.up);
        Debug.DrawRay(frontRay.origin, frontRay.direction * frontClearance, Color.red);
        Debug.DrawRay(backRay.origin, backRay.direction * rearClearance, Color.red);

        // 4. Wheel acceleration calc
        acceleration = (wheel.velocity.z - lastVelocity) / Time.deltaTime;
        lastVelocity = wheel.velocity.z;
        velocity = wheel.velocity.z;

    }

    /*************************************************************************************
    *   Function: Reset
    *   - Resets vehicle state
    *   - int flag: (1) Same as init start params, (0/Default) Randomize Start Params
    *************************************************************************************/
    public void Reset(int flag=0)
    {
        // Reset vehicle to pos, rot, vel
        wheel.position = wheelPosStart;
        wheel.rotation = wheelRotStart;
        frame.position = framePosStart;
        frame.rotation = frameRotStart;
        lastVelocity = 0;
        
        if (flag == 0)
        {
            // OneWheel properties
            motorTorqueGain = UnityEngine.Random.Range(motorTorqueGainRange.Item1, motorTorqueGainRange.Item2); // Randomize motor stength, simulates motor stength differences
            wheel.transform.localScale = wheelScaleStart * UnityEngine.Random.Range(wheelScaleRange.Item1, wheelScaleRange.Item2); // Randomize wheel scale: Simulates different tire sizes
            //***TODO: Calculate max angle***

            // Rider simulation properties
            frameTorqueGain = UnityEngine.Random.Range(frameTorqueGainRange.Item1, frameTorqueGainRange.Item2); // Randomize rider tilt rate of change, simualtes rider lean speed.
            frame.mass = UnityEngine.Random.Range(riderMassRange.Item1, riderMassRange.Item2 + 1); // Randomize frame mass within range, simulates rider mass
            frame.centerOfMass = new Vector3(0, UnityEngine.Random.Range(comYAxisRange.Item1, comYAxisRange.Item2), UnityEngine.Random.Range(comZAxisRange.Item1, comYAxisRange.Item2)); // Randomize COM Y/Z offset: Simulates riders COM 
            //***TODO: Rider roll simulation***
        }
        else if (flag == 1)
        {
            
        }
        else
        {
            Debug.LogException(new Exception("SBVehicle>Reset() - Flag argument invalid"));
        }

        Update();
    }
}
