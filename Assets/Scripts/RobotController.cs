using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour, IControllable {

    [SerializeField] private Instruction[] instructions;
    public float absVelocity;
    [HideInInspector] public Vector2 currentVelocity;
    public float approximateDistance = 0.05f;
    private int currentInstructionId;
    private Instruction currentInstruction;
    private Rigidbody2D body;
    private RemoteController remote;

    [HideInInspector] public bool isRewinded { get; set; }

    
    void Start() {
    	body = GetComponent<Rigidbody2D>();
        currentInstructionId = -1;
        isRewinded = false;
        remote = GameObject.Find("Player").GetComponent<RemoteController>();
        NextInstruction();
    }

    void NextInstruction() {
    	currentInstructionId += isRewinded ? -1 : 1;
    	if (currentInstructionId >= instructions.Length) {
    		currentInstructionId = 0;
    	}
        currentInstruction = instructions[currentInstructionId];
        Actions.Execute(gameObject, currentInstruction.name, currentInstruction.position);
    }

    
    void Update() {
        if (Mathf.Abs(currentInstruction.position - transform.position.x) < approximateDistance) {
    		currentVelocity = new Vector2(0f, 0f);
        	NextInstruction();
        }
    	body.velocity = currentVelocity;
    }

    public void Activate() {
    	// todo implement for further uses
    }

    void OnMouseOver() {
    	if (remote.isAiming) {
    		remote.AimFound();
    	}
    }

    void OnMouseExit() {
		remote.AimLost();
    }

    void OnMouseDown() {
    	if (Input.GetMouseButtonDown(0) & remote.isAiming) {
    		remote.Rewind(gameObject);
    	}
    }
}
