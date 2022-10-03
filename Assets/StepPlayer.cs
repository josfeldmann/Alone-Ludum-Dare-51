using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepPlayer : MonoBehaviour
{
    public AudioSource step;
    
    public void PlayStep() {
        step.Play();
    }

}
