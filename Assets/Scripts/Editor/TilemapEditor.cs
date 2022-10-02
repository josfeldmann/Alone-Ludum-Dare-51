using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(Tilemap))]
[CanEditMultipleObjects]
public class TilemapEditor : Editor {
  

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("Clear All Tiles")) {
            ClearTiles();
        }
    }

    public void ClearTiles() {
        ((Tilemap)target).ClearAllTiles();
    }

}
