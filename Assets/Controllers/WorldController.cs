using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {

    static WorldController _instance;
    public static WorldController Instance { get; protected set; }

    public Sprite floorSprite;
    public World World { get; protected set; }

    // Start is called before the first frame update
    void Start() {

        if(Instance != null) {
            Debug.LogError("There should never be two world controllers.");
        }
        Instance = this;

        World = new World();

        for(int x = 0; x < World.Width; x++) {
            for(int y = 0; y < World.Height; y++) {
                Tile tileData = World.GetTileAt(x, y);
                
                GameObject tileGameObject = new GameObject();
                tileGameObject.name = $"Tile_{x}_{y}";
                tileGameObject.transform.position = new Vector3(tileData.X, tileData.Y, 0);
                tileGameObject.transform.SetParent(this.transform, true);

                tileGameObject.AddComponent<SpriteRenderer>();

                tileData.RegisterTileTypeChangedCallback( (tile) => { OnTileTypeChanged(tile, tileGameObject); } );
            }
        }

        World.RandomizeTiles();
    }

    // Update is called once per frame
    void Update() {

    }

    /* 
        Callback function that we set on each tile. Whenever the tile's type changes,
        this function will be called so that the correct sprite gets set on the 
        SpriteRenderer.
     */
    void OnTileTypeChanged(Tile tileData, GameObject tileGameObject) {

        if(tileData.Type == Tile.TileType.Floor) {
            tileGameObject.GetComponent<SpriteRenderer>().sprite = floorSprite;
        } else if (tileData.Type == Tile.TileType.Empty) {
            tileGameObject.GetComponent<SpriteRenderer>().sprite = null;
        } else {
            Debug.LogError("OnTileTypeChanged - Unrecognised tile type.");
        }

    }
}
