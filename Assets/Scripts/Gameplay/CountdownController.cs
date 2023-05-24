using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownController : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private DependencyContainer dependencyContainerSO = default;

    [Header("Numbers")]
    [SerializeField] private Transform oneObject = default;
    [SerializeField] private Transform twoObject = default;
    [SerializeField] private Transform threeObject = default;

    private Sequence countdownSequence = default;

    private void Awake()
    {
        dependencyContainerSO.CountdownController = this;
    }

    private void OnEnable()
    {
        GameManager.PrepereLevelAction += PrepereForLevel;
    }

    private void OnDisable()
    {
        GameManager.PrepereLevelAction -= PrepereForLevel;
    }

    private void PrepereForLevel()
    {
        oneObject.transform.localEulerAngles = Vector3.zero;
        twoObject.transform.localEulerAngles = Vector3.zero;
        threeObject.transform.localEulerAngles = Vector3.zero;
        oneObject.transform.localScale = Vector3.zero;
        twoObject.transform.localScale = Vector3.zero;
        threeObject.transform.localScale = Vector3.zero;
        if (countdownSequence != null)
        {
            countdownSequence.Kill();
        }
    }

    public void StartCountdown()
    {
        CameraController.SetCountdownPositionAction?.Invoke();
        Sequence countdownSequence = DOTween.Sequence();
        countdownSequence.Append(threeObject.DOScale(1, .35f));
        countdownSequence.Append(threeObject.transform.DORotate(threeObject.localEulerAngles + new Vector3(0, 360f, 0), 0.3f, RotateMode.FastBeyond360).SetEase(Ease.Linear));
        countdownSequence.Append(threeObject.DOScale(0, .35f));
        countdownSequence.Append(twoObject.DOScale(1, .35f));
        countdownSequence.Append(twoObject.transform.DORotate(twoObject.localEulerAngles + new Vector3(0, 360f, 0), 0.3f, RotateMode.FastBeyond360).SetEase(Ease.Linear));
        countdownSequence.Append(twoObject.DOScale(0, .35f));
        countdownSequence.Append(oneObject.DOScale(1, .35f));
        countdownSequence.Append(oneObject.transform.DORotate(oneObject.localEulerAngles + new Vector3(0, 360f, 0), 0.15f, RotateMode.FastBeyond360).SetEase(Ease.Linear));
        countdownSequence.Append(oneObject.DOScale(0, .35f)).OnComplete(() => 
        {

            GamePanelController.StopCarsAction?.Invoke();
            GamePanelController.CountdownCountainerStatusAction?.Invoke(false);
            GamePanelController.EngineRawContainerStatusAction?.Invoke(true);
            GameManager.LevelStartAction?.Invoke();
            countdownSequence = null;
        });
        countdownSequence.Play();
    }
}
