using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WheelColliders
{
    public WheelCollider LF, LR, RF, RR;
}

[System.Serializable]
public class WheelMeshes
{
    public MeshRenderer LF, LR, RF, RR;
}

[System.Serializable]
public class WheelParticles
{
    public ParticleSystem LF, LR, RF, RR;
}

public class CarController : MonoBehaviour
{
    // Car elements
    private Rigidbody rb;
    public WheelColliders WheelColliders;
    public WheelMeshes WheelMeshes;
    public WheelParticles WheelParticles;
    public GameObject smokePrefab;

    // Inputs
    public float acceleration;
    public float steering;
    public float brake;

    // Car properties
    public float torque;
    public float brakePower;

    // Vectors and angles
    private float slipAngle;
    private float speed;
    public AnimationCurve steeringCurve;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        CreateSmoke();
    }

    void CreateSmoke()
    {
        WheelParticles.RF = Instantiate(smokePrefab, WheelColliders.RF.transform.position - Vector3.up * WheelColliders.RF.radius,
            Quaternion.identity, WheelColliders.RF.transform).GetComponent<ParticleSystem>();

        WheelParticles.LF = Instantiate(smokePrefab, WheelColliders.LF.transform.position - Vector3.up * WheelColliders.LF.radius,
            Quaternion.identity, WheelColliders.LF.transform).GetComponent<ParticleSystem>();

        WheelParticles.RR = Instantiate(smokePrefab, WheelColliders.RR.transform.position - Vector3.up * WheelColliders.RR.radius,
            Quaternion.identity, WheelColliders.RR.transform).GetComponent<ParticleSystem>();

        WheelParticles.LR = Instantiate(smokePrefab, WheelColliders.LR.transform.position - Vector3.up * WheelColliders.LR.radius,
            Quaternion.identity, WheelColliders.LR.transform).GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        speed = rb.velocity.magnitude;
        CheckInput();
        ApplyTorque();
        ApplySteering();
        ApplyBrake();
        UpdateWheelsPos();
        CheckParticles();
    }

    void CheckInput()
    {
        acceleration = Input.GetAxis("Vertical");
        steering = Input.GetAxis("Horizontal");
        slipAngle = Vector3.Angle(transform.forward, rb.velocity - transform.forward);
        if (slipAngle < 120f)
        {
            if (acceleration < 0)
            {
                brake = Mathf.Abs(acceleration);
                acceleration = 0;
            }
        }
        else
        {
            brake = 0;
        }
    }

    void ApplySteering()
    {
        float angle = steering * steeringCurve.Evaluate(speed);
        WheelColliders.RF.steerAngle = angle;
        WheelColliders.LF.steerAngle = angle;
    }

    void ApplyBrake()
    {
        WheelColliders.RF.brakeTorque = brake * brakePower * 0.6f;
        WheelColliders.LF.brakeTorque = brake * brakePower * 0.6f;

        WheelColliders.RR.brakeTorque = brake * brakePower * 0.3f;
        WheelColliders.LR.brakeTorque = brake * brakePower * 0.3f;
    }

    void ApplyTorque()
    {
        WheelColliders.RR.motorTorque = torque * acceleration;
        WheelColliders.LR.motorTorque = torque * acceleration;
    }


    void UpdateWheelsPos()
    {
        UpdateSingleWheel(WheelColliders.LF, WheelMeshes.LF);
        UpdateSingleWheel(WheelColliders.LR, WheelMeshes.LR);
        UpdateSingleWheel(WheelColliders.RF, WheelMeshes.RF);
        UpdateSingleWheel(WheelColliders.RR, WheelMeshes.RR);
    }

    void CheckParticles()
    {
        WheelHit[] wheelHits = new WheelHit[4];

        WheelColliders.RF.GetGroundHit(out wheelHits[0]);
        WheelColliders.LF.GetGroundHit(out wheelHits[1]);
        WheelColliders.RR.GetGroundHit(out wheelHits[2]);
        WheelColliders.LR.GetGroundHit(out wheelHits[3]);

        float slipAllowance = 0.3f;
        if (Mathf.Abs(wheelHits[0].sidewaysSlip) + Mathf.Abs(wheelHits[0].forwardSlip) > slipAllowance)
        {
            WheelParticles.RF.Play();
        }
        else
        {
            WheelParticles.RF.Stop();
        }

        if (Mathf.Abs(wheelHits[1].sidewaysSlip) + Mathf.Abs(wheelHits[1].forwardSlip) > slipAllowance)
        {
            WheelParticles.LF.Play();
        }
        else
        {
            WheelParticles.LF.Stop();
        }

        if (Mathf.Abs(wheelHits[2].sidewaysSlip) + Mathf.Abs(wheelHits[2].forwardSlip) > slipAllowance)
        {
            WheelParticles.RR.Play();
        }
        else
        {
            WheelParticles.RR.Stop();
        }

        if (Mathf.Abs(wheelHits[3].sidewaysSlip) + Mathf.Abs(wheelHits[3].forwardSlip) > slipAllowance)
        {
            WheelParticles.LR.Play();
        }
        else
        {
            WheelParticles.LR.Stop();
        }
    }

    // Updates textures to collider rotation and position
    void UpdateSingleWheel(WheelCollider wc, MeshRenderer mr)
    {
        Quaternion quat;
        Vector3 position;

        wc.GetWorldPose(out position, out quat);
        mr.transform.position = position;
        mr.transform.rotation = quat;
    }
}
