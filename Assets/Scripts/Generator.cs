using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public List<SpectralFluxInfo> Infos;
    public AudioClip Clip;
    public GameObject Map;
    public GameObject Ball;
    public GameObject Line;
    public List<GameObject> Obstacles;

    public float MapVelocity = 0.1f;
    public int ComplexJumpRestTime = 5;

    private List<float> Mark = new List<float>();
    private List<TileObstacle> TileObstacles = new List<TileObstacle>();

    struct TileObstacle
    {
        public char Type;
        public float StartTime;
        public float EndTime;
        public float Number;
    }

    private void MarkObstacle()
    {
        int markIndex = 0;
        for (int i = 0; i < Infos.Count; i++)
        {
            float time = Infos[i].time;
            int temp = (int)(time * (1 / MapVelocity));
            float roundTime = (float)((temp * 1.0) * MapVelocity);
            if(markIndex!=0 && Mark[markIndex] != roundTime)
            {
                Mark.Add(roundTime);
                markIndex++;
            }
        }
    }

    private void AnalyzeMark()
    {
        int markIndex = 0;
        while (markIndex != Mark.Count)
        {
            int tempIndex = markIndex;
            int consecutive = 1;
            tempIndex++;
            while (tempIndex < Mark.Count)
            {
                if(Mark[tempIndex].Equals(Mark[tempIndex] + 0.1f))
                {
                    consecutive++;
                    tempIndex++;
                }
                else break;
            }
            if(consecutive > 1)
            {
                // TODO > 3 in a line = trampoline
                markIndex = tempIndex;
                continue;
            }

       
            tempIndex = markIndex;
            consecutive = 1;
            float curDelta = 0;
            if(tempIndex + 1 < Mark.Count)
            {
                curDelta = Mark[tempIndex + 1] - Mark[tempIndex];
                tempIndex++;
                while(tempIndex + 1 < Mark.Count)
                {
                    float tempDelta = Mark[tempIndex + 1] - Mark[tempIndex];
                    if (tempDelta.Equals(curDelta))
                    {
                        consecutive++;
                        tempIndex++;
                    }
                    else break;
                }
                // TODO should also put restriction on delta
                if(consecutive > 2 && consecutive < 4) 
                {
                    // TODO 2 or 3 same pattern = blinking line
                    markIndex = tempIndex;
                    continue;
                }
                else if(consecutive > 2)
                {
                    // TODO 4 or more same pattern = wall jump
                    markIndex = tempIndex;
                    continue;
                }
            }

            
            // TODO Spike
            
        }
    }

    private void InfoFilter()
    {
        MarkObstacle();
        AnalyzeMark();
        int infoIndex = 1;
        while(infoIndex < TileObstacles.Count)
        {
            if(TileObstacles[infoIndex].StartTime - TileObstacles[infoIndex - 1].EndTime < ComplexJumpRestTime * MapVelocity)
            {
                TileObstacles.RemoveAt(infoIndex);
            }
            else
            {
                infoIndex++;
            }
        }
    }

    public void Generate()
    {
        InfoFilter();
        float lengh = Clip.length; // In seconds
        float iteration = lengh / MapVelocity;
        int infoIndex = 0;
        Vector3 location = new Vector3(0, 0, 0);

        GameObject cloneLine = Instantiate(Line);
        cloneLine.transform.parent = Map.transform;
        cloneLine.transform.localPosition = new Vector3(-1, 0, 0);

        for (var i = 0; i < iteration; i++)
        {
            if (TileObstacles[infoIndex].StartTime.Equals(i * MapVelocity))
            {
                // TODO Spawn obstacle
            }
            else
            {
                cloneLine = Instantiate(Obstacles[0]);
                cloneLine.transform.parent = Map.transform;
                cloneLine.transform.localPosition = location;
                location.x += 1;
            }
        }

        GameObject ball = Instantiate(Ball);
        ball.transform.position = new Vector3(-1, 1, 0);
        float moveVelocity = 1 / MapVelocity;
        ball.GetComponent<BallController>().Initialize(1 / MapVelocity);
    }
}
