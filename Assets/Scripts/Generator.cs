using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TileObstacle
{
    public char Type;
    public float StartTime;
    public float EndTime;
    public float Number;

    public TileObstacle(char p_Type, float p_StartTime, float p_EndTime, float p_Number = 1)
    {
        Type = p_Type;
        StartTime = p_StartTime;
        EndTime = p_EndTime;
        Number = p_Number;
    }

    public TileObstacle(TileObstacle p_Info)
    {
        Type = p_Info.Type;
        StartTime = p_Info.StartTime;
        EndTime = p_Info.EndTime;
        Number = p_Info.Number;
    }
}

public class Generator : MonoBehaviour
{
    public List<SpectralFluxInfo> Infos;
    public AudioClip Clip;
    public GameObject Map;
    public GameObject Ball;
    public GameObject Line;
    public List<TileObstacleInstance> Obstacles = new List<TileObstacleInstance>();

    public float MapVelocity = 0.1f;
    public int ComplexJumpRestTime = 5;
    public float ComplexJumpReactionTime = 0;

    private List<float> Mark = new List<float>();
    private List<TileObstacle> TileObstacles = new List<TileObstacle>();

    [System.Serializable]
    public struct TileObstacleInstance
    {
        public char Type;
        public GameObject Tile;
    }

    private void MarkObstacle()
    {
        int markIndex = 0;
        for (int i = 0; i < Infos.Count; i++)
        {
            float time = Infos[i].time;
            int temp = (int)(time * (1 / MapVelocity));
            float roundTime = (float)((temp * 1.0) * MapVelocity);
            if(markIndex!=0 && Mark[markIndex - 1] != roundTime)
            {
                Mark.Add(roundTime);
                markIndex++;
            }
            else if(markIndex == 0)
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
                if(Mathf.Abs(Mark[tempIndex] - Mark[tempIndex]) < 0.2f && Mathf.Abs(Mark[tempIndex] - Mark[tempIndex]) >= 0.1f)
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
                    if (Mathf.Abs(tempDelta - curDelta) < 0.1f)
                    {
                        consecutive++;
                        tempIndex++;
                    }
                    else break;
                }
                // TODO should also put restriction on delta
                if(consecutive >= 2 && consecutive < 4 && curDelta > ComplexJumpReactionTime) 
                {
                    // TODO 2 or 3 same pattern = blinking line
                    TileObstacle blink = new TileObstacle('B', Mark[markIndex], Mark[tempIndex - 1], consecutive);
                    TileObstacles.Add(blink);
                    markIndex = tempIndex;
                    continue;
                }
                else if(consecutive >= 4)
                {
                    // TODO 4 or more same pattern = wall jump
                    markIndex = tempIndex;
                    continue;
                }
            }


            TileObstacle spike = new TileObstacle('S', Mark[markIndex], Mark[markIndex] + MapVelocity);
            TileObstacles.Add(spike);
            markIndex++;
            
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
            if (infoIndex < TileObstacles.Count && Mathf.Abs(TileObstacles[infoIndex].StartTime - i * MapVelocity) < 0.1f)
            {
                int perLength = (int)Mathf.Round((TileObstacles[infoIndex].EndTime - TileObstacles[infoIndex].StartTime) / MapVelocity
                    / TileObstacles[infoIndex].Number);
                location.x += (perLength + 1) / 2.0f - 1;
                GameObject ObstacleIns = Obstacles.Find(x => x.Type == TileObstacles[infoIndex].Type).Tile;
                for (int j = 0; j < TileObstacles[infoIndex].Number; j++)
                {
                    GameObject obstacle = Instantiate(ObstacleIns);
                    obstacle.transform.parent = Map.transform;
                    obstacle.transform.localPosition = location;
                    obstacle.GetComponent<Tile>().Scale(perLength);
                    TileObstacle info = new TileObstacle(TileObstacles[infoIndex]);
                    info.StartTime += j * perLength * MapVelocity;
                    info.EndTime = info.StartTime + perLength * MapVelocity;
                    info.Number = j;
                    obstacle.GetComponent<Tile>().Initialize(info, MapVelocity);

                    if(j == 0)
                    {
                        i += perLength - 1;
                    }
                    else
                    {
                        i += perLength;
                    }

                    if(j!= TileObstacles[infoIndex].Number - 1)
                        location.x += perLength;
                    else
                        location.x += (perLength + 1) / 2.0f;
                }
                infoIndex++;
            }
            else
            {
                cloneLine = Instantiate(Line);
                cloneLine.transform.parent = Map.transform;
                cloneLine.transform.localPosition = location;
                location.x += 1;
            }
        }

        GameObject ball = Instantiate(Ball);
        ball.transform.position = new Vector3(-1, 0.6f, 0);
        float moveVelocity = 1 / MapVelocity;
        ball.GetComponent<BallController>().Initialize(moveVelocity);
    }
}
