using DG.Tweening;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class PromoPanelController : MonoBehaviour, IInterfacePanel
{
	[Header("Base")]
	[SerializeField] private UIPanelType uiPanelType = UIPanelType.Promo;
	[SerializeField] private Transform panelContainer = default;
	[SerializeField] private PlayerStorage playerStorageSO = default;

	[Header("Loading Image")]
	[SerializeField] private Slider loadingSlider = default;
	[SerializeField] private float fillingTime = default;

	[Header("Build Version")]
	[SerializeField] private TMP_Text buildVersionText = default;
	[SerializeField] private string builVersionPrefixString = "ver.";

	private void Awake()
	{
		Init();
	}

	private void OnEnable()
	{
		GameManager.GameStartAction += ShowPromo;
	}

	private void OnDisable()
	{
		GameManager.GameStartAction -= ShowPromo;
	}

	private void ShowPromo()
	{
		UIController.ShowUIPanelAloneAction?.Invoke(UIPanelType.Promo);
	}

	#region IInterfacePanel
	public UIPanelType UIPanelType { get => uiPanelType; }

	public void Hide()
	{
		panelContainer.gameObject.SetActive(false);
	}

	public void Show()
	{
		buildVersionText.text = $"{builVersionPrefixString}{Application.version}";
		panelContainer.gameObject.SetActive(true);
		loadingSlider.value = 0f;
		loadingSlider.DOValue(1f, fillingTime).OnComplete(() =>
		{
			playerStorageSO.LoadPlayer();
		});
	}

	public void Init()
	{
		UIController.InterfacePanels.Add(this);
	}
	#endregion
}
