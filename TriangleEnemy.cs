using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleEnemy : Enemy
{
    public float fireDelay;
    public float bulletSpeed;

    public GameObject projectilePrefab;
    public Transform aimPivot;
    public Transform aimIndicator;

    private float fireCooldown;

    private void Start()
    {
        fireCooldown = fireDelay;
    }

    private void Update()
    {
        aimPivot.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(aimPivot.position.y - player.position.y, aimPivot.position.x - player.position.x) * Mathf.Rad2Deg + 90f);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, aimIndicator.up);
        if (hit.transform == player)
        {
            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0)
            {
                fireCooldown = fireDelay;

                Instantiate(projectilePrefab, aimIndicator.position, Quaternion.Euler(0, 0, Random.Range(0, 360))).GetComponent<Rigidbody2D>().AddForce(aimIndicator.up * bulletSpeed, ForceMode2D.Impulse);
            }
        }
    }

    private void FixedUpdate()
    {
        Move(transform.position + (5f * (transform.position - player.position).normalized));
    }
}
