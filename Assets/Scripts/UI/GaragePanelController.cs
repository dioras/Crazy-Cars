using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GaragePanelController : MonoBehaviour, IInterfacePanel
{
	[Header("Base")]
	[SerializeField] private UIPanelType uiPanelType = UIPanelType.Garage;
	[SerializeField] private Transform panelContainer = default;
	[SerializeField] private DependencyContainer dependencyContainerSO = default;
	[SerializeField] private PlayerStorage playerStorageSO = default;
	[SerializeField] private DetailsStorage detailsStorageSO = default;
	[SerializeField] private RectTransform detailsContent = default;
	[SerializeField] private UI_DetailItem detailItem = default;

	[Header("Tutorial")]
	[SerializeField] private Canvas carPanelCanvas = default;
	[SerializeField] private Canvas tutorialBlockPanelCanvas = default;

	[Header("Buttons Block")]
	[SerializeField] private Button exitButton = default;

	[Header("Actions")]
	public static Action CheckInstalledDetailsAction = default;
	public static Action<Detail> SetNewDetailAction = default;

	private List<UI_DetailItem> detailItems = new List<UI_DetailItem>();

	private void Awake()
	{
		Init();
		PrepareButtons();
		SpawnDetailItems();
	}

    private void OnEnable()
    {
		CheckInstalledDetailsAction += CheckInstalledDetails;
		SetNewDetailAction += SetNewDetail;
	}

    private void OnDisable()
    {
		CheckInstalledDetailsAction -= CheckInstalledDetails;
		SetNewDetailAction -= SetNewDetail;
	}

    private void PrepareButtons()
	{
		exitButton.onClick.RemoveAllListeners();
		exitButton.onClick.AddListener(() => {
			SoundManager.PlaySomeSoundOnce(SoundType.ButtonClick);
			UIController.ShowUIPanelAloneAction?.Invoke(UIPanelType.Main);
			VibrationController.Vibrate(30);
		});
	}

	private void SpawnDetailItems()
	{
		for (int i = 0; i < detailsStorageSO.Details.Count; i++)
		{
			UI_DetailItem detail = Instantiate(detailItem, detailsContent);
			detail.gameObject.SetActive(false);
			detailItems.Add(detail);
		}
	}

	private void HideDetailItems()
	{
		for (int i = 0; i < detailItems.Count; i++)
		{
			detailItems[i].Owned = false;
			detailItems[i].gameObject.SetActive(false);
		}
	}

	private void PrepereDetailItems()
	{
		HideDetailItems();

		for (int i = 0; i < playerStorageSO.ConcretePlayer.PlayerDetails.Count; i++)
		{
			detailItems[i].Owned = true;
			detailItems[i].gameObject.SetActive(true);
			detailItems[i].SetDetail(playerStorageSO.ConcretePlayer.PlayerDetails[i]);
            if (playerStorageSO.ConcretePlayer.NewDetailItems.Count != 0 && playerStorageSO.ConcretePlayer.NewDetailItems.Any(det => det.detailType == detailItems[i].CurrentDetail.detailType && det.presetType == detailItems[i].CurrentDetail.presetType))
            {
				detailItems[i].NewDetailMarkerStatus(true);
			}
		}

		CheckInstalledDetails();;
	}
	private void CheckInstalledDetails()
	{
		foreach (var detailItem in detailItems)
		{
            if (detailItem.Owned)
            {
				detailItem.SetInstalled(playerStorageSO.ConcretePlayer.InstalledDetails.Any(detail => detail.detailType == detailItem.CurrentDetail.detailType && detail.presetType == detailItem.CurrentDetail.presetType));
            }
		}
	}

	private void SetNewDetail(Detail detail)
    {
		playerStorageSO.ConcretePlayer.NewDetailItems.Add(detail);
	}

	private void StartTutorial()
	{
		tutorialBlockPanelCanvas.gameObject.SetActive(true);
		carPanelCanvas.overrideSorting = tutorialBlockPanelCanvas.overrideSorting = true;
		carPanelCanvas.sortingOrder = 10;
		tutorialBlockPanelCanvas.sortingOrder = 5;

		detailItems[0].StartTutorial();
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
		dependencyContainerSO.GarageController.StartRotate();
		dependencyContainerSO.PlayerController.PlayerCanvas.gameObject.SetActive(false);
		dependencyContainerSO.PlayerController.SetPreviewPosition();
		dependencyContainerSO.GarageController.SetCar();
		PrepereDetailItems();

		carPanelCanvas.overrideSorting = false;
		tutorialBlockPanelCanvas.gameObject.SetActive(false);
		if (playerStorageSO.ConcretePlayer.FirstLaunch)
		{
			StartTutorial();
		}
	}

	public void Init()
	{
		UIController.InterfacePanels.Add(this);
	}
	#endregion
}
