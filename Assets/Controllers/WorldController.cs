using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {

    public Sprite floorSprite;
    World world;

    // Start is called before the first frame update
    void Start() {
        world = new World();

        for(int x = 0; x < world.Width; x++) {
            for(int y = 0; y < world.Height; y++) {
                Tile tileData = world.GetTileAt(x, y);
                
                GameObject tileGameObject = new GameObject();
                tileGameObject.name = $"Tile_{x}_{y}";
                tileGameObject.transform.position = new Vector3(tileData.X, tileData.Y, 0);
                tileGameObject.transform.SetParent(this.transform, true);

                tileGameObject.AddComponent<SpriteRenderer>();

                tileData.RegisterTileTypeChangedCallback( (tile) => { OnTileTypeChanged(tile, tileGameObject); } );
            }
        }

        world.RandomizeTiles();
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
