using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageController : MonoBehaviour
{
    [SerializeField] private DependencyContainer dependencyContainerSO = default;
    [SerializeField] private Transform podium = default;

    private Tween rotateTween = default;

    private void Awake()
    {
        dependencyContainerSO.GarageController = this;
    }

    public void StartRotate()
    {
        if (rotateTween != null)
        {
            rotateTween.Kill();
            rotateTween = null;
        }
        rotateTween = podium.DORotate(podium.eulerAngles + new Vector3(0, 360, 0), 10f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
    }

    public void SetCar()
    {
        dependencyContainerSO.PlayerController.transform.SetParent(podium);
    }
}
