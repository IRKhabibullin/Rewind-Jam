using UnityEngine;

public class ButtonController : MonoBehaviour, IPressable {
    public IControllable mechanism { get; private set; }

    void Start() {
        mechanism = transform.parent.gameObject.GetComponent<IControllable>();
    }

    public void Activate(IOperator r_operator) {
    	mechanism.Activate(r_operator);
    }
}
