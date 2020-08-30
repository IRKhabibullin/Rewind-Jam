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
    private bool isWaiting;
    private bool endedCommand;
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
        Debug.Log($"Robot start position: {transform.position}");
    }

    void NextInstruction()
    {
        Debug.Log($"Robot position 1: {transform.position}");
        currentInstructionId += isRewinded ? -1 : 1;
    	if (currentInstructionId >= instructions.Length) {
    		currentInstructionId = 0;
    	} else if (currentInstructionId < 0) {
    		currentInstructionId = instructions.Length - 1;
    	}
        currentInstruction = instructions[currentInstructionId];
        Actions.Execute(gameObject, currentInstruction.name, currentInstruction.position);
        Debug.Log($"Robot position 2: {transform.position}");
        endedCommand = false;
        Debug.Log($"Executing command {currentInstruction.name} from position {transform.position.x}:{transform.position.y}");
    }

    
    void Update()
    {
        Debug.Log($"Robot position 3: {transform.position}");
        if (endedCommand & !isWaiting) {
        	NextInstruction();
    	} else if (Mathf.Abs(currentInstruction.position - transform.position.x) < approximateDistance) {
    		currentVelocity = new Vector2(0f, 0f);
    		animator.SetBool("Moving", false);
			endedCommand = true;
			StartCoroutine(WaitCommand(1f));
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

    IEnumerator WaitCommand(float time) {
    	isWaiting = true;
		yield return new WaitForSeconds(time);
		isWaiting = false;
	}
}
