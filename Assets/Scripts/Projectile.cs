using UnityEngine;

public class Projectile : MonoBehaviour {

    public float damage;
    public float speed;
    public Vector3 direction;
    public LayerMask targetLayer; public LayerMask collideLayer;


    public void Set(float d, float s, Vector3 dir, float expireTime, LayerMask tLayer, LayerMask cLayer) {

        damage = d;
        speed = s;
        direction = dir;
        transform.forward = dir;
        targetLayer = tLayer;
        collideLayer = cLayer;
        Destroy(gameObject, expireTime);

    }


    private void Update() {
        transform.position += direction * Time.deltaTime * speed;
    }

    private void OnTriggerEnter(Collider other) {
        if ( Layers.InLayerMask(targetLayer, other.gameObject.layer)) {
            HurtBox h = other.gameObject.GetComponent<HurtBox>();
            h.TakeDamage(damage);
            Destroy(gameObject);
        }
        if (Layers.InLayerMask(collideLayer, other.gameObject.layer)) {
            Destroy(gameObject);
        }
    }





}


