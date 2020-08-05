using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumerController : MonoBehaviour {

	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.tag == "Grabbable" & collision.gameObject.name != "Player") {
			Destroy(collision.gameObject);
		}
	}
}
