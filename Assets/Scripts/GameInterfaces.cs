using System.Collections.Generic;

public interface IPressable {
	// Can be activated only by IOperator implemented objects
	IControllable mechanism { get; }

	void Activate(IOperator r_operator);
}

public interface IControllable {
	// Can be activated only by IPressable implemented objects
	void Activate(IOperator r_operator);
}

public interface IOperator {

}

public class CyclicLinkedList<T> : List<T>
{
	public T[] data;
	private int currentItem;

	public CyclicLinkedList() {
		currentItem = 0;
	}

	public int Count { get; }

	public T GetCurrent()
    {
		return data[currentItem];
    }

	public T Next()
    {
		currentItem += 1;
		if (currentItem == data.Length)
        {
			currentItem = 0;
        }
		return data[currentItem];
    }

	public T Prev()
    {
		currentItem -= 1;
		if (currentItem < 0)
        {
			currentItem = data.Length - 1;
        }
		return data[currentItem];
    }
}