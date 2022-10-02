//using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public LayerMask targetLayer;
    public LayerMask collideLayer;


   // [Button("Fire Button Down")]
    public virtual void FireButtonDown() {

    }

    public virtual void FireButtonHeld() {

    }

    public virtual void FireButtonUp() {

    }
}


