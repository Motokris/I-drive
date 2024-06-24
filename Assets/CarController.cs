using System.Collections;
using TMPro;
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

public enum GearState
{
    Neutral,
    Running,
    CheckChange,
    Change
}

public class CarController : MonoBehaviour
{
    // Car elements
    private Rigidbody rb;
    public WheelColliders WheelColliders;
    public WheelMeshes WheelMeshes;

    // Inputs
    public float acceleration, brake, steering;

    // Car properties
    public float engineTorque, brakePower, RPM, redLine, idleRPM, diffRatio, increaseGearRPM, decreaseGearRPM, clutch;
    public float[] gearRatios;
    private float speedClamp, currentTorque, wheelRPM;
    private bool handbrake = false;
    public int engineRunning, currentGear;
    private GearState gearState;

    // Vectors and angles
    public float speed;
    private float slipAngle;
    public float maxSpeed;
    public AnimationCurve steeringCurve, HPtoRPMCurve;

    // UI
    public TMP_Text rpmText, gearText;
    public Transform rpmNeedle;
    public float minNeedleRotation, maxNeedleRotation;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rpmNeedle.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(minNeedleRotation, maxNeedleRotation, RPM / (redLine * 1.1f))); // Determine how much to rotate needle between minRotaion and maxRotation and RPM
        rpmText.text = "" + (int)RPM;
        gearText.text = (gearState == GearState.Neutral) ? "N" : (currentGear + 1).ToString();
        speed = WheelColliders.RR.rpm * WheelColliders.RR.radius * 2f * Mathf.PI * 60f / 1000f; // Formula for speed: circumference of wheel (2*radius*pi) * rpm of wheel * 60 / 1000 (to transform into km/h)
        speedClamp = Mathf.Lerp(speedClamp, speed, Time.deltaTime);
        ApplyTorque();
        ApplyBrake();
        UpdateWheelsPos();
        CheckParticles();
    }

    public void SetInput(float throttleInput, float steeringInput, float clutchInput, float handbrakeInput)
    {
        acceleration = throttleInput;
        steering = steeringInput;
        slipAngle = Vector3.Angle(transform.forward, rb.velocity - transform.forward);

        float movingDirection = Vector3.Dot(transform.forward, rb.velocity);

        if (Mathf.Abs(acceleration) > 0 && engineRunning == 0)
        {
            StartCoroutine(GetComponent<EngineAudio>().StartEngine());
            gearState = GearState.Running;
        }
        ApplySteering();

        if (gearState != GearState.Change)
        {
            if (gearState == GearState.Neutral)
            {
                clutch = 0;
                if (Mathf.Abs(acceleration) > 0)
                    gearState = GearState.Running;
            }
            else
            {
                clutch = Mathf.Abs(1 - clutchInput);
            }
        }
        else
        {
            clutch = 0;
        }

        if (movingDirection < -0.5f && acceleration > 0)
        {
            brake = Mathf.Abs(acceleration);
        }
        else if (movingDirection > 0.5f && acceleration < 0)
        {
            brake = Mathf.Abs(acceleration);
        }
        else
        {
            brake = 0;
        }
        handbrake = (handbrakeInput > 0.5f);
    }


    /// <summary>
    /// Based on the value of the car's speed, take the steering angle from the curve and apply it in the [-90, 90] degree space
    /// </summary>
    void ApplySteering()
    {
        float angle = steering * steeringCurve.Evaluate(speed);
        if (slipAngle < 120f)
        {
            angle += Vector3.SignedAngle(transform.forward, rb.velocity + transform.forward, Vector3.up);
        }
        angle = Mathf.Clamp(angle, -90f, 90f);
        WheelColliders.RF.steerAngle = angle;
        WheelColliders.LF.steerAngle = angle;
    }

    void ApplyBrake()
    {
        WheelColliders.RF.brakeTorque = brake * brakePower * 0.7f;
        WheelColliders.LF.brakeTorque = brake * brakePower * 0.7f;

        WheelColliders.RR.brakeTorque = brake * brakePower * 0.3f;
        WheelColliders.LR.brakeTorque = brake * brakePower * 0.3f;

        if (handbrake)
        {
            clutch = 0;
            WheelColliders.RR.brakeTorque = brakePower * 1000f;
            WheelColliders.LR.brakeTorque = brakePower * 1000f;
        }
    }

    void ApplyTorque()
    {
        currentTorque = CalculateTorque();
        WheelColliders.LR.motorTorque = currentTorque * acceleration;
        WheelColliders.RR.motorTorque = currentTorque * acceleration;
    }


    /// <summary>
    /// Using the formula HP = Torque * RPM / 5252 and based on gearRatios and differential ratio, determine the generated torque on the wheels
    /// </summary>
    /// <returns>Torque value in N*m</returns>
    float CalculateTorque()
    {
        float torque = 0;
        if (RPM < idleRPM + 200 && acceleration == 0 && currentGear == 0)
        {
            gearState = GearState.Neutral;
        }
        if (gearState == GearState.Running && clutch > 0)
        {
            if (RPM > increaseGearRPM)
            {
                if (currentGear != 5)
                {
                    StartCoroutine(ChangeGear(1));
                }
            }
            else if (RPM < decreaseGearRPM)
            {
                if (currentGear != 0)
                {
                    StartCoroutine(ChangeGear(-1));
                }
            }
        }
        if (engineRunning > 0)
        {
            if (clutch < 0.1f)
            {
                RPM = Mathf.Lerp(RPM, Mathf.Max(idleRPM, redLine * acceleration) + Random.Range(-50, 50), Time.deltaTime);
            }
            else
            {
                wheelRPM = Mathf.Abs((WheelColliders.RR.rpm + WheelColliders.LR.rpm)) / 2 * gearRatios[currentGear] * diffRatio;
                RPM = Mathf.Lerp(RPM, Mathf.Max(idleRPM - 100, wheelRPM), Time.deltaTime);
                torque = HPtoRPMCurve.Evaluate(RPM / redLine) * engineTorque / RPM * gearRatios[currentGear] * 5252f * diffRatio * clutch;
            }
        }
        return torque;
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
    }


    /// <summary>
    /// Updates position of the texture on a wheel based on the collider's movement
    /// </summary>
    /// <param name="wc">Collider of the wheel</param>
    /// <param name="mr">Mesh texture of the wheel</param>
    void UpdateSingleWheel(WheelCollider wc, MeshRenderer mr)
    {
        Quaternion quat;
        Vector3 position;

        wc.GetWorldPose(out position, out quat);
        mr.transform.position = position;
        mr.transform.rotation = quat;
    }

    public float SpeedRatio()
    {
        var acc = Mathf.Clamp(Mathf.Abs(acceleration), 0.5f, 1f);
        return RPM * acc / redLine;
    }


    /// <summary>
    /// Based on the gearChange state, check RPM and decide whether to go up or down the gears
    /// </summary>
    /// <param name="gearChange"></param>
    /// <returns></returns>
    IEnumerator ChangeGear(int gearChange)
    {
        gearState = GearState.CheckChange;
        if (currentGear + gearChange >= 0)
        {
            if (gearChange > 0)
            {
                yield return new WaitForSeconds(0.7f);
                if (RPM < increaseGearRPM || currentGear > gearRatios.Length - 1)
                {
                    gearState = GearState.Running;
                    yield break;
                }
            }
            if (gearChange < 0)
            {
                yield return new WaitForSeconds(0.1f);
                if (RPM > decreaseGearRPM || currentGear <= 0)
                {
                    currentGear = 0;
                    gearState = GearState.Running;
                    yield break;
                }
            }
            gearState = GearState.Change;
            yield return new WaitForSeconds(0.5f);
            currentGear += gearChange;
        }
        if (gearState != GearState.Neutral)
        {
            gearState = GearState.Running;
        }
    }
}
