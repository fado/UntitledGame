﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class MouseController : MonoBehaviour {

    public GameObject circleCursorPrefab;

    bool buildModeIsObjects = false;
    TileType buildModeTile = TileType.Floor;
    string buildModeObjectType;

    Vector3 mousePositionLastFrame;
    Vector3 mousePositionCurrentFrame;
    Vector3 dragStartPosition;

    List<GameObject> dragPreviewGameObjects;

    private const int RIGHT_MOUSE_BUTTON = 0;
    private const int LEFT_MOUSE_BUTTON = 1;
    private const int MIDDLE_MOUSE_BUTTON = 2;

    // Start is called before the first frame update
    void Start() {
        dragPreviewGameObjects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update() {
        mousePositionCurrentFrame = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePositionCurrentFrame.z = 0;

        //UpdateCursor();
        UpdateDragSelection();
        UpdateCameraMovement();

        mousePositionLastFrame = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePositionLastFrame.z = 0;
    }

    void UpdateCameraMovement() {
        // Handle camera dragging.
        if(Input.GetMouseButton(LEFT_MOUSE_BUTTON) || Input.GetMouseButton(MIDDLE_MOUSE_BUTTON)) {
            Vector3 diff = mousePositionLastFrame - mousePositionCurrentFrame;
            Camera.main.transform.Translate(diff);
        }

        Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis("Mouse ScrollWheel");
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 3f, 25f); // Set zoom limits.
    }

    void UpdateDragSelection() {
        // Return if we're over a UI element.
        if(EventSystem.current.IsPointerOverGameObject()) {
            return;
        }

        // Start selection drag.
        if(Input.GetMouseButtonDown(RIGHT_MOUSE_BUTTON)) {
            dragStartPosition = mousePositionCurrentFrame;
        }
        
        int startX = Mathf.FloorToInt(dragStartPosition.x);
        int endX = Mathf.FloorToInt(mousePositionCurrentFrame.x);
        int startY = Mathf.FloorToInt(dragStartPosition.y);
        int endY = Mathf.FloorToInt(mousePositionCurrentFrame.y);

        // If we're dragging down and/or left we need to invert these so that the loop works correctly.
        if(endX < startX) {
            int tmp = endX;
            endX = startX;
            startX = tmp;
        }
        if(endY < startY) {
            int tmp = endY;
            endY = startY;
            startY = tmp;
        }

        // CLean up old drag previews.
        while(dragPreviewGameObjects.Count > 0) {
            GameObject gameObject = dragPreviewGameObjects[0];
            dragPreviewGameObjects.RemoveAt(0);
            SimplePool.Despawn(gameObject);
        }

        if(Input.GetMouseButton(RIGHT_MOUSE_BUTTON)) {
            // Loop over all the tiles we've selected.
            for(int x = startX; x <= endX; x++) {
                for(int y = startY; y <= endY; y++) {
                    Tile tile = WorldController.Instance.World.GetTileAt(x, y);
                    if(tile != null) {
                        // Display the building hint on top of this tile position.
                        GameObject gameObject = SimplePool.Spawn(circleCursorPrefab, new Vector3(x, y, 0), Quaternion.identity);
                        gameObject.transform.SetParent(this.transform, true);
                        dragPreviewGameObjects.Add(gameObject);
                    }
                }
            }
        }

        // End selection drag.
        if(Input.GetMouseButtonUp(RIGHT_MOUSE_BUTTON)) {
            // Loop over all the tiles we've selected.
            for(int x = startX; x <= endX; x++) {
                for(int y = startY; y <= endY; y++) {
                    Tile tile = WorldController.Instance.World.GetTileAt(x, y);
                    
                    if(tile != null) {
                        if(buildModeIsObjects) {
                            WorldController.Instance.World.PlaceInstalledObject(buildModeObjectType, tile);
                        } else {
                            tile.Type = buildModeTile;
                        }
                    }
                }
            }

        }
    }

    public void SetModeBuildFloor() {
        buildModeIsObjects = false;
        buildModeTile = TileType.Floor;
    }

    public void SetModeBulldoze() {
        buildModeIsObjects = false;
        buildModeTile = TileType.Empty;
    }

    public void SetModeBuildInstalledObject(string objectType) {
        buildModeIsObjects = true;
        buildModeObjectType = objectType;
    }

}
