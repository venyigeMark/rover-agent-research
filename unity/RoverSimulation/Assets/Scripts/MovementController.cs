using UnityEngine;

public class MovementController : MonoBehaviour
{
    [Header("Kinematic Settings")]
    public float moveSpeed = 2.0f; // m/s
    public float turnSpeed = 90.0f; // fok/s

    [Header("Wheels for Animation")]
    public Transform[] wheels; // Húzd ide a 4 kereket az Inspectorban!

    private float currentLinearInput = 0f;
    private float currentAngularInput = 0f;
    private float wheelRadius = 0.5f; // A kerekek sugara a sebességszámításhoz

    private float lastCommandTime;
    private const float COMMAND_TIMEOUT = 0.5f;

    void Start()
    {
        Debug.Log("[Rover] Kinematikus modell inicializálva.");
    }

    void Update()
    {
        // Timeout védelem
        if (Time.time - lastCommandTime > COMMAND_TIMEOUT)
        {
            currentLinearInput = 0f;
            currentAngularInput = 0f;
        }

        // 1. Relatív szögû fordulás (Y tengely körül)
        if (currentAngularInput != 0f)
        {
            float turnAmount = currentAngularInput * turnSpeed * Time.deltaTime;
            transform.Rotate(0, turnAmount, 0);
        }

        // 2. Elõre/Hátra haladás (Lokális Z tengely mentén)
        if (currentLinearInput != 0f)
        {
            float moveDistance = currentLinearInput * moveSpeed * Time.deltaTime;
            transform.Translate(0, 0, moveDistance, Space.Self);

            // 3. Kerékanimáció (Követi a mozgást)
            // A megtett út = szögelfordulás (radián) * sugár
            // Szög (fok) = (Út / Kerület) * 360
            float wheelCircumference = 2 * Mathf.PI * wheelRadius;
            float rotationDegrees = (moveDistance / wheelCircumference) * 360f;

            foreach (Transform wheel in wheels)
            {
                if (wheel != null)
                {
                    // Helyi X tengely körüli forgatás a henger orientációja miatt
                    wheel.Rotate(Vector3.up, rotationDegrees, Space.Self);
                }
            }
        }
    }

    // A TCP szerver hívja ezt a függvényt.
    // x = fordulás (jobbra/balra), y = gáz (elõre/hátra)
    public void ExecuteMove(float angularInput, float linearInput)
    {
        currentAngularInput = angularInput;
        currentLinearInput = linearInput;
        lastCommandTime = Time.time;
    }

    public void StopMovement()
    {
        currentLinearInput = 0f;
        currentAngularInput = 0f;
    }

    public void ResetPosition()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        currentLinearInput = 0f;
        currentAngularInput = 0f;
    }
}