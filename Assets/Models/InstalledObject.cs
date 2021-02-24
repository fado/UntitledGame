using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstalledObject {

    public Tile tile {get; protected set; } // Base tile of the object.
    public string objectType {get; protected set; }
    float movementCost; // A movementCost of zero is impassable (e.g. a wall).
    int width;
    int height;
    public bool linksToNeighbour { get; protected set; }
    Action<InstalledObject> callbackOnChanged;

    protected InstalledObject() {

    }

    static public InstalledObject CreatePrototype( string objectType, float movementCost = 1f, int width = 1, int height = 1, bool linksToNeighbour = false) {
        InstalledObject obj = new InstalledObject();
        obj.objectType = objectType;
        obj.movementCost = movementCost;
        obj.width = width;
        obj.height = height;
        obj.linksToNeighbour = linksToNeighbour;

        return obj;
    }

    static public InstalledObject PlaceInstance( InstalledObject prototype, Tile tile) {
        InstalledObject obj = new InstalledObject();

        obj.objectType = prototype.objectType;
        obj.movementCost = prototype.movementCost;
        obj.width = prototype.width;
        obj.height = prototype.height;
        obj.linksToNeighbour = prototype.linksToNeighbour;

        obj.tile = tile;

        if(tile.PlaceObject(obj) == false) {
            // Unable to place object. May already be occupied.
            // Don't return new object. It will be garbage collected.
            return null;
        };

        if(obj.linksToNeighbour) {

            Tile t;
            int x = tile.X;
            int y = tile.Y;

            // Inform neighbours that they have a new neighbour by triggering OnChanged callback.
            t = tile.world.GetTileAt(x, y + 1); // North.
            if(t != null && t.installedObject != null && t.installedObject.objectType == obj.objectType) {
                t.installedObject.callbackOnChanged(t.installedObject);
            }
            t= tile.world.GetTileAt(x, y - 1); // South.
            if(t != null && t.installedObject != null && t.installedObject.objectType == obj.objectType) {
                t.installedObject.callbackOnChanged(t.installedObject);
            }
            t= tile.world.GetTileAt(x + 1, y); // East.
            if(t != null && t.installedObject != null && t.installedObject.objectType == obj.objectType) {
                t.installedObject.callbackOnChanged(t.installedObject);
            }
            t= tile.world.GetTileAt(x - 1, y); // West.
            if(t != null && t.installedObject != null && t.installedObject.objectType == obj.objectType) {
                t.installedObject.callbackOnChanged(t.installedObject);
            }
        }

        return obj;
    }

    public void RegisterOnChangedCallback(Action<InstalledObject> callbackFunction) {
        callbackOnChanged += callbackFunction;
    }

    public void UnregisterOnChangedCallback(Action<InstalledObject> callbackFunction) {
        callbackOnChanged -= callbackFunction;
    }

}
