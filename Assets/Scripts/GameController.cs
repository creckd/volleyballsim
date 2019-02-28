using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	private static GameController instance = null;
	public static GameController Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<GameController>();
			}
			return instance;
		}
	}

	public Action<float, Vector3> RefreshTapIndicator = delegate { };

	private int currentFingerID = -1;
	private float currentTapCircleRadius = 0f;

	private void Awake() {
		Application.targetFrameRate = 300;
		InputController.Instance.TapHappened += TapHappened;
		InputController.Instance.ReleaseHappened += ReleaseHappened;
	}

	private void ReleaseHappened(int fingerID, Vector3 pixelPos) {
		if (currentFingerID == fingerID) {
			currentFingerID = -1;
			currentTapCircleRadius = 0f;
			RefreshTapIndicator(currentTapCircleRadius, pixelPos);
		}
	}

	private void TapHappened(int fingerID, Vector3 pixelPos) {
		if (currentFingerID == -1) {
			currentFingerID = fingerID;
			currentTapCircleRadius = 1f;
			RefreshTapIndicator(currentTapCircleRadius, pixelPos);
		}
	}

	private void Update() {
		if (currentFingerID != -1) {
			currentTapCircleRadius = Mathf.Clamp(currentTapCircleRadius - ConfigDatabase.Instance.indicatorShrinkingSpeed * Time.deltaTime, 0f, Mathf.Infinity);
			RefreshTapIndicator(currentTapCircleRadius, Input.mousePosition);
		}
	}
}
