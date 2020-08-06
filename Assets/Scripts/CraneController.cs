using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneController : MonoBehaviour, IControllable {

    [SerializeField] private Instruction[] instructions;
    public GameObject head;
    private bool activated;
    private int currentInstructionId;
    private Instruction currentInstruction;
    public Transform grabbedObject;
    private Vector3 grabPosition;

    public float absVelocity;
    public Vector2 currentVelocity;
    public float approximateDistance = 0.05f;

    void Start() {
        head = transform.Find("CraneHead").gameObject;
        activated = false;
        currentInstructionId = -1;
        grabPosition = new Vector3(0f, -0.75f, 0f);
    }

    
    void Update() {
        if (activated) {
        	float currentPosition;
        	if (currentInstruction.name == ActionName.MoveSide) {
        		currentPosition = head.transform.position.x;
    		} else {
        		currentPosition = head.transform.position.y;
	    		if (currentInstruction.name == ActionName.Grab || currentInstruction.name == ActionName.Release) {
	    			currentPosition += grabPosition.y;
	    		}
    		}
			if (Mathf.Abs(currentInstruction.position - currentPosition) < approximateDistance) {
    			currentVelocity = new Vector2(0f, 0f);
        		NextInstruction();
        	}
    		head.GetComponent<Rigidbody2D>().velocity = currentVelocity;
    		if (grabbedObject != null) {
    			grabbedObject.position = head.transform.position + grabPosition;
    		}
        }
    }

    public void Activate() {
    	activated = true;
    	NextInstruction();
    }

    void NextInstruction() {
    	currentInstructionId += 1;
    	if (currentInstructionId >= instructions.Length) {
    		currentInstructionId = -1;
    		activated = false;
    		return;
    	}
        currentInstruction = instructions[currentInstructionId];
        Actions.Execute(gameObject, currentInstruction.name, currentInstruction.position);
    }
}
