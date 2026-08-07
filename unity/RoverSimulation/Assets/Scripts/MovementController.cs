using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    public float speed = 5.0f;
    private Rigidbody rb;
    private Vector3 startPosition;
    private Vector2 currentInput;

    // Biztonsági timeout változók (M03)
    private float lastCommandTime;
    private const float COMMAND_TIMEOUT = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position; // Kezdõpozíció mentése a resethez (M02)
        Debug.Log($"[Init] Rover kontroller elindult. Sebesség: {speed}");
    }

    void Update()
    {
        // Ha letelt a timeout új parancs nélkül, megállítjuk a rovert (M03)
        if (Time.time - lastCommandTime > COMMAND_TIMEOUT)
        {
            currentInput = Vector2.zero;
        }

        // Manuális reset gomb ('R') meghagyása teszteléshez (M02)
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPosition();
        }
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(currentInput.x, 0.0f, currentInput.y).normalized;
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    // Ezt a függvényt fogja hívni a hálózati TCP szerver (M03)
    public void ExecuteMove(float x, float y)
    {
        currentInput = new Vector2(x, y);
        lastCommandTime = Time.time; // Idõzítõ frissítése
    }

    public void StopMovement()
    {
        currentInput = Vector2.zero;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    // Publikus Reset funkció a tesztekhez és manuális visszaállításhoz (M02)
    public void ResetPosition()
    {
        transform.position = startPosition;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        currentInput = Vector2.zero; // A bemenetet is nullázzuk a biztonság kedvéért
        Debug.Log("Reset: Pozíció visszaállítva a kiindulópontra!");
    }

    // Egyszerû ütközésjelzés a Console-ra (M02)
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name != "Floor")
        {
            Debug.Log($"Ütközés történt ezzel: {collision.gameObject.name}");
        }
    }
}