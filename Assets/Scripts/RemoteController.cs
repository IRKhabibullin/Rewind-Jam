using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteController : MonoBehaviour {

	public bool isAiming;
	public Texture2D aimCursor;
    public Texture2D correctAimCursor;

    
    void Start() {
        isAiming = false;
    }

    
    void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
        	SetAim();
        } else if (isAiming & Input.GetMouseButtonDown(0)) {
        	// Rewind();
        }
    }

    void SetAim() {
    	isAiming = true;
    	Cursor.SetCursor(aimCursor, Vector2.zero, CursorMode.Auto);
    }

    public void AimFound() {
        Cursor.SetCursor(correctAimCursor, Vector2.zero, CursorMode.Auto);
    }

    public void AimLost() {
        if (isAiming) {
            Cursor.SetCursor(aimCursor, Vector2.zero, CursorMode.Auto);
        } else {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);    
        }
    }

    void RemoveAim() {
    	isAiming = false;
    	Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

  //   public void Rewind(GameObject rewindTarget) {
		// Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		// RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero, 0f);
		// if (hit.collider != null) {
		// 	GameObject clickedObject = hit.collider.gameObject;
		// 	if (clickedObject.GetComponent<IControllable>() != null) {
		// 		Debug.Log($"Clicked on a {clickedObject.name}");
		// 	}
		// }
  //   }
    public void Rewind(GameObject rewindTarget) {
        rewindTarget.GetComponent<IRewindable>().Rewind();
        RemoveAim();
    }
}
