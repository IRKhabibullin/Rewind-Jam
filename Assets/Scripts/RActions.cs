using System.Collections;
using UnityEngine;

public enum ActionName {
	// Robot actions
	Activate,

	// Crane actions
	Lift,
	MoveSide,
	Grab,
	Deliver
}

[System.Serializable]
public class RInstruction {
	public ActionName name;
	public Vector2 position;
	public GameObject target;
}

public class RActions {

	public static void Execute(GameObject executor, RInstruction instruction) {
		switch (instruction.name) {
			case ActionName.Activate:
				RobotController _rc = executor.GetComponent<RobotController>();
				_rc.instructionCoroutine = _rc.StartCoroutine(Activate(_rc, instruction));
				break;
			//case ActionName.Lift:
			//	Lift(executor, instruction.position);
			//	break;
			//case ActionName.MoveSide:
			//	MoveSide(executor, instruction.position);
			//	break;
			case ActionName.Grab:
				CraneController _cc = executor.GetComponent<CraneController>();
				_cc.instructionCoroutine = _cc.StartCoroutine(Grab(executor, instruction.position));
				break;
			case ActionName.Deliver:
				//CraneController _cc = executor.GetComponent<CraneController>();
				//_cc.instructionCoroutine = _cc.StartCoroutine(Deliver(executor, instruction.position));
				break;
		}
	}

	private static IEnumerator Activate(RobotController _rc, RInstruction instruction)
	{
		_rc.Move(instruction.target.transform.position);

		yield return new WaitUntil(() => _rc.NearThePoint(instruction.target.transform.position));

		_rc.Stop();

		yield return new WaitForSeconds(1f);

		instruction.target.GetComponent<ButtonController>().Activate(_rc);
		_rc.EndInstruction();
	}

	private static void Lift(GameObject executor, float position) {
		CraneController controller = executor.GetComponent<CraneController>();
		//float direction = Mathf.Sign(position - controller.head.transform.position.y);
		//controller.currentVelocity = new Vector2(0f, controller.absVelocity * direction);
		//controller.head.GetComponent<Rigidbody2D>().velocity = controller.currentVelocity;
	}

	private static void MoveSide(GameObject executor, float position) {
		CraneController controller = executor.GetComponent<CraneController>();
		//float direction = Mathf.Sign(position - controller.head.transform.position.x);
		//controller.currentVelocity = new Vector2(controller.absVelocity * direction, 0f);
		//controller.head.GetComponent<Rigidbody2D>().velocity = controller.currentVelocity;
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

	private static IEnumerator Grab(GameObject executor, Vector2 position) {
		CraneController controller = executor.GetComponent<CraneController>();
		yield return controller.StartCoroutine(MoveHeadToPosition(controller, position));
		controller.Grab();
    }

	private static IEnumerator Deliver(GameObject executor, Vector2 position) {
		CraneController controller = executor.GetComponent<CraneController>();
		yield return controller.StartCoroutine(MoveHeadToPosition(controller, position));
		controller.Release();
	}
}
