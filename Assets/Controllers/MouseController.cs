using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {

    public GameObject circleCursorPrefab;

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
    }

    void UpdateDragSelection() {
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
            Destroy(gameObject);
        }

        if(Input.GetMouseButton(RIGHT_MOUSE_BUTTON)) {
            // Loop over all the tiles we've selected.
            for(int x = startX; x <= endX; x++) {
                for(int y = startY; y <= endY; y++) {
                    Tile tile = WorldController.Instance.World.GetTileAt(x, y);
                    if(tile != null) {
                        // Display the building hint on top of this tile position.
                        GameObject gameObject = (GameObject)Instantiate(circleCursorPrefab, new Vector3(x, y, 0), Quaternion.identity);
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
                        tile.Type = Tile.TileType.Floor;
                    }
                }
            }

        }
    }

    // void UpdateCursor() {
    //     // Update the circle cursor position.
    //     Tile tileUnderMouse = WorldController.Instance.GetTileAtWorldCoord(mousePositionCurrentFrame);
    //     if(tileUnderMouse != null) {
    //         circleCursor.SetActive(true);
    //         Vector3 cursorPosition = new Vector3(tileUnderMouse.X, tileUnderMouse.Y, 0);
    //         circleCursor.transform.position = cursorPosition;
    //     } else {
    //         // Hide the cursor when it's out of range.
    //         circleCursor.SetActive(false);
    //     }
    // }

}
