using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileTrampoline : Tile
{
    private float JumpEnhance = 0;
    private float PreviousJumpVel = 0;

    public override void Initialize(TileObstacle p_Info, float p_MapVel)
    {
        base.Initialize(p_Info, p_MapVel);
        JumpEnhance = p_Info.Number;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PreviousJumpVel = collision.gameObject.GetComponent<BallController>().JumpVelocity;
            collision.gameObject.GetComponent<BallController>().JumpVelocity = PreviousJumpVel + JumpEnhance;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<BallController>().JumpVelocity = PreviousJumpVel;
        }
    }
}
