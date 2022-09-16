using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyController : MonoBehaviour
{
    [Header("Projectiles")]
    [SerializeField]
    public GameObject projectile;
    public float projDmg;
    [SerializeField]
    private float cooldown;
    private float cooldownTime;
    public LayerMask groundLayer;

    private void Start()
    {
        cooldownTime = cooldown / 2;
    }

    void Update()
    {
        if (cooldownTime >= cooldown)
        {
            Vector3 rayDir = PlayerController.player.transform.position - transform.position;
            if (!Physics2D.Raycast(transform.position, rayDir, Vector2.Distance(PlayerController.player.transform.position, transform.position), groundLayer))
            {
                GameObject shoot = Instantiate(projectile, transform.position, Quaternion.identity);
                shoot.GetComponent<EnemyProjController>().damage = projDmg;
            }
            //float shootAngle = Vector2.Angle(transform.position, PlayerController.player.transform.position);

            //shoot.GetComponent<Rigidbody2D>().rotation = shootAngle;
            //shoot.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(shootAngle * Mathf.Deg2Rad) * 10, Mathf.Sin(shootAngle * Mathf.Deg2Rad) * 10);
            cooldownTime = 0;
        }
        cooldownTime += Time.deltaTime;
    }
}
