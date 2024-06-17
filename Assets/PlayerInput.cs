using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CarController))]
public class PlayerInput : MonoBehaviour
{
    public float throttleInput, steeringInput, clutchInput, handbrakeInput, dampenSpeed;
    public float throttleDampen, steeringDampen;
    public CarInput input;
    private CarController carController;

    void Awake()
    {
        input = new CarInput();
        carController = GetComponent<CarController>();
    }

    private void OnEnable()
    {
        input.Enable();

        input.Car.Throttle.performed += ApplyThrottle;
        input.Car.Throttle.canceled += ReleaseThrottle;

        input.Car.Steering.performed += ApplySteering;
        input.Car.Steering.canceled += ReleaseSteering;

        input.Car.Clutch.performed += ApplyClutch;
        input.Car.Clutch.canceled += ReleaseClutch;

        input.Car.Handbrake.performed += ApplyHandbrake;
        input.Car.Handbrake.canceled += ReleaseHandbrake;
    }

    private void Update()
    {
        throttleDampen = DampenInput(throttleInput, throttleDampen);
        steeringDampen = DampenInput(steeringInput, steeringDampen);
        carController.SetInput(throttleDampen, steeringDampen, clutchInput, handbrakeInput);
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private float DampenInput(float input, float output)
    {
        return Mathf.Lerp(output, input, Time.deltaTime * dampenSpeed);
    }

    private void ApplyThrottle(InputAction.CallbackContext value)
    {
        throttleInput = value.ReadValue<float>();
    }

    private void ReleaseThrottle(InputAction.CallbackContext value)
    {
        throttleInput = 0;
    }

    private void ApplySteering(InputAction.CallbackContext value)
    {
        steeringInput = value.ReadValue<float>();
    }

    private void ReleaseSteering(InputAction.CallbackContext value)
    {
        steeringInput = 0;
    }

    private void ApplyClutch(InputAction.CallbackContext value)
    {
        clutchInput = value.ReadValue<float>();
    }

    private void ReleaseClutch(InputAction.CallbackContext value)
    {
        clutchInput = 0;
    }

    private void ApplyHandbrake(InputAction.CallbackContext value)
    {
        handbrakeInput = value.ReadValue<float>();
    }

    private void ReleaseHandbrake(InputAction.CallbackContext value)
    {
        handbrakeInput = 0;
    }
}
