using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour, IPressable {

	private IControllable mechanism;

    void Start() {
        mechanism = transform.parent.gameObject.GetComponent<IControllable>();
    }

    public void Activate() {
    	mechanism.Activate();
    }
}
