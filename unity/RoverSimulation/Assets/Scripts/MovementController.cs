using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    [Header("Paraméterek")]
    [Tooltip("A rover mozgási sebessége")]
    public float speed = 5.0f; // Az Inspector-ból állítható sebesség

    private Rigidbody rb;
    private Vector3 startPosition;
    private Vector2 inputDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position; // Kezdõpozíció mentése a resethez
    }

    void Update()
    {
        // 1. Billentyûzet olvasása (WASD vagy Nyilak) az Update-ben
        inputDirection.x = Input.GetAxisRaw("Horizontal");
        inputDirection.y = Input.GetAxisRaw("Vertical");

        // 2. Reset funkció ('R' billentyû)
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPosition();
        }
    }

    void FixedUpdate()
    {
        // 3. Fizikai mozgatás a FixedUpdate-ben a stabilitásért
        Vector3 movement = new Vector3(inputDirection.x, 0.0f, inputDirection.y).normalized;

        // A MovePosition stabil ütközéseket garantál, nem esik át a pályán
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    // Publikus Reset funkció (hogy a tesztekbõl is hívható legyen)
    public void ResetPosition()
    {
        transform.position = startPosition;
        rb.linearVelocity = Vector3.zero;  // Lendület nullázása
        rb.angularVelocity = Vector3.zero; // Forgás nullázása
        Debug.Log("Reset: Pozíció visszaállítva a kiindulópontra!");
    }

    // Egyszerû ütközésjelzés a Console-ra
    void OnCollisionEnter(Collision collision)
    {
        // A talajjal (Plane) való folyamatos ütközést nem spameljük tele
        if (collision.gameObject.name != "Floor")
        {
            Debug.Log($"Ütközés történt ezzel: {collision.gameObject.name}");
        }
    }
}