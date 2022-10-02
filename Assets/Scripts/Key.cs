using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public LayerMask pickUpLayer;
    public float scaleSpeed;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (Layers.InLayerMask(pickUpLayer, collision.gameObject.layer)) {
            TopDownPlayerController t = collision.gameObject.GetComponent<TopDownPlayerController>();
            if (!t.hasKey) {
                t.hasKey = true;
                ScaleDown();
            }

        }
    }

    public void ScaleDown() {
        StopAllCoroutines();
        StartCoroutine(ScaleDownNum());
    }

    public void Reset() {
        StopAllCoroutines();
        transform.localScale = Vector3.one;
    }

    IEnumerator ScaleDownNum() {
        while(transform.localScale != Vector3.zero) {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, scaleSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
