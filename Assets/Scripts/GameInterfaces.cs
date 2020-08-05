using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPressable {
	// Robots can activate, player/remote can't
	void Activate();
}

public interface IControllable {
	// player/remote/buttons can activate, robots can't

	// float currentVelocity { get; set; }
	// float absVelocity { get; set; }
	// float approximateDistance { get; set; }

	void Activate();
}
