using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class PistolScript : MonoBehaviour
{
    public Camera cam;
    public Transform shootingArms;

    private Transform shoulderCenter;

    private ShoulderScript shoulderScript;

    private Transform gunPoint;

    private float zlevel;

    private Transform gunholder;

    private MonoBehaviour rotationScript;

    private bool toMouse = false;

    public float holderDirectionX;

    public float holderDirectionY;

    public float offsetX = 0;
    public float offsetY = 0;

    private PlayerRotation playerRotation;

    private NPCRotation npcRotation;

    private bool isPlayer;

    // Start is called before the first frame update
    void Start()
    {
        shoulderScript = shootingArms.GetComponent<ShoulderScript>();

        shoulderCenter = shootingArms.GetChild(2);

        gunPoint = shoulderCenter.GetChild(0);

        gunholder = shootingArms.parent;

        isPlayer = gunholder.gameObject.tag == "Player";

        if (isPlayer)
        {
            playerRotation = gunholder.GetComponent<PlayerRotation>();
        }



        npcRotation = gunholder.GetComponent<NPCRotation>();

        if (isPlayer) //if the player is running this script
        {
            if (playerRotation.lookDirection == null)
            {
                toMouse = true;
            }
            else
            {
                toMouse = false;
            }


        }
        else
        {
            toMouse = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayer)
        {
            holderDirectionX = playerRotation.myDirectionX;
            holderDirectionY = playerRotation.myDirectionY;




            if (holderDirectionX == 1 && holderDirectionY == 0)
            {//RIGHT

                offsetY = 0.011f;
                offsetX = 0.01f;
            }
            else if (holderDirectionX == 1 && holderDirectionY == -1)
            {//RIGHT DOWN

                offsetY = 0.005f;
                offsetX = 0.01f;
            }
            else if (holderDirectionX == 0 && holderDirectionY == -1)
            {//DOWN
                offsetY = 0;
                offsetX = 0;

            }
            else if (holderDirectionX == -1 && holderDirectionY == -1)
            {//LEFT DOWN

                offsetY = 0.005f;
                offsetX = -0.01f;
            }
            else if (holderDirectionX == -1 && holderDirectionY == 0)
            {//LEFT

                offsetY = 0.011f;
                offsetX = -0.01f;
            }
            else if (holderDirectionX == -1 && holderDirectionY == 1)
            {//LEFT UP

                offsetY = 0.015f;
                offsetX = -0.01f;
            }
            else if (holderDirectionX == 0 && holderDirectionY == 1)
            {//UP
                offsetY = 0;
                offsetX = 0;

            }
            else if (holderDirectionX == 1 && holderDirectionY == 1)
            {//RIGHT UP
                offsetY = 0.015f;
                offsetX = 0.01f;
            }

            zlevel = shoulderScript.zlevel;

            transform.position = new Vector3(gunPoint.position.x + offsetX, gunPoint.position.y + offsetY, gunholder.position.z + zlevel - 0.001f);

            if (toMouse)
            {
                FaceTowards(cam.ScreenToWorldPoint(Input.mousePosition));
            }
            else
            {
                FaceTowards(playerRotation.lookDirection.transform.position);
            }

        }
        else
        {
            holderDirectionX = npcRotation.myDirectionX;
            holderDirectionY = npcRotation.myDirectionY;




            if (holderDirectionX == 1 && holderDirectionY == 0)
            {//RIGHT

                offsetY = 0.011f;
                offsetX = 0.01f;
            }
            else if (holderDirectionX == 1 && holderDirectionY == -1)
            {//RIGHT DOWN

                offsetY = 0.005f;
                offsetX = 0.01f;
            }
            else if (holderDirectionX == 0 && holderDirectionY == -1)
            {//DOWN
                offsetY = 0;
                offsetX = 0;

            }
            else if (holderDirectionX == -1 && holderDirectionY == -1)
            {//LEFT DOWN

                offsetY = 0.005f;
                offsetX = -0.01f;
            }
            else if (holderDirectionX == -1 && holderDirectionY == 0)
            {//LEFT

                offsetY = 0.011f;
                offsetX = -0.01f;
            }
            else if (holderDirectionX == -1 && holderDirectionY == 1)
            {//LEFT UP

                offsetY = 0.015f;
                offsetX = -0.01f;
            }
            else if (holderDirectionX == 0 && holderDirectionY == 1)
            {//UP
                offsetY = 0;
                offsetX = 0;

            }
            else if (holderDirectionX == 1 && holderDirectionY == 1)
            {//RIGHT UP
                offsetY = 0.015f;
                offsetX = 0.01f;
            }

            zlevel = shoulderScript.zlevel;

            transform.position = new Vector3(gunPoint.position.x + offsetX, gunPoint.position.y + offsetY, gunholder.position.z + zlevel - 0.001f);

            if (toMouse)
            {
                FaceTowards(cam.ScreenToWorldPoint(Input.mousePosition));
            }
            else
            {
                FaceTowards(npcRotation.lookDirection.transform.position);
            }
        }
    }

    void FaceTowards(Vector3 target)
    {

        target.z = -0.5f;


        target.x = target.x - transform.position.x;
        target.y = target.y - transform.position.y;
        float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));


    }



}
