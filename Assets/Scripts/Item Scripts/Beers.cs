using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beers : MonoBehaviour
{
    // Start is called before the first frame update

    public PlayerDrunkLevel playerDrunkLevel;

    private Vector3 _startPosition;

    public float speedUpDown = 1;
    public float distanceUpDown = 1;

    void Start()
    {
        _startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _startPosition + new Vector3(0.0f, Mathf.Sin(speedUpDown * Time.time) * distanceUpDown, 0.0f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            playerDrunkLevel.getDrunk(20);
            Destroy(gameObject);

        }


    }



}
