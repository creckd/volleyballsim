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
		public Vector3 from;
		public Vector3 to;
		public float speed;
		public AnimationCurve curve;

		public Move(Vector3 from, Vector3 to, float speed, AnimationCurve curve) {
			this.from = from;
			this.to = to;
			this.speed = speed;
			this.curve = curve;
		}
	}

	public BallPath[] ballPaths;

	public float ballRadius;
	public bool currentlyRebounding = false;
	public bool currentlyTravelling = false;

	private Vector3 defaultStartingPosition = Vector3.zero;
	private List<Move> moveList = new List<Move>();

	public float timeFinishedTravelling;

	private Vector3 torqueVelocity = Vector3.zero;

	private void Awake() {
		Initilaize();
	}

	private void Update() {
		transform.Rotate(torqueVelocity);
	}

	private void LateUpdate() {
		float x = Mathf.Lerp(torqueVelocity.x, 0f, ConfigDatabase.Instance.torqueDrag * Time.deltaTime);
		float y = Mathf.Lerp(torqueVelocity.y, 0f, ConfigDatabase.Instance.torqueDrag * Time.deltaTime);
		float z = Mathf.Lerp(torqueVelocity.z, 0f, ConfigDatabase.Instance.torqueDrag * Time.deltaTime);
		torqueVelocity = new Vector3(x, y, z);
	}

	public void Initilaize() {
		defaultStartingPosition = transform.position;
	}

	public void TravelTo(BallPath path) {
		TravelTo(path.arrivePosition.position, path.firstBounce.position, 15f);
		TravelTo(path.firstBounce.position, path.secondBounce.position, 10f);
		TravelTo(path.secondBounce.position, path.arrivePosition.position, 5f, ConfigDatabase.Instance.reboundCurve);
	}

	public void TravelTo(BallPath path, Vector3 manualArrivePosition) {
		TravelTo(path.arrivePosition.position, path.firstBounce.position, 20f);
		TravelTo(path.firstBounce.position, path.secondBounce.position, 15f);
		TravelTo(path.secondBounce.position, manualArrivePosition, 10f, ConfigDatabase.Instance.reboundCurve);
	}

	public void TravelTo(Vector3 fromPosition, Vector3 toPosition, float speed, AnimationCurve pathCurve = null) {
		TravelTo(new Move(fromPosition, toPosition, speed, pathCurve));
	}

	public void TravelTo(Move move) {
		moveList.Add(move);
		if (!currentlyTravelling && moveList.Count == 1) {
			StartCoroutine(Travel(move, MoveFinished));
		}
	}

	private void MoveFinished(Move move) {
		if (move.curve != null) //gyors gameplay iteration hack
			currentlyRebounding = false;
		moveList.Remove(move);
		if (moveList.Count > 0) {
			if (moveList[0].curve != null)
				currentlyRebounding = true;
			CalculateTorque(move, moveList[0]);
			StartCoroutine(Travel(moveList[0], MoveFinished));
		}
	}

	private void CalculateTorque(Move current, Move next) {
		Vector3 fromDirection = (current.to - current.from).normalized;
		Vector3 toDirection = (next.to - next.from).normalized;
		torqueVelocity += new Vector3(0f, 0f, Vector3.Dot(fromDirection, toDirection) * ConfigDatabase.Instance.collisionTorqueMultiplier);
	}

	IEnumerator Travel(Move move, Action<Move>travelFinished = null) {
		currentlyTravelling = true;
		Vector3 fromPos = move.from;
		Vector3 toPos = move.to;
		float timer = 0f;
		float totalTime = Vector3.Distance(fromPos, toPos) / move.speed;
		while (timer <= totalTime) {
			timer += Time.deltaTime;
			transform.position = Vector3.Lerp(fromPos, toPos, timer / totalTime);
			if (move.curve != null)
				transform.position += move.curve.Evaluate(timer / totalTime) * Vector3.up * ConfigDatabase.Instance.reboundHeightMultiplier;
			yield return null;
		}
		transform.position = toPos;
		timeFinishedTravelling = Time.realtimeSinceStartup;
		currentlyTravelling = false;
		if (travelFinished != null)
			travelFinished(move);
	}

	public BallPath GetRandomBallPath() {
		//return ballPaths[0];
		return ballPaths[UnityEngine.Random.Range(0,ballPaths.Length)];
	}

}
