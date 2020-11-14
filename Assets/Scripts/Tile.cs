using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    protected float TileScale = 1;
    protected float MapVel = 0;

    public virtual void Initialize(TileObstacle p_Info, float p_MapVel)
    {
        MapVel = p_MapVel;
    }

    public virtual void Scale(float p_Scale) {
        TileScale = p_Scale;
        transform.localScale = new Vector3(p_Scale, 1, 1);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

        }
    }
}
