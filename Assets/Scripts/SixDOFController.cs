using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SixDOFController : MonoBehaviour
{
    public InputManager input;
    public Rigidbody body;

    public Transform lookTransform;
    public float moveSpeed;


    private void Awake() {

      
        
    }

    private void Update() {
        Vector3 movedir = Vector3.ClampMagnitude(((lookTransform.right * input.horizontal) + (lookTransform.up * input.vertical) + (lookTransform.forward * input.forward)), 1);
        body.velocity = movedir * moveSpeed;
    }

}
