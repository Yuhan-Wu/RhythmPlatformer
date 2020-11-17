using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{

    public Camera ActiveCamera;
    public float MoveVelocity = 10;
    public float JumpVelocity = 7;
    public GameObject Succeed;
    public GameObject Fail;
    public float DefaultGravity = 4;
    private Rigidbody2D rigidbody;

    private bool isJumping = false;
    private bool isOver = true;
    private bool isOnGround = true;

    public void Initialize(float p_MoveVel)
    {
        rigidbody = GetComponent<Rigidbody2D>();
        ActiveCamera = Camera.main;
        ActiveCamera.transform.position = transform.position - new Vector3(0, 0, 0.5f);
        MoveVelocity = p_MoveVel;
        isOver = false;
        rigidbody.gravityScale = DefaultGravity;
    }

    private void Update()
    {
        if (!isOver)
        {
            Vector2 distance = Vector2.right * MoveVelocity * Time.deltaTime;
            transform.position += new Vector3(distance.x, 0, 0);
            ActiveCamera.transform.position = new Vector3(transform.position.x, 0, -2);

            if(transform.position.y < -10)
            {
                ShowFail();
                return;
            }

            if (!isJumping && isOnGround)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    isJumping = true;
                    isOnGround = false;
                    rigidbody.AddForce(new Vector2(0, JumpVelocity), ForceMode2D.Impulse);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            ShowFail();
        }
        else if(collision.gameObject.CompareTag("Succeed"))
        {
            Succeed.SetActive(true);
            isOver = true;
        }
        else
        {
            isOnGround = true;
        }
    }

    private void ShowFail()
    {
        Fail.SetActive(true);
        isOver = true;
    }
}
