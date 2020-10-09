using System;
using System.Collections;
using UnityEngine;


public class CraneController : MonoBehaviour, IControllable
{
    public enum CraneAction { PickUp, Deliver }

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
    [SerializeField] private float absVelocity = 0f;
    [SerializeField] private float approximationDistance = 0.5f;
    private Vector3 grabPosition = new Vector3(0f, -0.75f, 0f);
    private Vector2 currentVelocity;
    private bool finishedInstruction;

    public CraneInstructions instructions;
    public Coroutine instructionCoroutine;

    void Start()
    {
        head = transform.Find("CraneHead").gameObject;
        activated = false;
    }

    void Update()
    {
        if (activated)
        {
            if (finishedInstruction)
            {
                NextInstruction();
            }
        }
    }

    private void FixedUpdate()
    {
        if (activated)
        {
            head.GetComponent<Rigidbody2D>().velocity = currentVelocity;
            if (grabbedObject != null)
            {
                grabbedObject.position = head.transform.position + grabPosition;
            }
        }
    }

    public void Activate(IOperator r_operator)
    {
        activated = true;
        NextInstruction();
    }

    void NextInstruction()
    {
        CraneInstruction next = instructions.Next();
        switch (next.name)
        {
            case CraneAction.PickUp:
                instructionCoroutine = StartCoroutine(PickUp(next.position));
                break;
            case CraneAction.Deliver:
                instructionCoroutine = StartCoroutine(Deliver(next.position));
                break;
        }
        finishedInstruction = false;
    }

    public bool NearThePoint(Vector2 point)
    {
        return Mathf.Abs(point.x - head.transform.position.x) <= approximationDistance;
    }

    #region head control methods
    public void MoveHead(Vector2 position)
    {
        if (position.x != head.transform.position.x && position.y != head.transform.position.y)
        {
            Debug.LogWarning($"Head of the crane can't reach point {position}");
            return;
        }
        float direction;
        if (Mathf.Abs(position.x - head.transform.position.x) < approximationDistance)
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
            Debug.Log($"Any object from {colliders.Length} grabbables not reachable. Crane is in {transform.position}");
        }
    }

    public void Release()
    {
        grabbedObject = null;
    } 
    #endregion

    #region action handlers
    private IEnumerator MoveHeadToPosition(Vector2 position)
    {
        Vector2 next_position = new Vector2(head.transform.position.x, transform.position.y);
        MoveHead(next_position);

        yield return new WaitUntil(() => NearThePoint(next_position));

        next_position = new Vector2(position.x, head.transform.position.y);
        MoveHead(next_position);

        yield return new WaitUntil(() => NearThePoint(next_position));

        next_position = new Vector2(head.transform.position.x, position.y);
        MoveHead(next_position);

        yield return new WaitUntil(() => NearThePoint(next_position));
    }

    private IEnumerator PickUp(Vector2 position)
    {
        yield return StartCoroutine(MoveHeadToPosition(position));
        Grab();
        finishedInstruction = true;
    }

    private IEnumerator Deliver(Vector2 position)
    {
        yield return StartCoroutine(MoveHeadToPosition(position));
        Release();
        finishedInstruction = true;
    }
    #endregion

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
}
