using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour, IPressable {

	private IPressable mechanism;

    void Start() {
        mechanism = transform.parent.gameObject.GetComponent<IPressable>();
    }

    public void Activate() {
    	mechanism.Activate();
    }
}
