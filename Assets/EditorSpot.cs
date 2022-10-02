using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorSpot : MonoBehaviour
{
    public SpotType spotType;

    private void OnDrawGizmos() {
        switch (spotType) {
            case SpotType.ENEMY:
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.position, 1);
                break;
            case SpotType.OBSTACLE:
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(transform.position, 1);
                break;
            case SpotType.FOOD:
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(transform.position, 0.5f);
                break;
        }
    }

}


public enum SpotType { ENEMY, OBSTACLE, FOOD}