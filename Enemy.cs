using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed;
    public int health = 1;

    public GameObject hurtParticle;

    [HideInInspector]
    public Transform player;
    private Rigidbody2D rb;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            GameOverManager.instance.DamagePlayer();
    }

    public void Move(Vector2 targetPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, Mathf.Infinity, LayerMask.GetMask("Platform", "Wall"));
        if (hit.collider)
        {
            targetPos = new Vector2(Mathf.Clamp(targetPos.x, hit.transform.position.x + 0.5f - hit.collider.bounds.size.x / 2, hit.transform.position.x - 0.5f + hit.collider.bounds.size.x / 2), targetPos.y);
        }

        rb.position = new Vector3(Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime).x, transform.position.y, 0);
    }

    public void Damage(int damage)
    {
        if (health <= 0)
            return;

        health -= damage;

        if (health <= 0)
        {
            if (hurtParticle)
                Instantiate(hurtParticle, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
