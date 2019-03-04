using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngamePanel : MonoBehaviour
{
	public struct IndicatorDisplaySettings {
		public float circleSize;
		public Vector3 screenPosition;
		public Color circleColor;

		public IndicatorDisplaySettings(float circleSize, Vector3 screenPosition, Color circleColor) {
			this.circleSize = circleSize;
			this.screenPosition = screenPosition;
			this.circleColor = circleColor;
		}
	}

	[System.Serializable]
	public struct AccuracyParticle {
		public Accuracy accuracy;
		public ParticleSystem particle;
	}

	public Image tapIndicator;
	public Image targetTapIndicator;
	public AccuracyParticle[] accuracyParticles;

	public GameObject losePanel;
	public Text levelHitNumberText;
	public Text tutorialText;

	private Material tapMat;
	private Material targetMat;
	private RectTransform mainCanvasRectTransform;

	private int localHitNumber = 0;

	public void Awake() {
		GameController.Instance.RefreshTapIndicator += RefreshTapIndicator;
		GameController.Instance.RefreshTarget += RefreshTargetIndicator;
		GameController.Instance.TriedToHitTheBall += PlayAccuracyParticle;
		GameController.Instance.RefreshHitNumber += (int hn) => { localHitNumber = hn; };
		GameController.Instance.GameFinished += GameFinished;
		GameController.Instance.GameStarted += GameStarted;
		tapMat = tapIndicator.material = Instantiate(tapIndicator.material);
		targetMat = targetTapIndicator.material = Instantiate(targetTapIndicator.material);
		mainCanvasRectTransform = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<RectTransform>();
	}

	private void GameStarted() {
		tutorialText.gameObject.SetActive(false);
	}

	private void RefreshTapIndicator(IndicatorDisplaySettings displaySettings) {
		SetIndicator(tapIndicator, tapMat, displaySettings);
	}

	private void RefreshTargetIndicator(IndicatorDisplaySettings displaySettings) {
		SetIndicator(targetTapIndicator, targetMat, displaySettings);
	}

	private void SetIndicator(Image indicatorImage, Material indicatorMaterial, IndicatorDisplaySettings displaySettings) {
		indicatorMaterial.SetFloat("_CircleSize", displaySettings.circleSize);
		indicatorMaterial.SetColor("_OutlineColor", displaySettings.circleColor);
		Vector3 screenPositionNormalized = new Vector3(displaySettings.screenPosition.x / Camera.main.pixelWidth, displaySettings.screenPosition.y / Camera.main.pixelHeight, 0f);
		Vector3 guiPosition = new Vector3(screenPositionNormalized.x * mainCanvasRectTransform.rect.width, screenPositionNormalized.y * mainCanvasRectTransform.rect.height, 0f);
		indicatorImage.rectTransform.anchoredPosition = guiPosition;
	}

	private ParticleSystem GetAccuracyParticle(Accuracy accuracy) {
		foreach (var aP in accuracyParticles) {
			if (aP.accuracy == accuracy)
				return aP.particle;
		}
		return null;
	}

	private void PlayAccuracyParticle(Accuracy accuracy) {
		GetAccuracyParticle(accuracy).transform.position = targetTapIndicator.transform.position;
		GetAccuracyParticle(accuracy).Play();
	}

	private void GameFinished() {
		losePanel.gameObject.SetActive(true);
		tapIndicator.gameObject.SetActive(false);
		targetTapIndicator.gameObject.SetActive(false);
		levelHitNumberText.text = localHitNumber.ToString();
	}

	public void RestartGame() {
		UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
	}
}
