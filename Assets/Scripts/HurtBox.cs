using UnityEngine;

public class HurtBox : MonoBehaviour {
    public Unit attachedUnit;

    public void TakeDamage(float amt) {
        attachedUnit.TakeDamage(amt);
    }


}


