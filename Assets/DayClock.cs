using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayClock : MonoBehaviour
{
    public DayController controller;


    private void Update() {
        transform.localEulerAngles = new Vector3(0, 0, 360f * (((Time.time - controller.startTime) % (controller.dayNightTime * 2)) / (controller.dayNightTime * 2)));
    }

}
