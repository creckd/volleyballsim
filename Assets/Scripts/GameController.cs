using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	private void Awake() {
		InputController.Instance.TapHappened += TapHappened;
	}

	private void TapHappened(int fingerID) {
	}
}
