using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {

    public static WorldController Instance { get; protected set; }

    public Sprite floorSprite;
    
    Dictionary<Tile, GameObject> tileGameObjectMap;
    Dictionary<InstalledObject, GameObject> installedObjectGameObjectMap;

    Dictionary<string, Sprite> installedObjectSprites;

    public World World { get; protected set; }

    // Start is called before the first frame update
    void Start() {

        installedObjectSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/InstalledObjects");

        Debug.Log("Loaded resource:");
        foreach(Sprite sprite in sprites) {
            Debug.Log(sprite.name);
            installedObjectSprites[sprite.name] = sprite;
        }

        if(Instance != null) {
            Debug.LogError("There should never be two world controllers.");
        }
        Instance = this;

        World = new World();
        // Register a callback so that the World will tell the WorldController when an InstalledObject is created,
        // so it can create a corresponding GameObject.
        World.RegisterInstalledObjectCreatedCallback(OnInstalledObjectCreated);

        // Instantiate dicionary which maps each tile to a game object.
        tileGameObjectMap = new Dictionary<Tile, GameObject>();
        installedObjectGameObjectMap = new Dictionary<InstalledObject, GameObject>();

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

    // Callback registered to the World object. Called whenever an InstalledObject is created. This
    // allows us to create a GameObject for the InstalledObject so we can see and interact with it.
    public void OnInstalledObjectCreated(InstalledObject obj) {
        // Create a visual GameObject linked to this data.
        GameObject objGameObject = new GameObject();

        // Add the installed object/game object pair to the dictionary.
        installedObjectGameObjectMap.Add(obj, objGameObject);

        // Tell the game object where it should go.
        objGameObject.name = $"{obj.objectType}_{obj.tile.X}_{obj.tile.Y}";
        objGameObject.transform.position = new Vector3(obj.tile.X, obj.tile.Y, 0);
        objGameObject.transform.SetParent(this.transform, true);

        // Add a sprite renderer and assume it's a wall for now.
        objGameObject.AddComponent<SpriteRenderer>().sprite = installedObjectSprites["Wall_"];
        objGameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;

        // Register a callback to the WorldController here so we can update the GameObject whenever
        // the underlying InstalledObject changes.
        obj.RegisterOnChangedCallback(OnInstalledObjectChanged);
    }

    // Callback registered to the InstalledObject. Called whenever the object changes so that
    // we can keep the corresponding GameObject up to date.
    void OnInstalledObjectChanged(InstalledObject obj) {
        Debug.LogError("OnInstalledObjectChanged - Not implemented.");
    }
}
