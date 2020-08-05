using System.Collections;
using System.Collections.Generic;
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

	public static void Execute(GameObject executor, ActionName action, float position) {
		switch(action) {
			case ActionName.Move:
				Move(executor, position);
				break;
			case ActionName.Activate:
				Activate(executor);
				break;
			case ActionName.Lift:
				Lift(executor, position);
				break;
			case ActionName.MoveSide:
				MoveSide(executor, position);
				break;
			case ActionName.Grab:
				Grab(executor, position);
				break;
			case ActionName.Release:
				Release(executor);
				break;
		}
	}

	private static void Move(GameObject executor, float position) {
        float direction = Mathf.Sign(position - executor.transform.position.x);
        RobotController controller = executor.GetComponent<RobotController>();
        controller.currentVelocity = new Vector2(controller.absVelocity * direction, 0f);
        executor.GetComponent<Rigidbody2D>().velocity = controller.currentVelocity;
	}

	private static void Activate(GameObject executor) {
		GameObject[] buttons = GameObject.FindGameObjectsWithTag("Button");
        RobotController controller = executor.GetComponent<RobotController>();
		foreach (GameObject button in buttons) {
			if (Mathf.Abs(button.transform.position.x - executor.transform.position.x) < controller.approximateDistance) {
				button.GetComponent<ButtonController>().Activate();
			}
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
		GameObject[] grabbables = GameObject.FindGameObjectsWithTag("Grabbable");
		foreach (GameObject grabbable in grabbables) {
			if (Mathf.Abs(grabbable.transform.position.x - controller.head.transform.position.x) < controller.approximateDistance) {
				controller.grabbedObject = grabbable.transform;
			}
		}
	}

	private static void Release(GameObject executor) {
        executor.GetComponent<CraneController>().grabbedObject = null;
	}
}
