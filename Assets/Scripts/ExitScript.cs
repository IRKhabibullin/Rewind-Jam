using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitScript : MonoBehaviour {
    private BoxCollider2D mCollider;

    void Start() {
        mCollider = GetComponent<BoxCollider2D>();
    }

    void Update() {
        
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Player") {
            Debug.Log("Finished!");
        }
    }
}