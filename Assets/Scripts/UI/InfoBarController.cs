using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using DG.Tweening;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using Random = UnityEngine.Random;

public class InfoBarController : MonoBehaviour
{
	[Header("Base")]
	[SerializeField] private PlayerStorage playerStorageSO = default;
	[SerializeField] private GameStorage gameStorageSO = default;
	[SerializeField] private Transform container = default;

	[Header("Components")]
	[SerializeField] private TMP_Text currencySoft = default;
	[SerializeField] private Button settingsButton = default;

	[Header("Settings Buttons")]
	[SerializeField] private Button closeSettingsButton = default;
	[SerializeField] private Toggle musicButton = default;
	[SerializeField] private Toggle soundButton = default;
	[SerializeField] private Toggle vibrationButton = default;
	[SerializeField] private Button saveButton = default;

	[Header("Settings Parameters")]
	[SerializeField] private RectTransform settingsPanel = default;
	[SerializeField] private RectTransform settingsContainer = default;
	[SerializeField] private Image backgroundImage= default;
	private bool isSoundOn = true;
	private bool isMusicOn = true;
	private bool isVibrationOn = true;

	[Header("Variables")]
	public static Action FillUserInfoPanelAction = default;
	private Vector3 containerScale = default;

	private void Awake()
	{
		containerScale = settingsContainer.transform.localScale;

		SettingsDisplayStatus(false);
		PrepareButtons();
	}

	private void OnEnable()
	{
		Hide();
		FillUserInfoPanelAction += FillUserInfoPanel;
		UIController.ShowUIPanelAloneAction += ReactPanel;
	}

	private void OnDisable()
	{
		FillUserInfoPanelAction -= FillUserInfoPanel;
		UIController.ShowUIPanelAloneAction -= ReactPanel;
	}

	private void ReactPanel(UIPanelType _uIPanelType)
	{
		switch (_uIPanelType)
		{
			case UIPanelType.Main:
				Show();
				break;
			case UIPanelType.Garage:
				Show();
				break;
			default:
				Hide();
				break;
		}
	}

	private void Show()
	{
		container.gameObject.SetActive(true);
		FillUserInfoPanel();
	}

	private void Hide()
	{
		container.gameObject.SetActive(false);
	}

	private void SetCurrencyCounter()
	{
		currencySoft.text = playerStorageSO.ConcretePlayer.Coins.ToString();
	}

	public void FillUserInfoPanel()
	{
		SetCurrencyCounter();
	}

	private void PrepareButtons()
	{
		settingsButton.onClick.RemoveAllListeners();
		settingsButton.onClick.AddListener(() => {
			SoundManager.PlaySomeSoundOnce?.Invoke(SoundType.ButtonClick);
			SettingsDisplayStatus(true);
		});

		closeSettingsButton.onClick.RemoveAllListeners();
		closeSettingsButton.onClick.AddListener(() => {
			SoundManager.PlaySomeSoundOnce?.Invoke(SoundType.ButtonClick);
			SettingsDisplayStatus(false);
		});

		musicButton.onValueChanged.RemoveAllListeners();
		musicButton.onValueChanged.AddListener((value) => {
			SoundManager.PlaySomeSoundOnce?.Invoke(SoundType.ButtonClick);
			isMusicOn = value;
		});

		soundButton.onValueChanged.RemoveAllListeners();
		soundButton.onValueChanged.AddListener((value) => {
			SoundManager.PlaySomeSoundOnce?.Invoke(SoundType.ButtonClick);
			isSoundOn = value;
		});

		vibrationButton.onValueChanged.RemoveAllListeners();
		vibrationButton.onValueChanged.AddListener((value) => {
			SoundManager.PlaySomeSoundOnce?.Invoke(SoundType.ButtonClick);
			isVibrationOn = value;
		});

		saveButton.onClick.RemoveAllListeners();
		saveButton.onClick.AddListener(() => {
			SoundManager.PlaySomeSoundOnce?.Invoke(SoundType.ButtonClick);
			SetSettings();
			SettingsDisplayStatus(false);
		});
	}

	private void GetSettings()
	{
		musicButton.isOn = playerStorageSO.ConcretePlayer.IsMusicOn;
		soundButton.isOn = playerStorageSO.ConcretePlayer.IsSoundOn;
		vibrationButton.isOn = playerStorageSO.ConcretePlayer.IsVibrationcOn;
	}

	private void SetSettings()
	{
		playerStorageSO.ConcretePlayer.SetSoundStatus(isSoundOn);
		playerStorageSO.ConcretePlayer.SetVibrationtatus(isVibrationOn);
		playerStorageSO.ConcretePlayer.SetMusicStatus(isMusicOn);
		playerStorageSO.ApplyPlayerSettings();
		if (isMusicOn)
		{
			SoundManager.PlaySomeSoundContinuous?.Invoke(SoundType.MainMelody, () => true);
		}
		GetSettings();
	}

	private void SettingsDisplayStatus(bool active)
	{
		if (active)
		{
			settingsContainer.transform.DOScale(new Vector3(1, 1, 1), gameStorageSO.GameBaseParameters.TimeToInteractUI);
			settingsPanel.gameObject.SetActive(true);
			backgroundImage.gameObject.SetActive(true);
			GetSettings();
		}
		else
		{
			settingsContainer.transform.DOScale(Vector3.zero, gameStorageSO.GameBaseParameters.TimeToInteractUI).OnComplete(() => {
				backgroundImage.gameObject.SetActive(false);
				settingsPanel.gameObject.SetActive(false);
			});
		}
	}
}
