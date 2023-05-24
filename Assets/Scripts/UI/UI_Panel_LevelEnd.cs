using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UI_Panel_LevelEnd : MonoBehaviour, IInterfacePanel
{
	[Header("Base")]
	[SerializeField] private UIPanelType uiPanelType = UIPanelType.LevelResult;
	[SerializeField] private Transform panelContainer = default;
    [SerializeField] private LevelStorage levelStorageSO = default;
    [SerializeField] private DependencyContainer dependencyContainerSO = default;
    [SerializeField] private PlayerStorage playerStorageSO = default;
    [SerializeField] private GameStorage gameStorageSO = default;
    [SerializeField] private DetailsStorage detailsStorageSO = default;

    [Header("Result Panels")]
    [SerializeField] private RectTransform winPanel = default;
    [SerializeField] private RectTransform losePanel = default;
    [SerializeField] private TextMeshProUGUI coinText = default;

    [Header("Buttons")]
    [SerializeField] private Button homeButton = default;
    [SerializeField] private Button nextlevelButton = default;
    [SerializeField] private Button restartButton = default;


    private void Awake() {
		PrepareButtons();
		Init();
	}

	private void OnEnable() {
        GameManager.LevelFinishAction += ShowPanel;

    }

	private void OnDisable() {
        GameManager.LevelFinishAction -= ShowPanel;
    }

	private void ShowPanel(bool win) {
        FillDataLayout(win);
        UIController.ShowUIPanelAloneAction?.Invoke(UIPanelType.LevelResult);
    }

    private void PrepareButtons() {
        homeButton.onClick.RemoveAllListeners();
        homeButton.onClick.AddListener(() => {
            SoundManager.PlaySomeSoundOnce?.Invoke(SoundType.ButtonClick);
            VibrationController.Vibrate(30);
            UIController.ShowUIPanelAloneAction?.Invoke(UIPanelType.Main);
        });

        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(() => {
            SoundManager.PlaySomeSoundOnce?.Invoke(SoundType.ButtonClick);
            VibrationController.Vibrate(30);
            DOTween.KillAll();
            GameManager.PrepereLevelAction?.Invoke();
            dependencyContainerSO.EnemyController.PrepereForLevel();
            dependencyContainerSO.GachaponController.PrepereForLevel();
            dependencyContainerSO.GachaponController.PrepereDetails();
            UIController.ShowUIPanelAloneAction?.Invoke(UIPanelType.Game);
        });

        nextlevelButton.onClick.RemoveAllListeners();
        nextlevelButton.onClick.AddListener(() => {
            SoundManager.PlaySomeSoundOnce?.Invoke(SoundType.ButtonClick);
            VibrationController.Vibrate(30);
            DOTween.KillAll();
            dependencyContainerSO.LevelGenerator.CreateLevel();
            GameManager.PrepereLevelAction?.Invoke();
            dependencyContainerSO.EnemyController.PrepereForLevel();
            dependencyContainerSO.GachaponController.PrepereForLevel();
            dependencyContainerSO.GachaponController.PrepereDetails();
            UIController.ShowUIPanelAloneAction?.Invoke(UIPanelType.Game);
        });
    }

    private void FillDataLayout(bool win)
    {
        winPanel.gameObject.SetActive(false);
        losePanel.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        nextlevelButton.gameObject.SetActive(false);
        

        if (win)
        {
            winPanel.gameObject.SetActive(true);
            nextlevelButton.gameObject.SetActive(true);
            coinText.text = "x" + (gameStorageSO.GameBaseParameters.BaseLevelProfit + gameStorageSO.GameBaseParameters.WinLevelProfit).ToString();
            playerStorageSO.ConcretePlayer.Coins += gameStorageSO.GameBaseParameters.BaseLevelProfit + gameStorageSO.GameBaseParameters.WinLevelProfit;
            dependencyContainerSO.LevelGenerator.SetLevel();
            //Metrics.EndLevelEvent(playerStorageSO.ConcretePlayer.PlayerCurrentLevel, "win", (int)(Time.time - dependencyContainerSO.PlayerController.StartLevelTime));
        }
        else
        {
            losePanel.gameObject.SetActive(true);
            restartButton.gameObject.SetActive(true);
            coinText.text = "x" + gameStorageSO.GameBaseParameters.BaseLevelProfit.ToString();
            playerStorageSO.ConcretePlayer.Coins += gameStorageSO.GameBaseParameters.BaseLevelProfit;
            //Metrics.EndLevelEvent(playerStorageSO.ConcretePlayer.PlayerCurrentLevel, "lose", (int)(Time.time - dependencyContainerSO.PlayerController.StartLevelTime));
        }
        InfoBarController.FillUserInfoPanelAction?.Invoke();
    }


    #region IInterfacePanel
    public UIPanelType UIPanelType { get => uiPanelType; }

	public void Hide() {
		panelContainer.gameObject.SetActive(false);
    }

	public void Show() {
		panelContainer.gameObject.SetActive(true);
    }

	public void Init() {
		UIController.InterfacePanels.Add(this);
	}
	#endregion
}
