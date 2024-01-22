using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerDrunkLevel : MonoBehaviour
{
    // Start is called before the first frame update


    public float maxDrunklevel = 100;

    public float currentDrunklevel;

    public DrunkBar drunkBar;

    public float decayRate = 1;

    public GameObject globalvolume;

    private Volume volume;

    private ChromaticAberration chromaticAberration;

    public float drunkEffectTreshold;



    void Start()
    {



        currentDrunklevel = maxDrunklevel;

        drunkBar.SetMaxDrunklevel(maxDrunklevel);

        volume = globalvolume.GetComponent<Volume>();

        volume.profile.TryGet(out chromaticAberration);

    }

    // Update is called once per frame
    void Update()
    {

        soberUp(Time.deltaTime * decayRate);

        if (Input.GetKeyUp(KeyCode.Space))
        {
            soberUp(20);
        }

        chromaticAberration.intensity.value = (currentDrunklevel / 100f) - drunkEffectTreshold / 100f;

        //Debug.Log(chromaticAberration.intensity.value);

    }


    public void soberUp(float drunkPoints)
    {
        if (currentDrunklevel - drunkPoints < 0)
        {

            currentDrunklevel = 0;

        }
        else
        {

            currentDrunklevel -= drunkPoints;

        }

        drunkBar.setDrunklevel(currentDrunklevel);
    }

    public void getDrunk(float drunkPoints)
    {
        if (currentDrunklevel + drunkPoints > 100)
        {

            currentDrunklevel = 100;

        }
        else
        {

            currentDrunklevel += drunkPoints;

        }

        drunkBar.setDrunklevel(currentDrunklevel);
    }
}
