using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon {

    public List<ShootPoint> points;
    public float timeInBetweenShots = 0.25f;

    private void Awake() {
        foreach (ShootPoint s in points) {
            s.attachedWeapon = this;
        }
    }

    public void Shoot() {
        foreach (ShootPoint s in points) {
            s.Fire();
        }
    }

    public override void FireButtonDown() {
        Debug.Log("Fire " + Time.time);
        Shoot();
    }

}


