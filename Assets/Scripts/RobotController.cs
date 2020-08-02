using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour {
	public enum ActionName {
		Move
	}
	[System.Serializable]
   	public class Action {
       	public ActionName name;
       	public float position;
    }

    [SerializeField] private Action[] actions;
    [SerializeField] private float absVelocity;
    private Vector2 currentVelocity;
    private int currentActionId;
    private Action currentAction;
    private Rigidbody2D body;
    [SerializeField] private float approximateDistance = 0.1f;
    
    void Start() {
    	body = GetComponent<Rigidbody2D>();
        currentActionId = 0;
        StartAction();
    }

    void StartAction() {
        currentAction = actions[currentActionId];
        float direction = Mathf.Sign(currentAction.position - transform.position.x);
        currentVelocity = new Vector2(absVelocity * direction, 0f);
        body.velocity = currentVelocity;
    }

    
    void Update() {
        if (Mathf.Abs(currentAction.position - transform.position.x) < approximateDistance) {
        	Debug.Log($"Reached point {currentAction.position}");
        	currentActionId += 1;
        	if (currentActionId >= actions.Length) {
        		currentActionId = 0;
        	}
        	StartAction();
        }
    	body.velocity = currentVelocity;
    }
}
