using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Shooting : MonoBehaviour
{
    public Transform shootingArms;
    public Transform firePoint;

    public Transform shootingArmsReturnPoint;

    public GameObject bulletPrefab;

    public GameObject bloodPrefab;

    public float recoilScale;

    public float bulletForce = 20f;

    private Light2D muzzleLight;
    private Transform emptyShellSpawnPoint;

    private Vector3 originalPosition;

    public Vector3 moveDirection; // Direction in which the object moves
    private Vector3 targetPosition;
    private bool isRecoil = false;

    private LayerMask playerLayerMask;

    private LayerMask enemyLayerMask;

    private int layersExceptPlayer;

    // Start is called before the first frame update
    void Start()
    {
        emptyShellSpawnPoint = firePoint.parent.GetChild(1);

        muzzleLight = firePoint.GetChild(0).GetComponent<Light2D>();

        muzzleLight.enabled = false;

        playerLayerMask = LayerMask.NameToLayer("Player");

        enemyLayerMask = LayerMask.NameToLayer("Enemy");

        layersExceptPlayer = ~(1 << playerLayerMask);


    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }

    }

    Vector2 GetRaysExitPoint(Vector2 entryPoint, Vector2 direction, GameObject hitObj)
    {

        Vector2 newCastPoint = entryPoint + (direction * 1.2f);

        Vector2 exitPoint;

        RaycastHit2D[] hits = Physics2D.RaycastAll(newCastPoint, -direction, 2f, layersExceptPlayer);

        Debug.DrawRay(newCastPoint, -direction, Color.red, 2.0f);

        foreach (RaycastHit2D hitInfo in hits)
        {

            if (hitInfo.collider.gameObject == hitObj)
            {
                Debug.Log("Did hit back");
                exitPoint = hitInfo.point;

                return exitPoint;

            }


        }
        return new Vector2(0, 0);
    }

    [Range(0, 0.5f)]
    public float clampValue;
    Vector2 GetRandomPointBetween(Vector2 firstPoint, Vector2 secondPoint)
    {

        //Debug.Log("First point => X: " + firstPoint.x + " Y: " + firstPoint.y);

        //Debug.Log("Second point => X: " + secondPoint.x + " Y: " + secondPoint.y);

        float t = Random.Range(0 + clampValue, 1 - clampValue);

        Vector2 randomPoint = Vector2.Lerp(firstPoint, secondPoint, t);

        return randomPoint;

    }


    void HitEnemy(RaycastHit2D hitInfo, Vector2 direction)
    {



        Vector2 exitpoint = GetRaysExitPoint(hitInfo.point, direction, hitInfo.transform.gameObject);

        Vector2 bloodposition = GetRandomPointBetween(hitInfo.point, exitpoint);

        //Debug.Log("Enemy Hit");

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle + 180f));

        GameObject bloodpe = Instantiate(bloodPrefab, bloodposition, rotation);



        //Debug.Log(firePoint.rotation);

        //Debug.Log(bloodpe.transform.rotation);

        Destroy(bloodpe, 1.0f);

        //Debug.Break();
    }

    void Shoot()
    {

        if (!isRecoil)
        {
            //Debug.Log(shootingArms.localPosition.z);
            //Debug.Log(originalPosition.z);
            //Debug.Log(shootingArms.position.z);

            //these below are for rigidbody shooting.

            //GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            //
            //Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            //
            //rb.AddForce(firePoint.right * bulletForce, ForceMode2D.Impulse);

            StartCoroutine(MuzzleFlash());

            RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, firePoint.right, Mathf.Infinity, layersExceptPlayer);

            Debug.DrawRay(firePoint.position, firePoint.right, Color.green, 2.0f);

            //Debug.DrawLine(firePoint);

            if (hitInfo)
            {
                Debug.Log(hitInfo.transform.name);

                if (hitInfo.transform.gameObject.layer == enemyLayerMask)
                {
                    HitEnemy(hitInfo, firePoint.right);
                }


            }






            moveDirection = new Vector3(-firePoint.right.x / recoilScale, -firePoint.right.y / recoilScale, shootingArms.localPosition.z);

            //Debug.Log(firePoint.right);
            //Debug.Log(shootingArms.position);

            targetPosition = new Vector3(shootingArmsReturnPoint.position.x + moveDirection.x, shootingArmsReturnPoint.position.y + moveDirection.y, shootingArms.localPosition.z);


            StartCoroutine(MoveObjectCoroutine());
        }



    }
    IEnumerator MuzzleFlash()
    {

        // Enable the light
        muzzleLight.enabled = true;

        // Wait for 5 frames
        for (int i = 0; i < 10; i++)
        {
            yield return null; // Wait for the next frame
        }

        // Disable the light
        muzzleLight.enabled = false;
    }

    IEnumerator MoveObjectCoroutine()
    {
        isRecoil = true;
        int frameCount = 0;

        // Calculate the initial distance from original to target
        float initialDistance = Vector3.Distance(shootingArmsReturnPoint.position, targetPosition);

        while (frameCount < 12)
        {




            targetPosition = new Vector3(shootingArmsReturnPoint.position.x + moveDirection.x, shootingArmsReturnPoint.position.y + moveDirection.y, shootingArms.localPosition.z);
            //Debug.Log(originalPosition);
            //Debug.Log(targetPosition);
            // Calculate the interpolation factor based on frame count
            float t = (float)frameCount / 11.0f; // Divide by 19 to get values from 0 to 1

            // Lerp the position between originalPosition and targetPosition
            shootingArms.position = Vector3.Lerp(shootingArmsReturnPoint.position, targetPosition, t);

            // Calculate the current distance from original to current position

            // Increment frame count
            frameCount++;

            // Yield for the next frame
            yield return null;
        }

        // Ensure the object reaches the exact target position
        shootingArms.position = targetPosition;

        // Wait for a moment (optional)
        // Return to the original position

        StartCoroutine(ReturnToOriginalPosition());
    }

    IEnumerator ReturnToOriginalPosition()
    {


        int frameCount = 0;



        while (frameCount < 12)
        {


            targetPosition = new Vector3(shootingArmsReturnPoint.position.x + moveDirection.x, shootingArmsReturnPoint.position.y + moveDirection.y, shootingArms.localPosition.z);

            float t = (float)frameCount / 11.0f;
            shootingArms.position = Vector3.Lerp(targetPosition, shootingArmsReturnPoint.position, t);

            frameCount++;
            yield return null;
        }

        // Ensure the object reaches the exact original position
        shootingArms.position = shootingArmsReturnPoint.position;
        isRecoil = false;
    }


}
