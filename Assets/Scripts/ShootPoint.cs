using UnityEngine;

public class ShootPoint : MonoBehaviour {

    public Projectile projectile;
    public Weapon attachedWeapon;
    public float damage, speed, expireTime;


    public void Fire() {
        Debug.Log("PFire");
        Projectile p = Instantiate(projectile, transform.position, transform.rotation);
        p.Set(damage, speed, transform.forward, expireTime, attachedWeapon.targetLayer, attachedWeapon.collideLayer);
    }


}


