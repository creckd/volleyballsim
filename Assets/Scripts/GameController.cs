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

	public Action<IngamePanel.IndicatorDisplaySettings> RefreshTapIndicator = delegate { };
	public Action<IngamePanel.IndicatorDisplaySettings> RefreshTarget = delegate { };

	private int currentFingerID = -1;
	private float currentTapCircleRadius = 0f;
	private Ball ball;

	private void Awake() {
		Application.targetFrameRate = 300;
		InputController.Instance.TapHappened += TapHappened;
		InputController.Instance.ReleaseHappened += ReleaseHappened;

		ball = FindObjectOfType<Ball>();
	}

	private void Start() {
		CreateTarget(ball.transform.position, TargetTypes.Medium);
	}

	public void CreateTarget(Vector3 worldPosition, TargetTypes targetType) {
		TargetConfig config = ConfigDatabase.Instance.GetTargetConfig(targetType);
		RefreshTarget(new IngamePanel.IndicatorDisplaySettings(config.circleSizeValue, Camera.main.WorldToScreenPoint(worldPosition), config.circleColor));
	}

	private void ReleaseHappened(int fingerID, Vector3 pixelPos) {
		if (currentFingerID == fingerID) {
			currentFingerID = -1;
			currentTapCircleRadius = 0f;
			RefreshTapIndicator(new IngamePanel.IndicatorDisplaySettings(currentTapCircleRadius,pixelPos,ConfigDatabase.Instance.playerIndicatorColor));
		}
	}

	private void TapHappened(int fingerID, Vector3 pixelPos) {
		if (currentFingerID == -1) {
			currentFingerID = fingerID;
			currentTapCircleRadius = 1f;
			RefreshTapIndicator(new IngamePanel.IndicatorDisplaySettings(currentTapCircleRadius, pixelPos, ConfigDatabase.Instance.playerIndicatorColor));
		}
	}

	private void Update() {
		if (currentFingerID != -1) {
			currentTapCircleRadius = Mathf.Clamp(currentTapCircleRadius - ConfigDatabase.Instance.indicatorShrinkingSpeed * Time.deltaTime, 0f, Mathf.Infinity);
			RefreshTapIndicator(new IngamePanel.IndicatorDisplaySettings(currentTapCircleRadius, Input.mousePosition, ConfigDatabase.Instance.playerIndicatorColor));
		}
	}
}
