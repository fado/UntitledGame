using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {

    public GameObject circleCursor;

    Vector3 mousePositionLastFrame;
    Vector3 dragStartPosition;

    private const int RightMouseButton = 0;
    private const int LeftMouseButton = 1;
    private const int MiddleMouseButton = 2;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
        Vector3 mousePositionCurrentFrame = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Need to set the z-level to zero or the cursor won't be visible.
        mousePositionCurrentFrame.z = 0;

        // Update the circle cursor position.
        Tile tileUnderMouse = GetTileAtWorldCoord(mousePositionCurrentFrame);
        if(tileUnderMouse != null) {
            circleCursor.SetActive(true);
            Vector3 cursorPosition = new Vector3(tileUnderMouse.X, tileUnderMouse.Y, 0);
            circleCursor.transform.position = cursorPosition;
        } else {
            // Hide the cursor when it's out of range.
            circleCursor.SetActive(false);
        }

        // Start selection drag.
        if(Input.GetMouseButtonDown(RightMouseButton)) {
            dragStartPosition = mousePositionCurrentFrame;
        }
        
        // End selection drag.
        if(Input.GetMouseButtonUp(RightMouseButton)) {
            int startX = Mathf.FloorToInt(dragStartPosition.x);
            int endX = Mathf.FloorToInt(mousePositionCurrentFrame.x);

            // If we're dragging down and/or left we need to invert these so that the loop works correctly.
            if(endX < startX) {
                int tmp = endX;
                endX = startX;
                startX = tmp;
            }
            
            int startY = Mathf.FloorToInt(dragStartPosition.y);
            int endY = Mathf.FloorToInt(mousePositionCurrentFrame.y);

            // If we're dragging down and/or left we need to invert these so that the loop works correctly.
            if(endY < startY) {
                int tmp = endY;
                endY = startY;
                startY = tmp;
            }

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
        
        // Handle camera dragging.
        if(Input.GetMouseButton(LeftMouseButton) || Input.GetMouseButton(MiddleMouseButton)) {
            Vector3 diff = mousePositionLastFrame - mousePositionCurrentFrame;
            Camera.main.transform.Translate(diff);
        }

        mousePositionLastFrame = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Need to set the z-level to zero or the cursor won't be visible.
        mousePositionLastFrame.z = 0;
    }

    Tile GetTileAtWorldCoord(Vector3 coord) {
        int x = Mathf.FloorToInt(coord.x);
        int y = Mathf.FloorToInt(coord.y);

        return WorldController.Instance.World.GetTileAt(x, y);
    }
}
