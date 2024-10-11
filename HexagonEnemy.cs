using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HexagonEnemy : Enemy
{
    public float fireDelay;
    public float fireSpeed;
    public float bulletSpeed;
    public int maxRicoshots;
    public float projectileThickness;

    public GameObject projectilePrefab;
    public LineRenderer aimIndicator;

    private float nextFire;
    private List<Vector2> fireRicoshots = new List<Vector2>();

    private void Start()
    {
        nextFire = Time.time + fireSpeed;

        aimIndicator.material.mainTextureScale = new Vector2(1f / aimIndicator.startWidth, 1.0f);
    }

    private void Update()
    {
        if (nextFire <= Time.time)
        {
            float fireAngle = Random.Range(0, 360);
            Vector2 fireDir = new Vector2(Mathf.Cos(fireAngle * Mathf.Deg2Rad), Mathf.Sin(fireAngle * Mathf.Deg2Rad)).normalized;

            if (ProjectRaycast(transform.position, fireDir, true))
            {
                nextFire = Time.time + fireSpeed;
                StartCoroutine(Shoot(fireDir));
            }
        }
    }

    private bool ProjectRaycast(Vector2 fireOrigin, Vector2 fireDir, bool newFire)
    {
        if (newFire)
            fireRicoshots.Clear();
        else if (fireRicoshots.Count >= maxRicoshots)
            return false;

        RaycastHit2D[] hits = Physics2D.RaycastAll(fireOrigin, fireDir);

        foreach (var hit in hits)
        {
            if (hit.transform.CompareTag("Platform"))
            {
                if (fireOrigin.y > hit.transform.position.y)
                {
                    fireRicoshots.Add(hit.point - (fireDir.normalized * projectileThickness));
                    return ProjectRaycast(hit.point - (fireDir.normalized * projectileThickness), Vector2.Reflect(fireDir, hit.normal), false);
                }
            }
            else if (hit.transform.CompareTag("Wall"))
            {
                fireRicoshots.Add(hit.point - (fireDir.normalized * projectileThickness));
                return ProjectRaycast(hit.point - (fireDir.normalized * projectileThickness), Vector2.Reflect(fireDir, hit.normal), false);
            }
            else if (hit.transform.CompareTag("Player"))
            {
                fireRicoshots.Add(hit.point - (fireDir.normalized * projectileThickness));
                return true;
            }
            else if (hit.transform.CompareTag("Enemy"))
            {
                return false;
            }
        }

        return false;
    }

    private IEnumerator Shoot(Vector2 fireDir)
    {
        fireRicoshots.Insert(0, transform.position);
        aimIndicator.positionCount = fireRicoshots.Count;

        for (int i = 0; i < fireRicoshots.Count; i++)
        {
            if (i < fireRicoshots.Count - 1)
                aimIndicator.SetPosition(i, (Vector3)fireRicoshots[i]);
            else
            {
                Debug.Log("e");

                RaycastHit2D[] hits = Physics2D.RaycastAll(fireRicoshots[i - 1], (fireRicoshots[i] - fireRicoshots[i - 1]).normalized);

                foreach (var hit in hits)
                {
                    if (hit.transform.CompareTag("Platform"))
                    {
                        if (fireRicoshots[i - 1].y > hit.transform.position.y)
                        {
                            aimIndicator.SetPosition(i, hit.point);
                            break;
                        }
                    }
                    else if (hit.transform.CompareTag("Wall"))
                    {
                        aimIndicator.SetPosition(i, hit.point);
                        break;
                    }
                }
            }
        }

        aimIndicator.enabled = true;

        yield return new WaitForSeconds(fireDelay);

        aimIndicator.enabled = false;

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), projectile.GetComponent<Collider2D>());
        projectile.GetComponent<Rigidbody2D>().AddForce(fireDir * bulletSpeed, ForceMode2D.Impulse);
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < fireRicoshots.Count; i++)
        {
            if (i > 0)
                Gizmos.DrawLine(fireRicoshots[i - 1], fireRicoshots[i]);
            else
                Gizmos.DrawLine(transform.position, fireRicoshots[i]);
        }
    }
}
