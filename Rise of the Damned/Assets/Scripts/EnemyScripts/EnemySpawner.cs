using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemies")]
    public GameObject[] enemies;
    public float[] enemyChance;

    // Start is called before the first frame update
    void Awake()
    {
        float rand = Random.value;
        int enemyIndex = -1;
        float sum = 0;
        for(int i = 0; i < enemies.Length; i++)
        {
            if(rand <= (sum + enemyChance[i]) / 100f)
            {
                enemyIndex = i;
                break;
            }
            sum += enemyChance[i];
        }
        if (enemyIndex >= 0 && enemies[enemyIndex] != null)
        {
            GameObject spawn = Instantiate(enemies[enemyIndex], transform.position, Quaternion.identity);
            EnemyController spawnControl = spawn.GetComponent<EnemyController>();
            spawnControl.damage *= PlayerController.roomNum + 1;
            spawnControl.health *= PlayerController.roomNum * .5f + 1;
            spawn.transform.SetParent(gameObject.transform);

            //if(transform.parent.parent.gameObject == NewRoomGovernor.spawn)
            //    spawn.GetComponent<EnemyController>().drops = new GameObject[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
       // Destroy(gameObject);
    }
}
