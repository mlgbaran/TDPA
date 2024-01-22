using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject bloodprefab;
    public float TimeToLive = 1f;
    private void Start()
    {
        Destroy(gameObject, TimeToLive);
    }
    // Update is called once per frame
    void Update()
    {

    }


    //can be modified with if statements and more layers like if it hits a wall, the bullet makes a hole, if it hits glass, the bullet breaks it etc.
    void OnTriggerEnter2D(Collider2D collision)
    {
        //destroy the bullet if it hits something HITTABLE (current layer)
        Debug.DrawLine(transform.position, new Vector3(0, 0, 0), Color.green, 1.0f);



        GameObject bloodpe = Instantiate(bloodprefab, transform.position, transform.rotation);

        Destroy(bloodpe, 1.0f);

        Destroy(gameObject);




    }
}
