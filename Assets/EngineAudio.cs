using System.Collections;
using UnityEngine;

public class EngineAudio : MonoBehaviour
{
    // Audio files
    public AudioSource runningSound, idleSound, startSound, reverseSound;

    // Audio parameters
    public float runningMaxVol, runningMaxPitch,
        reverseMaxVol, reverseMaxPitch,
        idleMaxVol,
        limiterSound = 1f, limiterFreq = 3f, limiterEngage = 0.8f, revLimiter;

    // Car properties
    public bool isRunning = false;
    private float speedRatio;

    private CarController carController;

    // Start is called before the first frame update
    void Start()
    {
        carController = GetComponent<CarController>();
        idleSound.volume = 0;
        runningSound.volume = 0;
        reverseSound.volume = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float speedSign = 0;
        if (carController)
        {
            speedSign = Mathf.Sign(carController.SpeedRatio());
            speedRatio = Mathf.Abs(carController.SpeedRatio());
        }

        if (speedRatio > limiterEngage)
        {
            revLimiter = (Mathf.Sin(Time.time * limiterFreq) + 1f) * limiterSound * (speedRatio - limiterEngage);
        }


        if (isRunning)
        {
            if (speedRatio == 0)
            {
                idleSound.volume = Mathf.Lerp(0.1f, idleMaxVol, speedRatio);
            }
            if (speedSign > 0)
            {
                reverseSound.volume = 0;
                runningSound.volume = Mathf.Lerp(0.3f, runningMaxVol, speedRatio);
                runningSound.pitch = Mathf.Lerp(runningSound.pitch, Mathf.Lerp(0.3f, runningMaxPitch, speedRatio) + revLimiter, Time.deltaTime);
            }
            else
            {
                runningSound.volume = 0;
                reverseSound.volume = Mathf.Lerp(0f, reverseMaxVol, speedRatio);
                reverseSound.pitch = Mathf.Lerp(runningSound.pitch, Mathf.Lerp(0.2f, reverseMaxPitch, speedRatio) + revLimiter, Time.deltaTime);
            }
        }
        else
        {
            idleSound.volume = 0;
            runningSound.volume = 0;
            reverseSound.volume = 0;
        }
    }
    public IEnumerator StartEngine()
    {
        startSound.Play();
        carController.engineRunning = 1;
        yield return new WaitForSeconds(0.6f);
        isRunning = true;
        yield return new WaitForSeconds(0.4f);
        carController.engineRunning = 2;
    }
}
