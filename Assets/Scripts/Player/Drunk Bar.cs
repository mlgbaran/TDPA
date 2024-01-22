using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DrunkBar : MonoBehaviour

{


    public Slider slider;

    public void SetMaxDrunklevel(float drunklevel)
    {

        slider.maxValue = drunklevel;
        slider.value = drunklevel;

    }

    public void setDrunklevel(float drunklevel)
    {
        slider.value = drunklevel;
    }
}
