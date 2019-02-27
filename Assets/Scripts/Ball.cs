using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BouncePoint {
	public Transform markerTransform;
}

public class Ball : MonoBehaviour
{
	public struct Move {
		public Vector3 to;
		public float speed;
		public AnimationCurve curve;

		public Move(Vector3 to, float speed, AnimationCurve curve) {
			this.to = to;
			this.speed = speed;
			this.curve = curve;
		}
	}

	public BouncePoint[] firstBouncePoints;
	public BouncePoint[] secondBouncePoints;

	public float ballRadius;

	private Vector3 defaultStartingPosition = Vector3.zero;
	private List<Move> moveList = new List<Move>();
	private bool currentlyTravelling = false;

	private void Start() {
		Initilaize();
	}

	public void Initilaize() {
		defaultStartingPosition = transform.position;

		Vector3 firstBouncePoint = GetFirstBouncePoint();
		Vector3 secondBouncePoint = GetSecondBouncePoint();

		TravelTo(firstBouncePoint,10f);
		TravelTo(secondBouncePoint, 10f);
		TravelTo(defaultStartingPosition, 10f);

	}

	public void TravelTo(Vector3 toPosition, float speed, AnimationCurve pathCurve = null) {
		TravelTo(new Move(toPosition, speed, pathCurve));
	}

	public void TravelTo(Move move) {
		moveList.Add(move);
		if (!currentlyTravelling && moveList.Count == 1) {
			StartCoroutine(Travel(move, MoveFinished));
		}
	}

	private void MoveFinished(Move move) {
		moveList.Remove(move);
		if (moveList.Count > 0) {
			StartCoroutine(Travel(moveList[0], MoveFinished));
		}
	}

	IEnumerator Travel(Move move, Action<Move>travelFinished = null) {
		currentlyTravelling = true;
		Vector3 fromPos = transform.position;
		Vector3 toPos = move.to + ((fromPos - move.to).normalized) * ballRadius;
		float timer = 0f;
		float totalTime = Vector3.Distance(fromPos, toPos) / move.speed;
		while (timer <= totalTime) {
			timer += Time.deltaTime;
			transform.position = Vector3.Lerp(fromPos, toPos, timer / totalTime);
			if (move.curve != null)
				transform.position += move.curve.Evaluate(timer / totalTime) * Vector3.up;
			yield return null;
		}
		transform.position = toPos;
		currentlyTravelling = false;
		if (travelFinished != null)
			travelFinished(move);
	}

	private Vector3 GetFirstBouncePoint() {
		return firstBouncePoints[0].markerTransform.position;
	}

	private Vector3 GetSecondBouncePoint() {
		return secondBouncePoints[0].markerTransform.position;
	}

}
