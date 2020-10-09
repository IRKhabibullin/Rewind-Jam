using System.Collections;
using UnityEngine;

public enum ActionName {
	// Robot actions
	Activate
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
}
