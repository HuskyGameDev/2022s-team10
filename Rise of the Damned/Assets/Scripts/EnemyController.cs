using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float health = 100;
    
    [SerializeField]
    private float speed;

    [SerializeField]
    private Vector2[] positions;

    private int index = 0;

    // Update is called once per frame

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, positions[index], Time.deltaTime * speed);

        if (Mathf.Abs(transform.position.x - positions[index].x) < .1)
        {
            if(index == positions.Length-1)
            {
                index = 0;
            }
            else
            {
                index++;
            }
        }

        if (health < 0)
            Destroy(gameObject);
    }
}


