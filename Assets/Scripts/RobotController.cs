using System.Collections;
using UnityEngine;

public class RobotController : MonoBehaviour, IOperator, IRewindable {

    // TODO need to make rewindable objects vulnurable (written with bad practices) so remote can access them as it is indeed hacking tool
    [SerializeField] private RInstruction[] instructions;
    [SerializeField] private float absVelocity;
    private Rigidbody2D body;
    private Animator animator;
    private Transform grabPosition;
    private RInstruction currentInstruction;
    private int currentInstructionId;
    private float faceDirection;
    private Vector2 currentVelocity;

    private Transform grabbedObject;
    private RemoteController remote;

    public float approximationDistance = 0.15f;                // how far from command position robot can act (i.e. from what distance robot can press button)
    public bool isExecutingCommand;                            // if robot executing command
    public Coroutine instructionCoroutine;

    public bool isRewinded { get; set; }

    
    void Start() {
    	body = GetComponent<Rigidbody2D>();
    	animator = GetComponent<Animator>();
        currentInstructionId = -1;
        isRewinded = false;
        remote = GameObject.Find("Player").GetComponent<RemoteController>();
        isExecutingCommand = false;
        grabbedObject = null;
        NextInstruction();
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
        isExecutingCommand = true;
        RActions.Execute(gameObject, currentInstruction);
    }

    
    void Update()
    {
        if (!isExecutingCommand)
        {
            isExecutingCommand = true;
            StartCoroutine(ProcessNextCommand());
        }
    }

    private void FixedUpdate()
    {
        if (body.velocity.x == 0 && currentVelocity.x != 0)
        {
            animator.SetBool("Moving", true);
        } else if (body.velocity.x != 0 && currentVelocity.x == 0)
        {
            animator.SetBool("Moving", false);
        }
        body.velocity = currentVelocity;
        if (grabbedObject != null)
        {
            grabbedObject.position = grabPosition.position;
        }
    }

    public void Activate() {
    	// todo implement for further uses
    }

    public void LoadCargo(GameObject cargo)
    {
        grabbedObject = cargo.transform;
    }

    public void Move(Vector2 destination)
    {
        if (Mathf.Abs(destination.y - transform.position.y) >= 1f)
        {
            Debug.LogError($"Robot can not move to position {destination} from {transform.position}");
            return;
        }

        faceDirection = Mathf.Sign(destination.x - transform.position.x);
        transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, -faceDirection));
        currentVelocity = new Vector2(absVelocity * faceDirection, 0f);
    }

    public void Stop()
    {
        currentVelocity = Vector2.zero;
    }

    /// <summary> Checks if object is standing near the passed point </summary>
    public bool NearThePoint(Vector2 point)
    {
        return Mathf.Abs(point.x - transform.position.x) <= approximationDistance;
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

    IEnumerator ProcessNextCommand()
    {
        yield return new WaitForSeconds(1f);
        NextInstruction();
    }

    public void Rewind()
    {
        StopCoroutine(instructionCoroutine);
        NextInstruction();
    }
}
