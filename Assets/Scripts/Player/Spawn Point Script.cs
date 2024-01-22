using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointScript : MonoBehaviour
{
    public Transform Player;

    // Start is called before the first frame update
    void Awake()
    {
        Player.position = transform.position;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
