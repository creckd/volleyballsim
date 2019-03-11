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

	public enum GameState {
		NotStarted,
		Playing,
		Finished
	}

	public GameState currentGameState = GameState.NotStarted;

	public Action<IngamePanel.IndicatorDisplaySettings> RefreshTapIndicator = delegate { };
	public Action<IngamePanel.IndicatorDisplaySettings> RefreshTarget = delegate { };

	public Action<Accuracy> TriedToHitTheBall = delegate { };
	public Action<int> RefreshHitNumber = delegate { };
	public Action GameFinished = delegate { };
	public Action GameStarted = delegate { };

	private int currentFingerID = -1;
	private float currentTapCircleRadius = 0f;
	private Ball ball;
	private BallPath currentPath = null;

	private TargetConfig currentTargetConfig;
	private Vector3 currentTargetWorldPosition = Vector3.zero;
	private int numberOfHits = 0;

	private void Awake() {
		Application.targetFrameRate = 300;
		InputController.Instance.TapHappened += TapHappened;
		InputController.Instance.ReleaseHappened += ReleaseHappened;

		ball = FindObjectOfType<Ball>();
	}

	private void Start() {
		currentPath = ball.ballPaths[0];
		CreateTarget(currentPath.arrivePosition.position, currentPath.targetType);
	}

	public void CreateTarget(Vector3 worldPosition, TargetTypes targetType) {
		TargetConfig config = ConfigDatabase.Instance.GetTargetConfig(targetType);
		RefreshTarget(new IngamePanel.IndicatorDisplaySettings(config.circleSizeValue, Camera.main.WorldToScreenPoint(worldPosition), config.circleColor));
		currentTargetConfig = config;
		currentTargetWorldPosition = worldPosition;
	}

	private void ReleaseHappened(int fingerID, Vector3 pixelPos) {
		if (currentFingerID == fingerID && currentGameState != GameState.Finished) {
			currentFingerID = -1;

			Accuracy accuracy = ConfigDatabase.Instance.GetAccuracy(DetermineAccuracy(pixelPos));
			TriedToHitTheBall(accuracy);
			if (accuracy == Accuracy.Perfect || accuracy == Accuracy.Good) {
				if (currentTargetConfig.type == TargetTypes.Hard)
					CameraShake.Instance.ShakeCamera(1f, 0.1f);
				if (currentGameState == GameState.NotStarted) {
					currentGameState = GameState.Playing;
					GameStarted();
				}
				numberOfHits++;
				BallPath nextPath = ball.GetRandomBallPath();
				ball.TravelTo(currentPath, nextPath.arrivePosition.position);
				currentPath = nextPath;
				CreateTarget(currentPath.arrivePosition.position, currentPath.targetType);
				RefreshHitNumber(numberOfHits);
			}

			currentTapCircleRadius = 0f;
			RefreshTapIndicator(new IngamePanel.IndicatorDisplaySettings(currentTapCircleRadius,pixelPos,ConfigDatabase.Instance.playerIndicatorColor));
		}
	}

	private void TapHappened(int fingerID, Vector3 pixelPos) {
		if (currentFingerID == -1 && currentGameState != GameState.Finished) {
			currentFingerID = fingerID;
			currentTapCircleRadius = 1f;
			RefreshTapIndicator(new IngamePanel.IndicatorDisplaySettings(currentTapCircleRadius, pixelPos, ConfigDatabase.Instance.playerIndicatorColor));
		}
	}

	private float DetermineAccuracy(Vector3 pixelPos) {
		float sizeDifference = Mathf.Abs(currentTapCircleRadius - currentTargetConfig.circleSizeValue);
		float positionDifference = (Vector3.Distance(Camera.main.WorldToScreenPoint(currentTargetWorldPosition), pixelPos)) / Screen.width;
		return sizeDifference + positionDifference;

	}

	private void Update() {
		if (currentFingerID != -1) {
			if (currentGameState == GameState.Playing) {
				if (ball.currentlyRebounding || (!ball.currentlyTravelling && (Time.realtimeSinceStartup - ball.timeFinishedTravelling < 0.25f))) {
					float targetCircleSize = currentTargetConfig.circleSizeValue - 0.1f;
					float normalizedDistance = Mathf.Clamp01(Vector3.Distance(ball.transform.position, currentTargetWorldPosition) / 6.5f);
					currentTapCircleRadius = targetCircleSize + (normalizedDistance * (1f - targetCircleSize));
				} else {
					currentTapCircleRadius = 1f;
				}
			} else {
				currentTapCircleRadius = Mathf.Clamp(currentTapCircleRadius - ConfigDatabase.Instance.indicatorShrinkingSpeed * Time.deltaTime, 0f, Mathf.Infinity);
			}
		} else {
			currentTapCircleRadius = 0f;
		}
			RefreshTapIndicator(new IngamePanel.IndicatorDisplaySettings(currentTapCircleRadius, Input.mousePosition, ConfigDatabase.Instance.playerIndicatorColor));

		if (!ball.currentlyTravelling && currentGameState == GameState.Playing && currentGameState != GameState.Finished && (Time.realtimeSinceStartup - ball.timeFinishedTravelling) > 0.25f) {
			currentGameState = GameState.Finished;
			GameFinished();
		}
	}
}
