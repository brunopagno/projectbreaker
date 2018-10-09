using System;
using UnityEngine;

public class HeroControl : MonoBehaviour
{
    public float speed = 2;
    public float acceleration = 200;
    public float jump = 450;
    public Rigidbody body;

    private bool isJumping = false;

    private void FixedUpdate()
    {
        ResolveRun();
        ResolveJump();
    }

    private void ResolveRun()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        body.AddForce(Vector3.right * horizontalInput * acceleration);
        if (Mathf.Abs(body.velocity.x) > speed)
        {
            Vector3 max = body.velocity;
            max.x = max.normalized.x * speed;
            body.velocity = max;
        }
    }

    private void ResolveJump()
    {
        if (!isJumping && Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            body.velocity = new Vector3(body.velocity.x, 0, body.velocity.z);
            body.AddForce(Vector3.up * jump);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        isJumping = false;
    }
}
