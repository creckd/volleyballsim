using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngamePanel : MonoBehaviour
{
	public Image tapIndicator;

	private Material tapMat;
	private RectTransform mainCanvasRectTransform;

	public void Awake() {
		GameController.Instance.RefreshTapIndicator += RefreshTapIndicator;
		tapMat = tapIndicator.material = Instantiate(tapIndicator.material);
		mainCanvasRectTransform = FindObjectOfType<Canvas>().GetComponent<RectTransform>();
	}

	private void RefreshTapIndicator(float currentCircleRadius, Vector3 screenPosition) {
		tapMat.SetFloat("_CircleSize", currentCircleRadius);
		Vector3 screenPositionNormalized = new Vector3(screenPosition.x / Camera.main.pixelWidth, screenPosition.y / Camera.main.pixelHeight, 0f);
		Vector3 guiPosition = new Vector3(screenPositionNormalized.x * mainCanvasRectTransform.rect.width, screenPositionNormalized.y * mainCanvasRectTransform.rect.height, 0f);
		tapIndicator.rectTransform.anchoredPosition = guiPosition;
	}
}
