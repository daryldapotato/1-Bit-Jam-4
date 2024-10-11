using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{
    public int shotgunPelletAmount;
    public float shotgunSpreadAngle;
    public float shotgunBulletSpeed;
    public float shotgunBulletLifetime;
    public float shotgunRecoil;
    public float shotgunFireCooldown;
    public Transform shotgunOrigin;
    public Rigidbody2D playerRb;
    public AudioSource shotgunShotAudioSource;

    [Space]
    public float grappleExtensionSpeed;
    public float grapplingHookLengthOffset;
    public Transform grapplingOrigin;
    public SpringJoint2D grapplingSpringJoint;
    public LineRenderer grapplingLineRenderer;
    public AudioSource grapplingAttachedAudioSource;

    [Space]
    public Transform aimPivot;
    public GameObject shotgunPelletPrefab;

    private float nextShotgunFire;
    private float grappleLength;
    private bool grappleConnected = false;

    private void Update()
    {
        if (grappleLength <= 0f)
        {
            Vector2 pointPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            aimPivot.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(aimPivot.position.y - pointPos.y, aimPivot.position.x - pointPos.x) * Mathf.Rad2Deg + 90f);
        }

        if (Input.GetMouseButtonDown(0) && Time.time >= nextShotgunFire)
            Shotgun();

        if (Input.GetMouseButtonDown(1))
            grappleConnected = false;

        if (Input.GetMouseButton(1))
        {
            grappleLength += grappleExtensionSpeed * Time.deltaTime;
            GrapplingHookShoot();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            grappleConnected = false;
            GrapplingHookRetract();
        }
    }

    private void Shotgun()
    {
        grappleConnected = true;
        GrapplingHookRetract();

        for (int i = 0; i < shotgunPelletAmount; i++)
        {
            GameObject newPellet = Instantiate(shotgunPelletPrefab, shotgunOrigin.position, Quaternion.identity);
            float pelletDir = Random.Range(-shotgunSpreadAngle / 2, shotgunSpreadAngle / 2) + shotgunOrigin.rotation.eulerAngles.z + 90f;
            newPellet.GetComponent<Rigidbody2D>().AddForce(new Vector2(Mathf.Cos(pelletDir * Mathf.Deg2Rad), Mathf.Sin(pelletDir * Mathf.Deg2Rad)).normalized * shotgunBulletSpeed);
            Destroy(newPellet, shotgunBulletLifetime);
        }

        playerRb.AddForce(shotgunOrigin.up * -shotgunRecoil, ForceMode2D.Impulse);

        shotgunShotAudioSource.Play();

        nextShotgunFire = Time.time + shotgunFireCooldown;
    }

    private void GrapplingHookShoot()
    {
        if (grappleConnected)
        {
            grapplingLineRenderer.SetPosition(0, grapplingOrigin.position);
            if (grapplingSpringJoint.connectedBody)
                grapplingLineRenderer.SetPosition(1, grapplingSpringJoint.connectedBody.position);

            grappleLength = 0f;

            return;
        }

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, grapplingOrigin.up, grappleLength);

        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {

                if (hit.transform.CompareTag("Enemy"))
                {
                    grapplingSpringJoint.connectedBody = hit.rigidbody;
                    grapplingSpringJoint.connectedAnchor = Vector2.zero;

                    grapplingAttachedAudioSource.Play();
                }
                else if (hit.transform.CompareTag("Platform"))
                {
                    if (transform.position.y > hit.point.y)
                    {
                        grapplingSpringJoint.connectedBody = null;
                        grapplingSpringJoint.connectedAnchor = hit.point;

                        grapplingAttachedAudioSource.Play();
                    }
                    else
                    {
                        grapplingLineRenderer.SetPosition(0, grapplingOrigin.position);
                        grapplingLineRenderer.SetPosition(1, grapplingOrigin.position + (grapplingOrigin.up * grappleLength));
                        grapplingLineRenderer.enabled = true;

                        continue;
                    }
                }
                else
                {
                    grapplingSpringJoint.connectedBody = null;
                    grapplingSpringJoint.connectedAnchor = hit.point;

                    grapplingAttachedAudioSource.Play();
                }

                grapplingSpringJoint.enabled = true;

                grapplingLineRenderer.SetPosition(0, grapplingOrigin.position);
                grapplingLineRenderer.enabled = true;

                if (grapplingSpringJoint.connectedBody)
                    grapplingLineRenderer.SetPosition(1, grapplingSpringJoint.connectedBody.position);
                else
                    grapplingLineRenderer.SetPosition(1, hit.point);

                grappleConnected = true;
                grappleLength = 0f;

                break;
            }
        }
        else
        {
            grapplingLineRenderer.SetPosition(0, grapplingOrigin.position);
            grapplingLineRenderer.SetPosition(1, grapplingOrigin.position + (grapplingOrigin.up * grappleLength));
            grapplingLineRenderer.enabled = true;
        }
    }

    private void GrapplingHookRetract()
    {
        grapplingSpringJoint.enabled = false;
        grapplingLineRenderer.enabled = false;
        grappleLength = 0f;
    }
}
