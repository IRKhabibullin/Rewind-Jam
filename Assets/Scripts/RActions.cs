using System.Collections;
using UnityEngine;

public enum ActionName {
	// Robot actions
	Move,
	Activate,

	// Crane actions
	Lift,
	MoveSide,
	Grab,
	Release
}

[System.Serializable]
public class RInstruction {
	public ActionName name;
	public float position;
	public GameObject target;
}

public class RActions {

	public static void Execute(GameObject executor, RInstruction instruction) {
		switch(instruction.name) {
			case ActionName.Move:
				//RobotController _rc = executor.GetComponent<RobotController>();
				//_rc.StartCoroutine(Move(_rc, instruction));
				break;
			case ActionName.Activate:
				RobotController _rc = executor.GetComponent<RobotController>();
				_rc.instructionCoroutine = _rc.StartCoroutine(Activate(_rc, instruction));
				break;
			case ActionName.Lift:
				Lift(executor, instruction.position);
				break;
			case ActionName.MoveSide:
				MoveSide(executor, instruction.position);
				break;
			case ActionName.Grab:
				Grab(executor, instruction.position);
				break;
			case ActionName.Release:
				Release(executor);
				break;
		}
	}

	private static IEnumerator Move(RobotController _rc, RInstruction instruction)
	{
		_rc.Move(instruction.target.transform.position);

        yield return new WaitUntil(() => _rc.NearThePoint(instruction.target.transform.position));

		yield return new WaitForSeconds(1f);

		instruction.target.GetComponent<ButtonController>().Activate();

		//float direction = Mathf.Sign(position - executor.transform.position.x);
		//executor.transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, -direction));
		//RobotController controller = executor.GetComponent<RobotController>();
		//yield return new WaitForSeconds(1f);
		//controller.currentVelocity = new Vector2(controller.absVelocity * direction, 0f);
  //      executor.GetComponent<Rigidbody2D>().velocity = controller.currentVelocity;
  //      controller.animator.SetBool("Moving", true);
	}

	private static IEnumerator Activate(RobotController _rc, RInstruction instruction)
	{
		_rc.Move(instruction.target.transform.position);

		yield return new WaitUntil(() => _rc.NearThePoint(instruction.target.transform.position));

		_rc.currentVelocity = new Vector2(0f, 0f);

		yield return new WaitForSeconds(1f);

		instruction.target.GetComponent<ButtonController>().Activate();
		_rc.executingCommand = false;
	}

	private static void Lift(GameObject executor, float position) {
        CraneController controller = executor.GetComponent<CraneController>();
        float direction = Mathf.Sign(position - controller.head.transform.position.y);
        controller.currentVelocity = new Vector2(0f, controller.absVelocity * direction);
        controller.head.GetComponent<Rigidbody2D>().velocity = controller.currentVelocity;
	}

	private static void MoveSide(GameObject executor, float position) {
        CraneController controller = executor.GetComponent<CraneController>();
        float direction = Mathf.Sign(position - controller.head.transform.position.x);
        controller.currentVelocity = new Vector2(controller.absVelocity * direction, 0f);
        controller.head.GetComponent<Rigidbody2D>().velocity = controller.currentVelocity;
	}

	private static void Grab(GameObject executor, float position) {
		CraneController controller = executor.GetComponent<CraneController>();
		bool foundButton = false;
		Collider2D[] colliders = Physics2D.OverlapCircleAll(controller.head.transform.position, controller.approximateDistance);
		foreach (Collider2D grabbable in colliders) {
			if (grabbable.tag == "Grabbable") {
				controller.grabbedObject = grabbable.gameObject.transform;
				foundButton = true;
			}
		}
		if (!foundButton)
		{
			Debug.Log($"Any object from {colliders.Length} grabbables not reachable. Crane is in {executor.transform.position.x} ({executor.name})");
		}
	}

	private static void Release(GameObject executor) {
		// release action position must be for <grabPosition> lower than previous action
        executor.GetComponent<CraneController>().grabbedObject = null;
	}
}
