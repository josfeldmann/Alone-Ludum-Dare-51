using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float rotSpeed;
    public InputManager input;
    // Update is called once per frame

    float rotx = 0;
    float roty = 0;


    private void Awake() {
        rotx = 0;
        roty = 0;
        transform.localEulerAngles = new Vector3(rotx, roty, 0);
    }

    void Update()
    {
        rotx += -input.lookY * Time.deltaTime * rotSpeed;
        roty += input.lookX * Time.deltaTime * rotSpeed;
        transform.localEulerAngles = new Vector3(rotx, roty, 0);


        // transform.Rotate(new Vector3(-input.lookY, input.lookX, 0) * Time.deltaTime * rotSpeed, Space.World);
    }
}
