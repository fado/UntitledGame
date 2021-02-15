using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {

    public GameObject circleCursor;

    Vector3 mousePositionLastFrame;

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
        circleCursor.transform.position = mousePositionCurrentFrame;
        
        // Handle camera dragging.
        if(Input.GetMouseButton(LeftMouseButton) || Input.GetMouseButton(MiddleMouseButton)) {
            Vector3 diff = mousePositionLastFrame - mousePositionCurrentFrame;
            Camera.main.transform.Translate(diff);
        }

        mousePositionLastFrame = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Need to set the z-level to zero or the cursor won't be visible.
        mousePositionLastFrame.z = 0;
    }
}
