using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBox : MonoBehaviour
{

    public LayerMask mask;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (Layers.InLayerMask(mask, collision.gameObject.layer)) {
            TopDownPlayerController t = collision.gameObject.GetComponent<TopDownPlayerController>();
            if (t.IsInKillableState()) {
                t.Kill();
            }
        }
    }
}
