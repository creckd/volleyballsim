using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TargetConfig {
	public TargetTypes type;
	[Range(0f, 1f)]
	public float circleSizeValue;
	public Color circleColor;
}

public enum Accuracy {
	Perfect,
	Good,
	Close,
	Awful
}

public class ConfigDatabase : MonoBehaviour
{
	private static ConfigDatabase instance = null;
	public static ConfigDatabase Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<ConfigDatabase>();
			}
			return instance;
		}
	}

	[Header("Ball")]
	public float torqueDrag = 1f;
	public float collisionTorqueMultiplier = 50f;
	public float reboundHeightMultiplier = 10f;
	public AnimationCurve reboundCurve;

	[Header("Indicator")]
	public float indicatorShrinkingSpeed = 1f;
	public TargetConfig[] targets;
	public Color playerIndicatorColor = Color.white;

	public TargetConfig GetTargetConfig(TargetTypes type) {
		foreach (var tC in targets) {
			if (tC.type == type)
				return tC;
		}
		return new TargetConfig();
	}

	public Accuracy GetAccuracy(float accuracyValue) {
		if (accuracyValue < 0.1f)
			return Accuracy.Perfect;
		if (accuracyValue < 0.15f)
			return Accuracy.Good;
		if (accuracyValue < 0.25f) {
			return Accuracy.Close;
		}
		return Accuracy.Awful;
	}

}
