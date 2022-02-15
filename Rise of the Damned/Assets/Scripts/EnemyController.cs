using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int diff = 80;
    public int shootAngle = 270;
    public GameObject FireBall;
    public int damage = 10;
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
        if (gameObject.name == "demon skull")    
    {
            GameObject shoot = Instantiate(FireBall, transform.position, Quaternion.identity);
            int shootAngle = 90;
            int diff = 80;
            shootAngle = 270;
            diff = -45;

            shoot.GetComponent<Rigidbody2D>().rotation = shootAngle;
            shoot.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(shootAngle * Mathf.Deg2Rad) * 10, Mathf.Sin(shootAngle * Mathf.Deg2Rad) * 10);
    }
}
}



