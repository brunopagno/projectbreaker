using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalMovement : MonoBehaviour
{
    public float speed;

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        gameObject.transform.position += Vector3.right * horizontalInput * speed * Time.deltaTime;
    }
}
