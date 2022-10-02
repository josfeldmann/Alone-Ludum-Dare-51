using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public float foodAmount = 25f;
    public LayerMask feedLayer;
    public float scalespeed = 1f;
    public bool eatenYet = false;


    private void Awake() {
        eatenYet = false;
        ScaleUp();
    }

    public void ScaleUp() {
        StopAllCoroutines();
        StartCoroutine(ScaleUpNum());
    }

    public void ScaleDown() {
        StopAllCoroutines();
        StartCoroutine(ScaleDownNum());

    }


    public IEnumerator ScaleUpNum() {
        transform.localScale = Vector3.zero;
        yield return null;
        while (transform.localScale != Vector3.one) {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one, Time.deltaTime * scalespeed);
            yield return null;
        }
    }


    public IEnumerator ScaleDownNum() {
        yield return null;
        while (transform.localScale != Vector3.zero) {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, Time.deltaTime * scalespeed);
            yield return null;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision) {
        if ( Layers.InLayerMask(feedLayer,collision.gameObject.layer)) {
            if (!eatenYet) {
                TopDownPlayerController t = collision.gameObject.GetComponent<TopDownPlayerController>();
                t.AddFood(foodAmount);
                eatenYet = true;
                ScaleDown();
            }
        }
    }


}
