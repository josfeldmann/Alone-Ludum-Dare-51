using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wiggler : MonoBehaviour
{
    public float wiggleAmount = 15f;
    public float wiggleSpeed = 45f;
    float currentTarget;
    float currentamt;

    private void Awake() {
        currentamt = 0;
        if (Random.Range(0,0.5f) > 0.5f) {
            currentTarget = wiggleAmount;
        } else {
            currentTarget = -wiggleAmount;
        }
    }

    private void Update() {
        if (currentTarget != currentamt) {
            currentamt = Mathf.MoveTowards(currentamt, currentTarget, wiggleSpeed * Time.deltaTime);
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, currentamt);
        } else {
            currentTarget = -currentTarget;
        }

    }
}
