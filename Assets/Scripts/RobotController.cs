using System.Collections;
using UnityEngine;

public class RobotController : MonoBehaviour, IControllable {

    [SerializeField] private RInstruction[] instructions;
    public float absVelocity;
    [HideInInspector] public Vector2 currentVelocity;
    public float approximateDistance = 0.15f;                           // how far from command position robot can act (i.e. from what distance robot can press button)
    private int currentInstructionId;
    private RInstruction currentInstruction;
    private Rigidbody2D body;
    private RemoteController remote;
    public bool executingCommand;                                       // if robot executing command
    public Animator animator;
    public Coroutine instructionCoroutine;

    [HideInInspector] public bool isRewinded { get; set; }

    
    void Start() {
    	body = GetComponent<Rigidbody2D>();
    	animator = GetComponent<Animator>();
        currentInstructionId = -1;
        isRewinded = false;
        remote = GameObject.Find("Player").GetComponent<RemoteController>();
        executingCommand = false;
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
        executingCommand = true;
        RActions.Execute(gameObject, currentInstruction);
        Debug.Log($"Executing command {currentInstruction.name} from position {transform.position.x}:{transform.position.y}");
    }

    
    void Update()
    {
        if (!executingCommand)
        {
            executingCommand = true;
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
    }

    public void Activate() {
    	// todo implement for further uses
    }

    public void Move(Vector2 destination)
    {
        if (Mathf.Abs(destination.y - transform.position.y) >= 1f)
        {
            Debug.LogError($"Robot can not move to position {destination} from {transform.position}");
            return;
        }

        float direction = Mathf.Sign(destination.x - transform.position.x);
        transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, -direction));
        currentVelocity = new Vector2(absVelocity * direction, 0f);
    }

    /// <summary> Checks if object is standing near the passed point </summary>
    public bool NearThePoint(Vector2 point)
    {
        return Mathf.Abs(point.x - transform.position.x) <= approximateDistance;
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
