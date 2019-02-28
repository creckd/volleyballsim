using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
