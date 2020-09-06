using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour, IControllable {

    [SerializeField] private Instruction[] instructions;
    public float absVelocity;
    [HideInInspector] public Vector2 currentVelocity;
    public float approximateDistance = 0.15f;                           // how far from command position robot can act (i.e. from what distance robot can press button)
    private int currentInstructionId;
    private Instruction currentInstruction;
    private Rigidbody2D body;
    private RemoteController remote;
    private bool isWaiting;                                             // if robot is pending before executing another command
    private bool endedCommand;                                          // if robot finished current command
    public Animator animator;

    [HideInInspector] public bool isRewinded { get; set; }

    
    void Start() {
    	body = GetComponent<Rigidbody2D>();
    	animator = GetComponent<Animator>();
        currentInstructionId = -1;
        isRewinded = false;
        remote = GameObject.Find("Player").GetComponent<RemoteController>();
        endedCommand = true;
        isWaiting = false;
    }

    void NextInstruction()
    {
        currentInstructionId += isRewinded ? -1 : 1;
    	if (currentInstructionId >= instructions.Length) {
    		currentInstructionId = 0;
    	} else if (currentInstructionId < 0) {
    		currentInstructionId = instructions.Length - 1;
    	}
        currentInstruction = instructions[currentInstructionId];
        Actions.Execute(gameObject, currentInstruction);
        endedCommand = false;
        Debug.Log($"Executing command {currentInstruction.name} from position {transform.position.x}:{transform.position.y}");
    }

    
    void Update()
    {
        if (endedCommand & !isWaiting) {
        	NextInstruction();
    	} else if (!endedCommand && Mathf.Abs(currentInstruction.position - transform.position.x) < approximateDistance) {
            Debug.Log($"Ended command {currentInstruction.name} ({currentInstruction.position}) at {transform.position.x} ({transform.name})");
    		currentVelocity = new Vector2(0f, 0f);
    		animator.SetBool("Moving", false);
			endedCommand = true;
			StartCoroutine(WaitCommand(1f));
        }
    }

    private void FixedUpdate()
    {
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

    IEnumerator WaitCommand(float time) {
    	isWaiting = true;
		yield return new WaitForSeconds(time);
		isWaiting = false;
	}
}
