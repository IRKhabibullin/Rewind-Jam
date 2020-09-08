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
public class Instruction {
	public ActionName name;
	public float position;
}

public class Actions {

	public static void Execute(GameObject executor, Instruction instruction) {
		switch(instruction.name) {
			case ActionName.Move:
				executor.GetComponent<RobotController>().StartCoroutine(Move(executor, instruction.position));
				break;
			case ActionName.Activate:
				executor.GetComponent<RobotController>().StartCoroutine(Activate(executor));
				Activate(executor);
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

	private static IEnumerator Move(GameObject executor, float position)
	{
		float direction = Mathf.Sign(position - executor.transform.position.x);
        executor.transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, -direction));
        RobotController controller = executor.GetComponent<RobotController>();
		yield return new WaitForSeconds(1f);
		controller.currentVelocity = new Vector2(controller.absVelocity * direction, 0f);
        executor.GetComponent<Rigidbody2D>().velocity = controller.currentVelocity;
        controller.animator.SetBool("Moving", true);
	}

	private static IEnumerator Activate(GameObject executor) {
		GameObject[] buttons = GameObject.FindGameObjectsWithTag("Button");
        RobotController controller = executor.GetComponent<RobotController>();
        bool foundButton = false;
		foreach (GameObject button in buttons) {
			if (Mathf.Abs(button.transform.position.x - executor.transform.position.x) < controller.approximateDistance)
			{
				yield return new WaitForSeconds(1f);
				button.GetComponent<ButtonController>().Activate();
				foundButton = true;
			}
		}
		if (!foundButton) {
			Debug.Log($"Any button from {buttons.Length} buttons not reachable. Robot is in {executor.transform.position.x} ({executor.name})");
		}
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
