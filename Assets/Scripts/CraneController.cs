using System;
using System.Collections;
using UnityEngine;


public class CraneController : MonoBehaviour, IControllable
{
    public enum CraneAction { Grab, Deliver }

    [Serializable]
    public class CraneInstruction
    {
        public CraneAction name;
        public Vector2 position;
        public GameObject target;
    }

    [Serializable]
    public class CraneInstructions : CyclicLinkedList<CraneInstruction> { }

    public GameObject head;
    private Transform grabbedObject;
    private bool activated;
    private float absVelocity;
    private float approximationDistance = 0.5f;
    private Vector3 grabPosition = new Vector3(0f, -0.75f, 0f);
    private Vector2 currentVelocity;

    public CraneInstructions instructions;
    public Coroutine instructionCoroutine;

    void Start()
    {
        head = transform.Find("CraneHead").gameObject;
        activated = false;
    }

    void Update()
    {
        //     if (activated) {
        //     	float currentPosition;
        //     	if (currentInstruction.name == CraneAction.MoveSide) {
        //     		currentPosition = head.transform.position.x;
        // 		} else {
        //     		currentPosition = head.transform.position.y;
        //  		if (currentInstruction.name == CraneAction.Grab || currentInstruction.name == CraneAction.Release) {
        //  			currentPosition += grabPosition.y;
        //  		}
        // 		}
        //if (Mathf.Abs(currentInstruction.position - currentPosition) < approximationDistance) {
        // 			currentVelocity = new Vector2(0f, 0f);
        //     		NextInstruction();
        //         }
        //         if (grabbedObject != null)
        //         {
        //             grabbedObject.position = head.transform.position + grabPosition;
        //         }
        //     }
    }

    private void FixedUpdate()
    {
        head.GetComponent<Rigidbody2D>().velocity = currentVelocity;
    }

    public void Activate(IOperator r_operator)
    {
        activated = true;
        NextInstruction();
    }

    public bool NearThePoint(Vector2 point)
    {
        return Mathf.Abs(point.x - head.transform.position.x) <= approximationDistance;
    }

    public void MoveHead(Vector2 position)
    {
        if (position.x != head.transform.position.x && position.y != head.transform.position.y)
        {
            Debug.LogWarning($"Head of the crane can't reach point {position}");
            return;
        }
        float direction = 0;
        if (position.x == head.transform.position.x)
        {
            direction = Mathf.Sign(position.y - head.transform.position.y);
            currentVelocity = Vector2.up * direction * absVelocity;
            return;
        }
        direction = Mathf.Sign(position.x - head.transform.position.x);
        currentVelocity = Vector2.right * direction * absVelocity;
    }

    public void Grab()
    {
        bool foundObject = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(head.transform.position, approximationDistance);
        foreach (Collider2D grabbable in colliders)
        {
            if (grabbable.tag == "Grabbable")
            {
                grabbedObject = grabbable.gameObject.transform;
                foundObject = true;
            }
        }
        if (!foundObject)
        {
            Debug.Log($"Any object from {colliders.Length} grabbables not reachable. Crane is in {transform.position.x} ({name})");
        }
    }

    public void Release()
    {
        grabbedObject = null;
    }

    void NextInstruction()
    {
        //   CraneInstruction next = instructions.Next();
        //currentInstructionId += 1;
        //if (currentInstructionId >= instructions.Length) {
        //	currentInstructionId = -1;
        //	activated = false;
        //	return;
        //}
        //   currentInstruction = instructions[currentInstructionId];
        //   RActions.Execute(gameObject, currentInstruction);
    }

    void OnMouseOver()
    {
        //if (remote.isAiming) {
        //    remote.AimFound();
        //}
    }

    void OnMouseExit()
    {
        //remote.AimLost();
    }

    void OnMouseDown()
    {
        //if (Input.GetMouseButtonDown(0) & remote.isAiming) {
        //    remote.Rewind(gameObject);
        //}
    }

    private static IEnumerator MoveHeadToPosition(CraneController controller, Vector2 position)
    {
        Vector2 next_position = new Vector2(controller.head.transform.position.x, controller.transform.position.y);
        controller.MoveHead(next_position);

        yield return new WaitUntil(() => controller.NearThePoint(next_position));

        next_position = new Vector2(position.x, controller.head.transform.position.y);
        controller.MoveHead(next_position);

        yield return new WaitUntil(() => controller.NearThePoint(next_position));

        next_position = new Vector2(controller.head.transform.position.x, position.y);
        controller.MoveHead(next_position);

        yield return new WaitUntil(() => controller.NearThePoint(next_position));
    }

    private static IEnumerator Grab(GameObject executor, Vector2 position)
    {
        CraneController controller = executor.GetComponent<CraneController>();
        yield return controller.StartCoroutine(MoveHeadToPosition(controller, position));
        controller.Grab();
    }

    private static IEnumerator Deliver(GameObject executor, Vector2 position)
    {
        CraneController controller = executor.GetComponent<CraneController>();
        yield return controller.StartCoroutine(MoveHeadToPosition(controller, position));
        controller.Release();
    }
}
