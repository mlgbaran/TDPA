using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isFemale;

    public bool isEquipped;

    public NPCRotation npcRotation;

    public GameObject shootingArms;

    public Animator animator;

    public GameObject weapon;

    public Shooting shooting;
    // Start is called before the first frame update

    void Awake()
    {

        animator.SetBool("isFemale", isFemale);

    }


    void Start()
    {

        //start unequipped
        LeaveWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        //npc equips a weapon or sth
    }

    void NPCEquipWeaponTrigger()
    {


    }


    void EquipWeapon()
    {
        //handle all weapon equipment stuff here
        //input should be probably Weapon weapon, as we'll need a weapon class

        isEquipped = true;

        npcRotation.isEquipped = true;

        shootingArms.SetActive(true);

        animator.SetBool("isEquippedPistol", true);

        shooting.enabled = true;

        weapon.SetActive(true);

    }


    void LeaveWeapon()
    {

        isEquipped = false;

        npcRotation.isEquipped = false;

        shootingArms.SetActive(false);

        animator.SetBool("isEquippedPistol", false);

        shooting.enabled = true;

        weapon.SetActive(false);

    }
}
