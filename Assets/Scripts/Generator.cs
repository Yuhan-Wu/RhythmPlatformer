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
    public int NormalJumpRestTime = 3;

    private void InfoFilter()
    {
        int infoIndex = 1;
        bool lastWasCons = false;
        while(infoIndex < Infos.Count)
        {
            float timeDelta = Infos[infoIndex].time - Infos[infoIndex - 1].time;
            if(timeDelta <= MapVelocity)
            {
                lastWasCons = true;
                infoIndex++;
            }
            else if((lastWasCons && timeDelta < MapVelocity * ComplexJumpRestTime) || timeDelta < MapVelocity * NormalJumpRestTime)
            {
                Infos.RemoveAt(infoIndex);
                continue;
            }
            else
            {
                lastWasCons = false;
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
            bool spawnLine = true;
            while(infoIndex < Infos.Count && i * MapVelocity < Infos[infoIndex].time && (i + 1) * MapVelocity > Infos[infoIndex].time)
            {
                // TODO Spawn in obstacle
                cloneLine = Instantiate(Obstacles[0]);
                cloneLine.transform.parent = Map.transform;
                cloneLine.transform.localPosition = location;

                infoIndex++;
                spawnLine = false;

                location.x += 1;
            }

            if (spawnLine)
            {
                cloneLine = Instantiate(Line);
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
