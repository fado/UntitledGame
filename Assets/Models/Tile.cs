using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {
    
    public enum TileType { Empty, Floor };

    private TileType _type = TileType.Empty;

    Action<Tile> callbackTileTypeChanged;
    
    World world;
    public int X { get; protected set; }
    public int Y { get; protected set; }

    LooseObject looseObject;
    InstalledObject installedObject;

    public TileType Type {
        get { return _type; }
        set {
            TileType oldType = _type;
            _type = value;

            if(callbackTileTypeChanged != null && _type != oldType) {
                callbackTileTypeChanged(this);
            }
        }
    }

    public Tile( World world, int x, int y ) {
        this.world = world;
        this.X = x;
        this.Y = y;
    }

    public void RegisterTileTypeChangedCallback(Action<Tile> callbackFunction)  {
        callbackTileTypeChanged += callbackFunction;
    }

    public void UnregisterTileTypeChangedCallback(Action<Tile> callbackFunction)  {
        callbackTileTypeChanged -= callbackFunction;
    }

}
