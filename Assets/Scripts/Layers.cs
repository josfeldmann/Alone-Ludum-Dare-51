using UnityEngine;

public static class Layers {

    public const int ground = 8, player = 6, weapon = 10, interact = 11, grabbable = 12, itemPickup = 13, enemy = 7;


    public static bool InLayerMask(LayerMask mask, int layer) {
        return mask == (mask | (1 << layer));
    }


}