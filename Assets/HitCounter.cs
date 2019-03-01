using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitCounter : MonoBehaviour
{
	public Text hitCounterText;

	private void Awake() {
		GameController.Instance.RefreshHitNumber += (int hCount) => { hitCounterText.text = hCount.ToString(); };
	}
}
