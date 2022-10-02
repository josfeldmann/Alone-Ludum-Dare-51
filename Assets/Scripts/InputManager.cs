using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class InputManager : MonoBehaviour
{
    public float horizontal = 0;
    public float vertical = 0;
    public float forward = 0;

    public bool jumpButton;
    public bool jumpButtonDown;
    public bool fireButtonDown;
    public bool fireButton;
    public bool sprintButton;
    public bool sprintButtonDown;
    public bool pauseButtonDown;

    public bool aButton;
    public bool aButtonDown;
    public bool bButton;
    public bool bButtonDown;

    public float lookX, lookY;



    public void OnMove(CallbackContext i) {
       // Debug.Log("Move " + Time.time.ToString());
        Vector2 v = i.ReadValue<Vector2>();
        horizontal =  v.x;
        forward = v.y;
    }

    public void OnLook(CallbackContext i) {
       // Debug.Log("Look " + Time.time.ToString());
        Vector2 v = i.ReadValue<Vector2>();
        lookX = v.x;
        lookY = v.y;
    }


    public void OnVertical(CallbackContext i) {
       // Debug.Log("Vertical Move " + Time.time.ToString());
        float v = i.ReadValue<float>();
        vertical = v;
        
    }

    public void OnFire(CallbackContext i) {
       // Debug.Log("Here " + Time.time);
        if (i.ReadValueAsButton()) {
            fireButton = true;
            fireButtonDown = true;
        } else {
            fireButton = false;
        }
    }

    public void OnJump(CallbackContext i) {
        if (i.ReadValueAsButton()) {
            jumpButton = true;
            jumpButtonDown = true;
        } else {
            jumpButton = false;
        }
    }

    public void OnPause(CallbackContext i) {
        if (i.ReadValueAsButton()) {
            pauseButtonDown = true;
        } else {
        }
    }




    private void LateUpdate() {
        if (fireButtonDown) {
            fireButtonDown = false;
        }
        if (jumpButtonDown) {
            jumpButtonDown = false;
        }
        if (pauseButtonDown) {
            pauseButtonDown = false;
        }
    }




}
