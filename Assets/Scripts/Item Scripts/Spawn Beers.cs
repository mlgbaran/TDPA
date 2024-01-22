using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBeers : MonoBehaviour
{

    public GameObject beer;

    public float spawnPointZ;

    public bool isActive = false;

    public float spawnAreaRadius;

    private float nextActionTime = 0.0f;
    public float spawnRate = 0.5f;

    private float randomx;
    private float randomy;

    private Vector3 spawnPoint;


    // Start is called before the first frame update
    void Start()
    {



    }

    // Update is called once per frame
    void Update()

    {
        if (isActive && Time.time > nextActionTime)
        {
            nextActionTime += 1 / spawnRate;

            spawnBeer(beer);


        }


    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, spawnAreaRadius);
    }


    void spawnBeer(GameObject beer)
    {
        randomx = UnityEngine.Random.Range(transform.position.x, spawnAreaRadius);

        randomy = UnityEngine.Random.Range(transform.position.y, spawnAreaRadius);

        spawnPoint = new Vector3(randomx, randomy, spawnPointZ);

        Instantiate(beer, spawnPoint, transform.rotation);

        //Debug.Log("Beer spawned");
    }

}
