using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_DetailItem : MonoBehaviour
{
    [SerializeField] private DependencyContainer dependencyContainerSO = default;
    [SerializeField] private PlayerStorage playerStorageSO = default;
    [SerializeField] private Image spriteImage = default;
    [SerializeField] private Image background = default;
    [SerializeField] private Image border = default;
    [SerializeField] private Image newDetailMarker = default;
    [SerializeField] private TextMeshProUGUI powerText = default;

    [SerializeField] private Button button = default;

    public Detail CurrentDetail { get; set; }

    public bool Owned { get; set; }
    public bool Installed { get; set; }

    private Canvas tutorialCanvas = default;
    private GraphicRaycaster tutorialRaycaster = default;
    private Tween tutorialButtonTween = default;

    private void Awake()
    {
        PrepareButtons();
    }

    private void PrepareButtons()
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => {
            SoundManager.PlaySomeSoundOnce(SoundType.ButtonClick);
            VibrationController.Vibrate(30);
            dependencyContainerSO.PlayerController.SetDetail(CurrentDetail);
            GaragePanelController.CheckInstalledDetailsAction?.Invoke();
            NewDetailMarkerStatus(false);
            playerStorageSO.ConcretePlayer.NewDetailItems.Remove(CurrentDetail);

            if (playerStorageSO.ConcretePlayer.FirstLaunch)
            {
                StartCoroutine(ExitTutorialCoroutine());
            }

            if (tutorialButtonTween != null)
            {
                tutorialButtonTween.Kill();
            }
            transform.localScale = Vector3.one;
        });
    }

    public void SetDetail(Detail detail)
    {
        CurrentDetail = detail;
        spriteImage.sprite = detail.sprite;
        NewDetailMarkerStatus(false);
        background.color = new Color(detail.backgroundColor.r, detail.backgroundColor.g, detail.backgroundColor.b, 1f);

        if (tutorialButtonTween != null)
        {
            tutorialButtonTween.Kill();
        }
        transform.localScale = Vector3.one;
        powerText.text = "+" + detail.motorPower.ToString();
    }

    public void SetInstalled(bool installed)
    {
        Installed = installed;
        border.gameObject.SetActive(installed);
    }

    public void NewDetailMarkerStatus(bool status)
    {
        newDetailMarker.gameObject.SetActive(status);
    }

    public void StartTutorial()
    {
        tutorialCanvas = gameObject.AddComponent<Canvas>();
        tutorialRaycaster = gameObject.AddComponent<GraphicRaycaster>();
        tutorialCanvas.overrideSorting = true;
        tutorialCanvas.sortingOrder = 10;

        tutorialButtonTween = transform.DOScale(transform.localScale * 1.1f, 0.3f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }

    public void ExitTutorial()
    {
        tutorialButtonTween.Kill();
        tutorialCanvas.overrideSorting = false;
        playerStorageSO.ConcretePlayer.FirstLaunch = false;
        GameManager.PrepereLevelAction?.Invoke();
        UIController.ShowUIPanelAloneAction?.Invoke(UIPanelType.Game);
    }

    private IEnumerator ExitTutorialCoroutine()
    {
        yield return new WaitForSeconds(0.3f + 1f);
        ExitTutorial();
    }
}
