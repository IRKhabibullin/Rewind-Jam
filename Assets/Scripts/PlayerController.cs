using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private Rigidbody2D body;
	private Vector3 velocity = Vector3.zero;
	[SerializeField] private float m_MovementSmoothing = 0.5f;
	[SerializeField] private float runSpeed;

    void Start() {
        body = GetComponent<Rigidbody2D>();
    }

    void Update() {
        float horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

		// if (Input.GetKeyDown(KeyCode.Space)) {
		// 	jump = true;
		// }
		Move(horizontalMove);
    }

    void Move(float direction) {

    	// Move the character by finding the target velocity
		Vector3 targetVelocity = new Vector2(direction, body.velocity.y);
		// And then smoothing it out and applying it to the character
		body.velocity = Vector3.SmoothDamp(body.velocity, targetVelocity, ref velocity, m_MovementSmoothing);
    }
}
