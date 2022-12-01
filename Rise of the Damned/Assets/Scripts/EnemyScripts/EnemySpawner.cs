using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemies")]
    public GameObject[] enemies;

    // Start is called before the first frame update
    void Start()
    {
        GameObject spawn = Instantiate(enemies[Random.Range(0, enemies.Length)], transform.position, Quaternion.identity);
        EnemyController spawnControl = spawn.GetComponent<EnemyController>();
        spawnControl.damage *= PlayerController.roomNum + 1;
        spawnControl.health *= PlayerController.roomNum * .5f + 1;
        spawn.transform.SetParent(gameObject.transform);
    }

    // Update is called once per frame
    void Update()
    {
       // Destroy(gameObject);
    }
}
