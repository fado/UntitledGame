using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {

    public static WorldController Instance { get; protected set; }

    public Sprite floorSprite;
    Dictionary<Tile, GameObject> tileGameObjectMap;
    public World World { get; protected set; }

    // Start is called before the first frame update
    void Start() {

        if(Instance != null) {
            Debug.LogError("There should never be two world controllers.");
        }
        Instance = this;

        World = new World();

        // Instantiate dicionary which maps each tile to a game object.
        tileGameObjectMap = new Dictionary<Tile, GameObject>();

        for(int x = 0; x < World.Width; x++) {
            for(int y = 0; y < World.Height; y++) {
                Tile tileData = World.GetTileAt(x, y);
                
                GameObject tileGameObject = new GameObject();

                tileGameObjectMap.Add(tileData, tileGameObject);

                tileGameObject.name = $"Tile_{x}_{y}";
                tileGameObject.transform.position = new Vector3(tileData.X, tileData.Y, 0);
                tileGameObject.transform.SetParent(this.transform, true);

                tileGameObject.AddComponent<SpriteRenderer>();

                tileData.RegisterTileTypeChangedCallback( OnTileTypeChanged );
            }
        }

        World.RandomizeTiles();
    }

    // Update is called once per frame
    void Update() {

    }

     
    // Callback function that we set on each tile. Whenever the tile's type changes,
    // this function will be called so that the correct sprite gets set on the 
    // SpriteRenderer.
    void OnTileTypeChanged(Tile tileData) {

        if(tileGameObjectMap.ContainsKey(tileData) == false) {
            Debug.LogError("tileGameObjectMap does not contain tile data.");
            return;
        }

        GameObject tileGameObject = tileGameObjectMap[tileData];

        if(tileGameObject == null) {
            Debug.LogError("GameObject mapped to tile data was null.");
            return;
        }

        if(tileData.Type == TileType.Floor) {
            tileGameObject.GetComponent<SpriteRenderer>().sprite = floorSprite;
        } else if (tileData.Type == TileType.Empty) {
            tileGameObject.GetComponent<SpriteRenderer>().sprite = null;
        } else {
            Debug.LogError("OnTileTypeChanged - Unrecognised tile type.");
        }

    }

    public Tile GetTileAtWorldCoord(Vector3 coord) {
        int x = Mathf.FloorToInt(coord.x);
        int y = Mathf.FloorToInt(coord.y);

        return World.GetTileAt(x, y);
    }
}
