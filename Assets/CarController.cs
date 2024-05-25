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

[System.Serializable]
public class WheelParticles
{
    public ParticleSystem LF, LR, RF, RR;
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
    public WheelParticles WheelParticles;
    public GameObject smokePrefab;

    // Inputs
    public float acceleration, brake, steering;

    // Car properties
    public float engineTorque, brakePower, RPM, redLine, idleRPM, diffRatio, increaseGearRPM, decreaseGearRPM;
    public float[] gearRatios;
    private float speedClamp, currentTorque, clutch, wheelRPM;
    public int engineRunning, currentGear;
    private GearState gearState;

    // Vectors and angles
    private float slipAngle, speed;
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
        rpmNeedle.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(minNeedleRotation, maxNeedleRotation, RPM / (redLine * 1.1f)));
        rpmText.text = "" + (int)RPM;
        gearText.text = (gearState == GearState.Neutral) ? "N" : (currentGear + 1).ToString();
        speed = WheelColliders.RR.rpm * WheelColliders.RR.radius * 2f * Mathf.PI / 10f;
        speedClamp = Mathf.Lerp(speedClamp, speed, Time.deltaTime);
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

        float movingDirection = Vector3.Dot(transform.forward, rb.velocity);

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
                clutch = Input.GetKey(KeyCode.LeftShift) ? 0 : Mathf.Lerp(clutch, 1, Time.deltaTime);
            }
        }
        else
        {
            clutch = 0;
        }

        if (Mathf.Abs(acceleration) > 0 && engineRunning == 0)
        {
            StartCoroutine(GetComponent<EngineAudio>().StartEngine());
            gearState = GearState.Running;
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
                StartCoroutine(ChangeGear(1));
            }
            else if (RPM < decreaseGearRPM)
            {
                StartCoroutine(ChangeGear(-1));
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
                RPM = Mathf.Lerp(RPM, Mathf.Max(idleRPM - 100, wheelRPM), Time.deltaTime * 3f);
                torque = HPtoRPMCurve.Evaluate(RPM / redLine) * engineTorque / RPM * gearRatios[currentGear] * diffRatio * 5252f * clutch;
            }
        }
        return torque;
    }

    void ApplyTorque()
    {
        currentTorque = CalculateTorque();
        WheelColliders.LR.motorTorque = currentTorque * acceleration;
        WheelColliders.RR.motorTorque = currentTorque * acceleration;
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

    public float SpeedRatio()
    {
        var acc = Mathf.Clamp(Mathf.Abs(acceleration), 0.5f, 1f);
        return RPM * acc / redLine;
    }

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
