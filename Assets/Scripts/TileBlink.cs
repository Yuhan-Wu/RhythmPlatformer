﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBlink : Tile
{
    public float Delay = 0.01f;

    private float Start = 0;
    private float End = 0;
    private float TotalTime = 0;
    private bool Enter = false;
    private bool Over = false;
    private bool Active = false;

    public override void Initialize (TileObstacle p_Info, float p_MapVel)
    {
        base.Initialize(p_Info, p_MapVel);
        Start = p_Info.StartTime - Delay;
        End = p_Info.EndTime + Delay;
        Active = (p_Info.Number % 2 == 1); 
    }

    private void Update()
    {
        if (Active && !Over)
        {
            TotalTime += Time.deltaTime;
            if (!Enter && TotalTime >= Start)
            {
                GetComponent<SpriteRenderer>().enabled = true;
                GetComponent<Collider2D>().enabled = true;
                Enter = true;
                return;
            }
            if (TotalTime >= End)
            {
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<Collider2D>().enabled = false;
                Over = true;
                return;
            }
        }
        
    }
}
