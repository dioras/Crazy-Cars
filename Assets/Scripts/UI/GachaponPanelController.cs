using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GachaponPanelController : MonoBehaviour, IInterfacePanel
{
    [Header("Base")]
    [SerializeField] private UIPanelType uiPanelType = UIPanelType.Gachapon;
    [SerializeField] private Transform panelContainer = default;
    [SerializeField] private LevelStorage levelStorageSO = default;
    [SerializeField] private DependencyContainer dependencyContainerSO = default;
    [SerializeField] private PlayerStorage playerStorageSO = default;
    [SerializeField] private GameStorage gameStorageSO = default;
    [SerializeField] private DetailsStorage detailsStorageSO = default;
    [SerializeField] private Color backgroundColor = default;

    [Header("Buttons")]
    [SerializeField] private Button toGarageButton = default;


    private void Awake()
    {
        PrepareButtons();
        Init();
    }

    private void OnEnable()
    {
        GachaponController.OpenedGachaAction += OpenedGacha;
    }

    private void OnDisable()
    {
        GachaponController.OpenedGachaAction -= OpenedGacha;
    }

    private void PrepareButtons()
    {
        toGarageButton.onClick.RemoveAllListeners();
        toGarageButton.onClick.AddListener(() => {
            SoundManager.PlaySomeSoundOnce?.Invoke(SoundType.ButtonClick);
            VibrationController.Vibrate(30);
            UIController.ShowUIPanelAloneAction?.Invoke(UIPanelType.Garage);
            playerStorageSO.ConcretePlayer.HasNewDetails = false;
        });
    }

    private void PreperePanel()
    {
        CameraController.SetGachaponPositionAction?.Invoke();
        toGarageButton.gameObject.SetActive(false);
    }

    private void OpenedGacha()
    {
        toGarageButton.gameObject.SetActive(true);
    }

    #region IInterfacePanel
    public UIPanelType UIPanelType { get => uiPanelType; }

    public void Hide()
    {
        panelContainer.gameObject.SetActive(false);
    }

    public void Show()
    {
        dependencyContainerSO.GachaponController.PrepereForLevel();
        PreperePanel();
        Camera.main.backgroundColor = backgroundColor;
        panelContainer.gameObject.SetActive(true);
        dependencyContainerSO.GachaponController.ShowGachapon();
    }

    public void Init()
    {
        UIController.InterfacePanels.Add(this);
    }
    #endregion
}
