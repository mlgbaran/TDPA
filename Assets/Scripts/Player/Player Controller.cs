using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public bool isEquipped;

    public PlayerRotation playerRotation;

    public GameObject shootingArms;

    public Animator animator;

    public GameObject weapon;

    public Shooting shooting;
    // Start is called before the first frame update
    void Start()
    {

        //start unequipped
        LeaveWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {

            if (isEquipped)
            {
                LeaveWeapon();
            }
            else
            {
                EquipWeapon();
            }
        }
    }


    void EquipWeapon()
    {
        //handle all weapon equipment stuff here
        //input should be probably Weapon weapon, as we'll need a weapon class

        isEquipped = true;

        playerRotation.isEquipped = true;

        shootingArms.SetActive(true);

        animator.SetBool("isEquippedPistol", true);

        shooting.enabled = true;

        weapon.SetActive(true);

    }


    void LeaveWeapon()
    {

        isEquipped = false;

        playerRotation.isEquipped = false;

        shootingArms.SetActive(false);

        animator.SetBool("isEquippedPistol", false);

        shooting.enabled = true;

        weapon.SetActive(false);

    }
}
