using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoController : MonoBehaviour
{
    [SerializeField] private Vector3 finalPosition = default;

    private void Awake()
    {
        transform.DOLocalMove(new Vector3(finalPosition.x, transform.localPosition.y, transform.localPosition.z), 10f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }
}
