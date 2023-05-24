using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamePanelController : MonoBehaviour, IInterfacePanel
{
	[Header("Base")]
	[SerializeField] private DependencyContainer dependencyContainerSO = default;
	[SerializeField] private LevelStorage levelStorageSO = default;
	[SerializeField] private GameStorage gameStorageSO = default;
	[SerializeField] private PlayerStorage playerStorageSO = default;
	[SerializeField] private UIPanelType uiPanelType = UIPanelType.Game;
	[SerializeField] private Transform panelContainer = default;

	[Header("Countdown")]
	[SerializeField] private RectTransform coundownTextContainer = default;
	[SerializeField] private TextMeshProUGUI coundownText = default;

	[Header("Red Zone Slider")]
	//[SerializeField] private RectTransform rideSliderContainer = default;
	[SerializeField] private RectTransform engineRawContainer = default;
	//[SerializeField] private RectTransform warningContainer = default;
	//[SerializeField] private Slider redZoneSlider = default;
	//[SerializeField] private float redZoneSliderSpeed = default;
	//[SerializeField] private Slider visualSlider = default;
	//[SerializeField] private float baseRedZoneValue = default;
	//[SerializeField] private float redZoneTime = default;

	[Header("Buttons")]
	[SerializeField] private Button startButton = default;
	[SerializeField] private Button refreshButton = default;
	[SerializeField] private Button homeButton = default;
	[SerializeField] private EventTrigger rideTrigger = default;

	[Header("Text")]
	[SerializeField] private TextMeshProUGUI holdText = default;

	[Header("Actions")]
	//public static Action<bool> CheckRedZoneAction = default;
	public static Action DissableButtonsAction = default;
	public static Action<bool> EngineRawContainerStatusAction = default;
	public static Action CarExplosionAction = default;

	//public static Action EngineSpeedUpAction = default;
	//public static Action EngineSpeedDownAction = default;
	public static Action<bool> CountdownCountainerStatusAction = default;
	public static Action StopCarsAction = default;
	//public static Action<bool> RedZoneSliderStatusAction = default;
	public static Action<bool> RideTriggerStatusAction = default;


	//private Coroutine redZoneCoroutine = default;
	//private Coroutine destroyDetailCoroutine = default;
	//private Tween scaleTween = default;
	//private Vector3 baseWarningContainerScale = default;
	private Coroutine startCarsCoroutine = default;
	//private Sequence countdownSequence = default;
	private Tween scaleHoldTextTween = default;

	private void Awake()
	{
		Init();
		PrepareButtons();
		PrepareTrigger();
		//baseWarningContainerScale = warningContainer.transform.localScale;
	}

    private void OnEnable()
    {
		GameManager.PrepereLevelAction += PrepareForLevel;
		//CheckRedZoneAction += CheckRedZone;
		DissableButtonsAction += DissableButtons;
		EngineRawContainerStatusAction += EngineRawContainerStatus;
		//EngineSpeedUpAction += EngineSpeedUp;
		//EngineSpeedDownAction += EngineSpeedDown;
		CountdownCountainerStatusAction += CountdownContainerStatus;
		StopCarsAction += StopCars;
		//RedZoneSliderStatusAction += RedZoneSliderStatus;
		RideTriggerStatusAction += RideTriggerStatus;
	}

    private void OnDisable()
    {
		GameManager.PrepereLevelAction -= PrepareForLevel;
		//CheckRedZoneAction -= CheckRedZone;
		DissableButtonsAction -= DissableButtons;
		EngineRawContainerStatusAction -= EngineRawContainerStatus;
		//EngineSpeedUpAction -= EngineSpeedUp;
		//EngineSpeedDownAction -= EngineSpeedDown;
		CountdownCountainerStatusAction -= CountdownContainerStatus;
		StopCarsAction -= StopCars;
		//RedZoneSliderStatusAction -= RedZoneSliderStatus;
		RideTriggerStatusAction -= RideTriggerStatus;
	}

    private void PrepareButtons() 
	{
		startButton.onClick.RemoveAllListeners();
		startButton.onClick.AddListener(() => {
			VibrationController.Vibrate(30);
			RideTriggerStatus();
			CountdownCountainerStatusAction?.Invoke(true);
			//dependencyContainerSO.CountdownController.StartCountdown();
			StartCountdown();
		});

		refreshButton.onClick.RemoveAllListeners();
		refreshButton.onClick.AddListener(() => {
			SoundManager.PlaySomeSoundOnce(SoundType.ButtonClick);
			VibrationController.Vibrate(30);
			DOTween.KillAll();
			GameManager.PrepereLevelAction?.Invoke();
			StopCars();
			UIController.ShowUIPanelAloneAction?.Invoke(UIPanelType.Game);
		});

		homeButton.onClick.RemoveAllListeners();
		homeButton.onClick.AddListener(() => {
			SoundManager.PlaySomeSoundOnce(SoundType.ButtonClick);
			VibrationController.Vibrate(30);
            DOTween.KillAll();
			StopCars();
			UIController.ShowUIPanelAloneAction?.Invoke(UIPanelType.Main);
		});
	}

	private void PrepareTrigger()
	{
		rideTrigger.triggers.Clear();

		EventTrigger.Entry entry = new EventTrigger.Entry
		{
			eventID = EventTriggerType.PointerDown
		};
		entry.callback.AddListener((data) => {

			HoldTextStatus(false);
			dependencyContainerSO.TugController.Move = true;
		});
		rideTrigger.triggers.Add(entry);

		entry = new EventTrigger.Entry
		{
			eventID = EventTriggerType.PointerUp
		};
		entry.callback.AddListener((data) => {

			dependencyContainerSO.TugController.Move = false;
		});

		rideTrigger.triggers.Add(entry);
	}

	private void PrepareForLevel()
    {
		CountdownCountainerStatusAction?.Invoke(false);
		//scaleTween.Kill();
		//warningContainer.transform.localScale = baseWarningContainerScale;
		//warningContainer.gameObject.SetActive(false);
		StartButtonStatus();
		EngineRawContainerStatusAction?.Invoke(false);
		//visualSlider.value = baseRedZoneValue;
		//redZoneSlider.value = 0f;

		RideTriggerStatus();
		CountdownCountainerStatusAction?.Invoke(true);
		//dependencyContainerSO.CountdownController.StartCountdown();
		StartCountdown();
		playerStorageSO.ConcretePlayer.PlayerCurrentLevel++;
		//Metrics.StartLevelEvent(playerStorageSO.ConcretePlayer.PlayerCurrentLevel);
		dependencyContainerSO.PlayerController.StartLevelTime = Time.time;

		//if (destroyDetailCoroutine != null)
		//{
		//	StopCoroutine(destroyDetailCoroutine);
		//}
		//if (redZoneCoroutine != null)
		//{
		//	StopCoroutine(redZoneCoroutine);
		//}
	}

	private void RideTriggerStatus(bool status)
    {
		rideTrigger.gameObject.SetActive(status);
	}

	private void HoldTextStatus(bool status)
    {

        if (status)
        {
			holdText.gameObject.SetActive(true);
			scaleHoldTextTween = holdText.transform.DOScale(Vector3.one * 1.1f, 0.2f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
		}
        else
        {
			StartCoroutine(HoldTextCoroutine());
		}
    }

	private IEnumerator HoldTextCoroutine()
    {
		yield return new WaitForSeconds(0.5f);
		holdText.gameObject.SetActive(false);
		scaleHoldTextTween.Kill();
		scaleHoldTextTween = null;
	}

	private void StartCountdown()
    {
		CameraController.SetCountdownPositionAction?.Invoke();

		coundownTextContainer.DOScale(0, 0f).OnComplete(() => 
		{
			coundownText.text = "3";
			coundownTextContainer.DOScale(1, 0.5f).OnComplete(() =>
			{
				coundownTextContainer.DOScale(0, 0.5f).OnComplete(() =>
				{
					coundownText.text = "2";
					coundownTextContainer.DOScale(1, 0.5f).OnComplete(() =>
					{
						coundownTextContainer.DOScale(0, 0.5f).OnComplete(() =>
						{
							coundownText.text = "1";
							coundownTextContainer.DOScale(1, 0.5f).OnComplete(() =>
							{
								coundownTextContainer.DOScale(0, 0.5f).OnComplete(() =>
								{
									RideTriggerStatus(true);
									HoldTextStatus(true);
									GamePanelController.StopCarsAction?.Invoke();
									GamePanelController.CountdownCountainerStatusAction?.Invoke(false);
									GamePanelController.EngineRawContainerStatusAction?.Invoke(true);
									GameManager.LevelStartAction?.Invoke();
								});
							});
						});
					});
				});
			});
		});
	}

	private void CountdownContainerStatus(bool status)
    {
		coundownTextContainer.gameObject.SetActive(status);
	}

	private void DissableButtons()
    {
		startButton.gameObject.SetActive(false);
		rideTrigger.gameObject.SetActive(false);
	}

	private void StartButtonStatus()
    {
		DissableButtons();
		startButton.gameObject.SetActive(true);
	}

	private void RideTriggerStatus()
	{
		DissableButtons();
		rideTrigger.gameObject.SetActive(true);
	}

    private void EngineRawContainerStatus(bool status)
    {
		engineRawContainer.gameObject.SetActive(status);
    }

	//private void RedZoneSliderStatus(bool start)
 //   {
 //       if (start && dependencyContainerSO.TugController.RedZone)
 //       {
	//		if (redZoneSlider.value < 1)
	//		{
	//			redZoneSlider.value += 0.005f;
	//		}
 //           else
 //           {
	//			dependencyContainerSO.PlayerController.CarExplosion();
	//		}
	//	}
 //       else
 //       {
	//		if (redZoneSlider.value > 0 && !dependencyContainerSO.TugController.RedZone)
	//		{
	//			redZoneSlider.value -= 0.005f;
	//		}
	//	}
	//}

	//private void EngineSpeedUp()
	//{
	//	if (rideSlider.value < 1)
	//	{
	//		rideSlider.value += dependencyContainerSO.PlayerController.CurrentMotorPower * 0.00015f;
	//	}
	//}

	//private void EngineSpeedDown()
	//{
	//	if (rideSlider.value > 0)
	//	{
	//		rideSlider.value -= dependencyContainerSO.PlayerController.CurrentMotorPower * 0.0005f * 2f;
	//	}
	//}

	//private void CheckRedZone(bool start)
	//{
	//       if (start)
	//       {
	//		if (rideSlider.value >= baseRedZoneValue)
	//		{
	//			if (playerStorageSO.ConcretePlayer.InstalledDetails.Count > 0 || (playerStorageSO.ConcretePlayer.InstalledDetails.Count == 0 && !dependencyContainerSO.PlayerController.LastDetail))
	//			{
	//				if (redZoneCoroutine == null)
	//				{
	//					redZoneCoroutine = StartCoroutine(RedZoneCoroutine());
	//				}
	//			}
	//			else if (playerStorageSO.ConcretePlayer.InstalledDetails.Count == 0 && dependencyContainerSO.PlayerController.LastDetail)
	//			{
	//				StopCheckRedZone();
	//				CarExplosionAction?.Invoke();
	//			}
	//		}
	//	}
	//       else
	//       {
	//		StopCheckRedZone();
	//	}
	//}

	//private void StopCheckRedZone()
	//   {
	//	dependencyContainerSO.PlayerController.DownCar();
	//	if (redZoneCoroutine != null)
	//	{
	//		StopCoroutine(redZoneCoroutine);
	//	}
	//	redZoneCoroutine = null;
	//	if (destroyDetailCoroutine != null)
	//	{
	//		StopCoroutine(destroyDetailCoroutine);
	//	}
	//	destroyDetailCoroutine = null;
	//	dependencyContainerSO.GameStarted = false;
	//}

	//private IEnumerator RedZoneCoroutine()
	//{
	//	dependencyContainerSO.PlayerController.UpCar();
	//	yield return new WaitForSeconds(redZoneTime);

	//	warningContainer.transform.localScale = baseWarningContainerScale;
	//	warningContainer.gameObject.SetActive(true);
	//	scaleTween = warningContainer.DOScale(baseWarningContainerScale * 1.2f, 0.2f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
	//	destroyDetailCoroutine = StartCoroutine(DestroyDetailCoroutine());

	//	while (rideSlider.value >= baseRedZoneValue)
	//	{
	//		VibrationController.Vibrate(30);
	//		dependencyContainerSO.TugController.RedZone = true;
	//		yield return new WaitForSeconds(0.01f);
	//	}

	//	dependencyContainerSO.PlayerController.DownCar();
	//	StopCoroutine(destroyDetailCoroutine);
	//	destroyDetailCoroutine = null;

	//	warningContainer.gameObject.SetActive(false);
	//	scaleTween.Kill();
	//	dependencyContainerSO.TugController.RedZone = false;
	//	StopCoroutine(redZoneCoroutine);
	//	redZoneCoroutine = null;
	//}

	//private IEnumerator DestroyDetailCoroutine()
	//   {
	//	yield return new WaitForSeconds(2f);
	//	dependencyContainerSO.PlayerController.DestroyDetail();
	//	dependencyContainerSO.PlayerController.LastDetail = playerStorageSO.ConcretePlayer.InstalledDetails.Count == 0;

	//       if (rideSlider.value >= baseRedZoneValue)
	//       {
	//		StopCoroutine(destroyDetailCoroutine);
	//		destroyDetailCoroutine = null;

	//		destroyDetailCoroutine = StartCoroutine(DestroyDetailCoroutine());
	//	}
	//}

	private void StartCars()
    {
		startCarsCoroutine = StartCoroutine(StartCarsCoroutine());
	}

	private IEnumerator StartCarsCoroutine()
    {
		dependencyContainerSO.PlayerController.PlayerWheelParticle(true);
		dependencyContainerSO.EnemyController.EnemyWheelParticle(true);
		yield return new WaitForSeconds(1f);
		dependencyContainerSO.PlayerController.PlayerWheelParticle(false);
		dependencyContainerSO.EnemyController.EnemyWheelParticle(false);
		yield return new WaitForSeconds(2f);
		StartCars();
	}

	private void StopCars()
    {
		dependencyContainerSO.PlayerController.PlayerWheelParticle(false);
		dependencyContainerSO.EnemyController.EnemyWheelParticle(false);
		StopCoroutine(startCarsCoroutine);
		startCarsCoroutine = null;
	}

	#region IInterfacePanel
	public UIPanelType UIPanelType { get => uiPanelType; }

	public void Hide()
	{
		panelContainer.gameObject.SetActive(false);
	}

	public void Show()
	{
		RideTriggerStatus(false);
		panelContainer.gameObject.SetActive(true);
		dependencyContainerSO.EnemyController.SetCharacteristics();
		//CameraController.SetBasePositionAction?.Invoke();
		StartCars();
	}

	public void Init()
	{
		UIController.InterfacePanels.Add(this);
	}
	#endregion
}
