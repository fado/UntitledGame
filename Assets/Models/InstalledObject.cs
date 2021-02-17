using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstalledObject {

    Tile tile; // Base tile of the object.
    string objectType;
    float movementCost; // A movementCost of zero is impassable (e.g. a wall).
    int width;
    int height;

    protected InstalledObject() {

    }

    static public InstalledObject CreatePrototype( string objectType, float movementCost = 1f, int width = 1, int height = 1) {
        InstalledObject obj = new InstalledObject();
        obj.objectType = objectType;
        obj.movementCost = movementCost;
        obj.width = width;
        obj.height = height;

        return obj;
    }

    static public InstalledObject PlaceInstance( InstalledObject prototype, Tile tile) {
        InstalledObject obj = new InstalledObject();

        obj.objectType = prototype.objectType;
        obj.movementCost = prototype.movementCost;
        obj.width = prototype.width;
        obj.height = prototype.height;

        obj.tile = tile;

        if(tile.PlaceObject(obj) == false) {
            // Unable to place object. May already be occupied.
            // Don't return new object. It will be garbage collected.
            return null;
        };

        return obj;
    }

}
