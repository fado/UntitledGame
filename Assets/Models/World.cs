using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World {
    
    Tile[,] tiles;

    Dictionary<string, InstalledObject> installedObjectPrototypes;

    public int Width { get; protected set; }
    public int Height { get; protected set; }

    Action<InstalledObject> callbackInstalledObjectCreated;

    public World( int width = 100, int height = 100) {
        this.Width = width;
        this.Height = height;

        tiles = new Tile[width, height];

        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                tiles[x,y] = new Tile(this, x, y);
            }
        }

        CreateInstalledObjectPrototypes();

        Debug.Log($"World created with {width*height} tiles.");
    }

    void CreateInstalledObjectPrototypes() {
        installedObjectPrototypes = new Dictionary<string, InstalledObject>();

        installedObjectPrototypes.Add("Wall", InstalledObject.CreatePrototype(
            "Wall",
            0, // Impassable.
            1, // Width.
            1,  // Height.
            true // Links to neighbours.
        ));

    }

    public Tile GetTileAt(int x, int y) {
        if(x > Width || x < 0 || y > Height || y < 0) {
            Debug.LogError($"Tile ({x},{y}) is out of range.");
            return null;
        }
        return tiles[x,y];
    }

    public void RandomizeTiles() {
        Debug.Log("RandomizeTiles");
        for(int x = 0; x < Width; x++) {
            for(int y = 0; y < Height; y++) {
                if(UnityEngine.Random.Range(0, 2) == 0) {
                    tiles[x,y].Type = TileType.Empty;
                } else {
                    tiles[x,y].Type = TileType.Floor;
                }
            }
        }
    }

    public void PlaceInstalledObject(string objectType, Tile tile) {
        // Assuming 1x1 tiles for now.
        if (installedObjectPrototypes.ContainsKey(objectType) == false) {
            Debug.LogError($"installedObjectPrototypes does not contain prototype for key {objectType}");
            return;
        }

        InstalledObject obj = InstalledObject.PlaceInstance(installedObjectPrototypes[objectType], tile);

        if (obj == null) {
            // Failed to place object, probably something already there.
            return;
        }

        // Alert delegates that an InstalledObject has been created here.
        if(callbackInstalledObjectCreated != null) {
            callbackInstalledObjectCreated(obj);
        }
    }

    public void RegisterInstalledObjectCreatedCallback(Action<InstalledObject> callbackFunction) {
        callbackInstalledObjectCreated += callbackFunction;
    }

    public void UnregisterInstalledObjectCreatedCallback(Action<InstalledObject> callbackFunction) {
        callbackInstalledObjectCreated -= callbackFunction;
    }

}
