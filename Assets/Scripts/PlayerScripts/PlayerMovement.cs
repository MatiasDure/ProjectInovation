using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float gravity = -9.81f; // Use a more realistic gravity value

    [SerializeField] private float jumpForce = 5f; // Assign a default jumpForce value

    private Vector3 velocity;
    private bool grounded;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (grounded && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Jumped");
            grounded = false;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Use Rigidbody to apply jump force
        }
    }

    private void FixedUpdate()
    {
        if (!grounded)
        {
            // Apply gravity
            rb.AddForce(Vector3.up * gravity * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsGroundCollision(collision))
        {
            grounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (IsGroundCollision(collision))
        {
            grounded = false;
        }
    }

    private bool IsGroundCollision(Collision collision)
    {
        // Customize this method to determine if the player is colliding with a "ground" object.
        // You could use tags, layers, or specific object names.
        // This example uses the object's tag "Ground".
        return collision.gameObject.CompareTag("Ground");
    }
}


