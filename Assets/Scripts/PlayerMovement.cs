using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Hard-coded WASD input
        movement.x = 0f;
        movement.y = 0f;

        if (Input.GetKey(KeyCode.W)) movement.y += 1f;
        if (Input.GetKey(KeyCode.S)) movement.y -= 1f;
        if (Input.GetKey(KeyCode.A)) movement.x -= 1f;
        if (Input.GetKey(KeyCode.D)) movement.x += 1f;

        movement = movement.normalized * moveSpeed;
    }

    private void FixedUpdate()
    {
        // Apply movement
        rb.velocity = movement;
    }
}
