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

    // Start is called before the first frame update
    void Start()
    {
       // time = -3 * timePerSpike;
    }

    // Update is called once per frame
    void Update()
    {
        if(isActive)
        {
           
            for(int i = 0; i < waves.Count; i++)
                waves[i] += Time.deltaTime;

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
                if (waves[i] / timePerSpike >= (transform.childCount) * timePerSpike && (transform.childCount - (waves[i] / timePerSpike)) * multiplier <= -3)
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
        waves.Add(-3 * timePerSpike);
        //Debug.Log("Running Spike Wave");
    }

    private float GetSpikeY(int index, float time)
    {
        return Mathf.Clamp(3f - (Mathf.Abs(index - (time / timePerSpike)) * multiplier), 0, 3);
    }
}
