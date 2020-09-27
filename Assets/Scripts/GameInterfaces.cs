public interface IPressable {
	// Can be activated only by IOperator implemented objects
	IControllable mechanism { get; }

	void Activate(IOperator r_operator);
}

public interface IControllable {
	// Can be activated only by IPressable implemented objects
	void Activate(IOperator r_operator);
}

public interface IRewindable
{
	// Can be activate only by IRewind implemented objects
	bool isRewinded { get; set; }

	void Rewind();
}

public interface IOperator
{

}

public interface IRewind
{

}
