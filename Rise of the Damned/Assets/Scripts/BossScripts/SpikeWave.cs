using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeWave : MonoBehaviour
{
    private bool isActive = false;
    //private float time;
    public float timePerSpike;
    public float multiplier;
    private List<float> waves = new List<float>();
    private bool isGoingLeft = true;
    private int numSpikes;

    // Start is called before the first frame update
    void Start()
    {
        // time = -3 * timePerSpike;
        numSpikes = transform.childCount;
    }

    // Update is called once per frame
    void Update()
    {
        if(isActive)
        {
           
            for(int i = 0; i < waves.Count; i++)
                waves[i] += Time.deltaTime * (isGoingLeft ? 1: -1);

            foreach (Transform spike in transform)
            {
                int spikeIndex = int.Parse(spike.name.Remove(0, 5)) - 1;
                float maxY = 0;
                foreach (float time in waves)
                {
                    float spikeY = GetSpikeY(spikeIndex, time);
                    if (spikeY > maxY)
                        maxY = spikeY;
                }
                spike.localPosition = new Vector3(0, maxY, 0);
            }

            for (int i = waves.Count - 1; i >= 0; i--)
            {
                if (waves[i] < -3 * timePerSpike || (waves[i] / timePerSpike >= numSpikes * timePerSpike && (numSpikes - (waves[i] / timePerSpike)) * multiplier <= -3))
                {
                    //isActive = false;
                    //Debug.Log("Time: " + waves[i] + "\tSpikes: " + transform.childCount + "\tRatio: " + (waves[i] / timePerSpike));
                    waves.Remove(waves[i]);
                }
            }

            if(waves.Count == 0)
                isActive = false;
        }
    }

    public void Run()
    {
        isActive = true;
        waves.Add(isGoingLeft ? -3 * timePerSpike : (3 + numSpikes) * timePerSpike);
    }

    public void ChangeDir(bool dir)
    {
        isGoingLeft = dir;
        
    }

    private float GetSpikeY(int index, float time)
    {
        return Mathf.Clamp(3f - (Mathf.Abs(index - (time / timePerSpike)) * multiplier), 0, 3);
    }
}
