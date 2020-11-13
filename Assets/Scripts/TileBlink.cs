using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBlink : Tile
{
    public int Delay = 1;

    private int Index = 0;
    private float MapVel = 0;
    private float TotalTime = 0;
    private bool State = true;

    public override void Initialize (int p_Index, float p_MapVel)
    {
        
        Index = p_Index / Delay;
        MapVel = p_MapVel;

        if (Index % 2 != 0) State = false;
    }

    private void Update()
    {
        TotalTime += Time.deltaTime;
        if(TotalTime >= MapVel)
        {
            TotalTime = 0;
            State = !State;
            GetComponent<SpriteRenderer>().enabled = State;
            GetComponent<Collider2D>().enabled = State;
        }
    }
}
