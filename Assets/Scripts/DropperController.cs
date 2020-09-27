using UnityEngine;

public class DropperController : MonoBehaviour, IControllable {
	[SerializeField] private GameObject box;

    public void Activate(IOperator r_operator)
    {
        // only robots can activate dropper, so they will be loaded with a box
        if (r_operator is RobotController r_controller)
        {
            GameObject createdBox = Instantiate(box, transform.position, Quaternion.identity);
            createdBox.GetComponent<Rigidbody2D>().freezeRotation = true;
            r_controller.LoadCargo(box);
        }
    }
}
