using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MainPanelController : MonoBehaviour, IInterfacePanel
{
	[Header("Base")]
	[SerializeField] private UIPanelType uiPanelType = UIPanelType.Main;
	[SerializeField] private Transform panelContainer = default;
	[SerializeField] private DependencyContainer dependencyContainerSO = default;
	[SerializeField] private PlayerStorage playerStorageSO = default;
	[SerializeField] private GameStorage gameStorageSO = default;
	[SerializeField] private LevelStorage levelStorageSO = default;
	[SerializeField] private DetailsStorage detailsStorageSO = default;

	[Header("Level Bar")]
	//[SerializeField] private RectTransform prizeImage = default;
	[SerializeField] private List<LayoutElement> levelBarMarks = new List<LayoutElement>();
	[SerializeField] private Color compliteLevelMark = Color.white;
	[SerializeField] private Color currencyLevelMark = Color.white;
	[SerializeField] private Color defaultLevelMark = Color.white;
	[SerializeField] private float defaultHeight = 96f;
	[SerializeField] private float currencyLevelHeight = 112f;
	[SerializeField] private Image imageCurrencyWorld = default;
	[SerializeField] private Image imageNextWorld = default;

	private Sequence sequenceLevelBar = default;

	[Header("Tutorial")]
	[SerializeField] private Canvas tutorialBlockPanelCanvas = default;
	[SerializeField] private Canvas buttonCanvas = default;

	[Header("Buttons Block")]
	[SerializeField] private Button startButton = default;
	[SerializeField] private Button garageButton = default;
	[SerializeField] private Button buyGachaponButton = default;
	[SerializeField] private Image newDetailMarker = default;

	[Header("Gachapon")]
	[SerializeField] private Image gachaponImage = default;
	[SerializeField] private TextMeshProUGUI gachaCoastText = default;
	[SerializeField] private RectTransform gachaMarker = default;
	[SerializeField] private RectTransform gachaParticle = default;

	[Header("Actions")]
	public static Action<bool> NewDetailMarkerStatusAction = default;
	public static Action SetGachaponStatusAction = default;

	private Tween tutorialButtonTween = default;

	private void Awake()
	{
		Init();
		PrepareButtons();

		//prizeImage.anchoredPosition = new Vector2(0f, 50.5f);
		//prizeImage.DOAnchorPosY(80f, .5f).SetLoops(-1, LoopType.Yoyo);
	}

	private void OnEnable()
	{
		GameManager.PlayerLoadedAction += ShowMain;
		NewDetailMarkerStatusAction += NewDetailMarkerStatus;
		SetGachaponStatusAction += SetGachaponStatus;
	}

	private void OnDisable()
	{
		GameManager.PlayerLoadedAction -= ShowMain;
		NewDetailMarkerStatusAction -= NewDetailMarkerStatus;
		SetGachaponStatusAction -= SetGachaponStatus;
	}

	private void PrepareButtons()
	{
		startButton.onClick.RemoveAllListeners();
		startButton.onClick.AddListener(() => {
			SoundManager.PlaySomeSoundOnce(SoundType.ButtonClick);
			GameManager.PrepereLevelAction?.Invoke();
			UIController.ShowUIPanelAloneAction?.Invoke(UIPanelType.Game);
			VibrationController.Vibrate(30);
		});

		garageButton.onClick.RemoveAllListeners();
		garageButton.onClick.AddListener(() => {
			SoundManager.PlaySomeSoundOnce(SoundType.ButtonClick);
			UIController.ShowUIPanelAloneAction?.Invoke(UIPanelType.Garage);
			VibrationController.Vibrate(30);
			playerStorageSO.ConcretePlayer.HasNewDetails = false;

			if (tutorialButtonTween != null)
			{
				tutorialButtonTween.Kill();
			}
		});

		buyGachaponButton.onClick.RemoveAllListeners();
		buyGachaponButton.onClick.AddListener(() => {
			SoundManager.PlaySomeSoundOnce(SoundType.ButtonClick);
			VibrationController.Vibrate(30);

			if (playerStorageSO.ConcretePlayer.FirstGacha == true)
			{
				playerStorageSO.ConcretePlayer.Coins -= gameStorageSO.GameBaseParameters.BaseGachaponCoast / 2;
				playerStorageSO.ConcretePlayer.FirstGacha = false;
			}
			else
			{
				playerStorageSO.ConcretePlayer.Coins -= gameStorageSO.GameBaseParameters.BaseGachaponCoast;
			}


			
			InfoBarController.FillUserInfoPanelAction?.Invoke();
			UIController.ShowUIPanelAloneAction?.Invoke(UIPanelType.Gachapon);

			//NewDetailMarkerStatus(playerStorageSO.ConcretePlayer.HasNewDetails);
			//SetGachaponStatus();
		});
	}

	public void NewDetailMarkerStatus(bool status)
	{
		newDetailMarker.gameObject.SetActive(status);
	}

	private void ShowMain()
	{
		UIController.ShowUIPanelAloneAction?.Invoke(UIPanelType.Main);
	}

	private void StartTutorial()
	{
		dependencyContainerSO.LevelGenerator.SetLevel();
		startButton.gameObject.SetActive(false);
		buttonCanvas.overrideSorting = true;
		buttonCanvas.sortingOrder = 10;

		tutorialBlockPanelCanvas.gameObject.SetActive(true);
		tutorialBlockPanelCanvas.overrideSorting = true;
		tutorialBlockPanelCanvas.sortingOrder = 5;

		tutorialButtonTween = garageButton.gameObject.transform.DOScale(garageButton.gameObject.transform.localScale * 1.1f, 0.3f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
	}

	private void SetGachaponStatus()
    {
        if (playerStorageSO.ConcretePlayer.PlayerDetails.Count < detailsStorageSO.Details.Count)
        {
			
			if (playerStorageSO.ConcretePlayer.FirstGacha == true)
			{
				buyGachaponButton.interactable = playerStorageSO.ConcretePlayer.Coins >= gameStorageSO.GameBaseParameters.BaseGachaponCoast / 2f;
			}
			else
			{
				buyGachaponButton.interactable = playerStorageSO.ConcretePlayer.Coins >= gameStorageSO.GameBaseParameters.BaseGachaponCoast;
			}

			if (buyGachaponButton.interactable)
			{
				dependencyContainerSO.GachaponController.ActiveGacha();
				gachaponImage.color = new Color(gachaponImage.color.r, gachaponImage.color.g, gachaponImage.color.b, 1f);
			}
			else
			{
				gachaponImage.color = new Color(gachaponImage.color.r, gachaponImage.color.g, gachaponImage.color.b, 0.5f);
			}

			gachaMarker.gameObject.SetActive(buyGachaponButton.interactable);
			gachaParticle.gameObject.SetActive(buyGachaponButton.interactable);
			
		}
        else
        {
			gachaParticle.gameObject.SetActive(false);
			buyGachaponButton.interactable = false;
			gachaMarker.gameObject.SetActive(false);
		}
	}


	private void ShowCurrencyProgress()
	{
		sequenceLevelBar = DOTween.Sequence();

		for (int i = 0; i < levelBarMarks.Count; i++)
		{
			levelBarMarks[i].gameObject.SetActive(false);
		}

		imageCurrencyWorld.sprite = levelStorageSO.WorldList.Find(world => world.WorldType == playerStorageSO.ConcretePlayer.WorldType).WorldSprite;
		var worldIndex = (int)playerStorageSO.ConcretePlayer.WorldType;
		worldIndex++;
		imageNextWorld.sprite = worldIndex <= levelStorageSO.WorldList.Count - 1 ? levelStorageSO.WorldList.Find(world => world.WorldType == (WorldType)worldIndex).WorldSprite : levelStorageSO.WorldList.Find(world => world.WorldType == WorldType.Alpha).WorldSprite;

		levelBarMarks.ForEach((_mark) => SetMarkLevelBar(_mark, defaultLevelMark, defaultHeight));
		bool _compliteLevels = true;
		for (int i = 0; i < levelStorageSO.WorldList.Find(world => world.WorldType == playerStorageSO.ConcretePlayer.WorldType).LevelsList.Count; i++)
		{
			levelBarMarks[i].gameObject.SetActive(true);
			//prizeImage.transform.SetParent(levelBarMarks[i].transform);
			//prizeImage.anchoredPosition = new Vector2(0f, prizeImage.anchoredPosition.y);
			if ((int)playerStorageSO.ConcretePlayer.LevelNameType == i)
			{
				SetMarkLevelBar(levelBarMarks[i], currencyLevelMark, currencyLevelHeight);
				_compliteLevels = false;
			}
			else
			{
				SetMarkLevelBar(levelBarMarks[i], _compliteLevels ? compliteLevelMark : defaultLevelMark, defaultHeight);
			}
		}

		sequenceLevelBar.Play();
	}

	private void SetMarkLevelBar(LayoutElement mark, Color color, float height)
	{
		var markImage = mark.GetComponent<Image>();
		markImage.color = defaultLevelMark;
		if (color != defaultLevelMark)
		{
			sequenceLevelBar.Append(mark.DOPreferredSize(new Vector2(mark.preferredWidth, currencyLevelHeight), .3f / 2f).OnComplete(() =>
			{
				markImage.DOColor(color, .2f / 2f);
				mark.DOPreferredSize(new Vector2(mark.preferredWidth, height), .15f / 2f);
			}));
		}
		else
		{
			mark.preferredHeight = height;
			markImage.DOColor(color, .2f / 2f);
		}
	}

	#region IInterfacePanel
	public UIPanelType UIPanelType { get => uiPanelType; }

	public void Hide()
	{
		panelContainer.gameObject.SetActive(false);
	}

	public void Show()
	{
		panelContainer.gameObject.SetActive(true);
		dependencyContainerSO.LevelGenerator.CreateLevel();
		CameraController.SetMenuPositionAction?.Invoke();
		dependencyContainerSO.EnemyController.PrepereForLevel();
		dependencyContainerSO.GachaponController.PrepereForLevel();
		dependencyContainerSO.GachaponController.PrepereDetails();
		dependencyContainerSO.GachaponController.MenuMove();
		if (playerStorageSO.ConcretePlayer.FirstGacha == true)
		{
			gachaCoastText.text = "$" + (gameStorageSO.GameBaseParameters.BaseGachaponCoast / 2f).ToString();
		}
		else
		{
			gachaCoastText.text = "$" + gameStorageSO.GameBaseParameters.BaseGachaponCoast.ToString();
		}
		
		//dependencyContainerSO.EnemyController.StartCanvasStatus(false);
		dependencyContainerSO.PlayerController.SetBody();

		if (tutorialButtonTween != null)
		{
			tutorialButtonTween.Kill();
		}
		garageButton.gameObject.transform.localScale = Vector3.one;
		tutorialBlockPanelCanvas.gameObject.SetActive(false);
		startButton.gameObject.SetActive(true);
		buttonCanvas.overrideSorting = false;

		if (playerStorageSO.ConcretePlayer.FirstLaunch)
        {
            dependencyContainerSO.PlayerController.AddDetail(detailsStorageSO.Details.Find(detail => detail.worldType == WorldType.Alpha && detail.detailType == DetailType.Wheel && detail.presetType == PresetType.Alpha));
			StartTutorial();
		}
        else
        {
            dependencyContainerSO.PlayerController.SetDetails();
        }

        dependencyContainerSO.PlayerController.SetGamePosition();
		

		NewDetailMarkerStatus(playerStorageSO.ConcretePlayer.HasNewDetails);
		SetGachaponStatus();
		ShowCurrencyProgress();
		
	}

	public void Init()
	{
		UIController.InterfacePanels.Add(this);
	}
	#endregion
}
