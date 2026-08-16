using UnityEngine;

public class MovementController : MonoBehaviour
{
    // 1. Az Állapotgép (State Machine) definíciója
    public enum RoverState { IDLE, MOVING, TURNING, ERROR }

    [Header("State")]
    public RoverState currentState = RoverState.IDLE;

    [Header("Kinematic Settings")]
    public float moveSpeed = 2.0f; // m/s
    public float turnSpeed = 90.0f; // fok/s
    public Transform[] wheels;
    private float wheelRadius = 0.5f;

    [Header("Safety Constraints")]
    public float maxDistance = 5.0f;
    public float maxAngle = 360.0f;
    public float watchdogTimeout = 2.0f;

    // Belsõ változók a célalapú mozgáshoz
    private float targetAmount = 0f;
    private float currentAmount = 0f;

    // Eredeti pozíció és forgatás megjegyzése
    private Vector3 startPosition;
    private Quaternion startRotation;

    // Watchdog idõzítõ
    private float lastCommunicationTime;

    void Start()
    {
        // Megjegyezzük, hol állt a rover a játék indításakor
        startPosition = transform.position;
        startRotation = transform.rotation;

        lastCommunicationTime = Time.time;
        Debug.Log("[Rover] Biztonsági Állapotgép inicializálva.");
    }

    void Update()
    {
        // 2. Watchdog ellenõrzés (ha mozog, de megszakadt a kapcsolat)
        if (Time.time - lastCommunicationTime > watchdogTimeout && currentState != RoverState.IDLE && currentState != RoverState.ERROR)
        {
            Debug.LogWarning("[Watchdog] Hálózati csend! E-STOP aktiválva.");
            StopMovement();
            currentState = RoverState.ERROR;
            return;
        }

        // 3. Állapotgép futtatása
        if (currentState == RoverState.MOVING)
        {
            HandleMovement();
        }
        else if (currentState == RoverState.TURNING)
        {
            HandleTurning();
        }
    }

    private void HandleMovement()
    {
        float step = moveSpeed * Time.deltaTime;
        float remaining = Mathf.Abs(targetAmount) - Mathf.Abs(currentAmount);

        // Ha túlfutnánk a célon, pont a célig megyünk
        if (remaining <= step)
        {
            step = remaining;
            currentState = RoverState.IDLE; // Cél elért, vissza IDLE-be
        }

        float direction = Mathf.Sign(targetAmount);
        float moveDistance = step * direction;

        transform.Translate(0, 0, moveDistance, Space.Self);
        currentAmount += moveDistance;

        AnimateWheels(moveDistance);
    }

    private void HandleTurning()
    {
        float step = turnSpeed * Time.deltaTime;
        float remaining = Mathf.Abs(targetAmount) - Mathf.Abs(currentAmount);

        if (remaining <= step)
        {
            step = remaining;
            currentState = RoverState.IDLE;
        }

        float direction = Mathf.Sign(targetAmount);
        float turnAngle = step * direction;

        transform.Rotate(0, turnAngle, 0);
        currentAmount += turnAngle;
    }

    private void AnimateWheels(float moveDistance)
    {
        float wheelCircumference = 2 * Mathf.PI * wheelRadius;
        float rotationDegrees = (moveDistance / wheelCircumference) * 360f;
        foreach (Transform wheel in wheels)
        {
            if (wheel != null) wheel.Rotate(Vector3.up, rotationDegrees, Space.Self);
        }
    }

    // Ezt hívja a TCP szerver, hogy jelezze: él a kapcsolat
    public void PingWatchdog()
    {
        lastCommunicationTime = Time.time;
    }

    // --- API VÉGPONTOK A SZERVERNEK ---

    public bool ExecuteMove(float distance)
    {
        PingWatchdog();
        if (currentState != RoverState.IDLE && currentState != RoverState.ERROR) return false; // ERR_BUSY
        if (Mathf.Abs(distance) > maxDistance) return false; // ERR_OUT_OF_BOUNDS

        targetAmount = distance;
        currentAmount = 0f;
        currentState = RoverState.MOVING;
        return true;
    }

    public bool ExecuteTurn(float angle)
    {
        PingWatchdog();
        if (currentState != RoverState.IDLE && currentState != RoverState.ERROR) return false;
        if (Mathf.Abs(angle) > maxAngle) return false;

        targetAmount = angle;
        currentAmount = 0f;
        currentState = RoverState.TURNING;
        return true;
    }

    public void StopMovement()
    {
        PingWatchdog();
        currentState = RoverState.IDLE;
        targetAmount = 0f;
        currentAmount = 0f;
    }

    public void ResetPosition()
    {
        PingWatchdog();

        // A fix (0,0,0) helyett a megjegyzett kezdõpontra ugrunk!
        transform.position = startPosition;
        transform.rotation = startRotation;

        currentState = RoverState.IDLE;
        targetAmount = 0f;
        currentAmount = 0f;
    }
}