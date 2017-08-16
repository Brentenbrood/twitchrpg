using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour {
    public float Health
    {
        get { return Slider.value*100; }
        set { Slider.value = value / 100; }
    }

    public bool RotateToCamera = true;

    public bool Show
    {
        set { Slider.gameObject.SetActive(value); }
        get { return Slider.gameObject.activeInHierarchy; }
    }

    public Slider Slider;

    void Update()
    {
        if (RotateToCamera)
        {
            var cam = Camera.main;
            if (cam)
            {
                transform.LookAt(cam.transform.position);
                var angles = transform.localEulerAngles;
                angles.x = 0;
                angles.z = 0;
                transform.localEulerAngles = angles;
            }

        }
    }
}
