using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour, IControllable {

    [SerializeField] private Instruction[] instructions;
    public float absVelocity;
    public Vector2 currentVelocity;
    public float approximateDistance = 0.05f;
    private int currentInstructionId;
    private Instruction currentInstruction;
    private Rigidbody2D body;
    
    void Start() {
    	body = GetComponent<Rigidbody2D>();
        currentInstructionId = -1;
        NextInstruction();
    }

    void NextInstruction() {
    	currentInstructionId += 1;
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
}
