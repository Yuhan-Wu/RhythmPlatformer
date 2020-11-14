using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Camera ActiveCamera;
    public float MoveVelocity = 10;
    public float JumpVelocity;

    private Rigidbody2D rigidbody;

    private bool isJumping = false;

    public void Initialize(float p_MoveVel)
    {
        rigidbody = GetComponent<Rigidbody2D>();
        ActiveCamera = Camera.main;
        ActiveCamera.transform.position = transform.position - new Vector3(0, 0, 0.5f);
        MoveVelocity = p_MoveVel;
    }

    private void Update()
    {
        Vector2 distance = Vector2.right * MoveVelocity * Time.deltaTime;
        transform.position += new Vector3(distance.x, 0, 0);
        ActiveCamera.transform.position = new Vector3(transform.position.x, 0, -2);

        if (!isJumping)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isJumping = true;
                rigidbody.AddForce(new Vector2(0, 7), ForceMode2D.Impulse);
            }
        }
        else
        {
            // TODO On a platform && Space is up
            if (Input.GetKeyUp(KeyCode.Space))
            {
                isJumping = false;
            }
        }
    }
}
