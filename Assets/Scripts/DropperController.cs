using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropperController : MonoBehaviour, IControllable {
	[SerializeField] private GameObject box;

    public void Activate() {
        Instantiate(box, transform.position, Quaternion.identity);
    	Debug.Log("Created box");
    }   
}
