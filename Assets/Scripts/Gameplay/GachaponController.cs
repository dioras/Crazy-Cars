using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GachaponController : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private DependencyContainer dependencyContainerSO = default;
    [SerializeField] private PlayerStorage playerStorageSO = default;
    [SerializeField] private DetailsStorage detailsStorageSO = default;
    [SerializeField] private Transform gachaBody = default;
    [SerializeField] private List<CarDetail> details = default;

    [Header("Animation")]
    [SerializeField] private Animator animator = default;
    [SerializeField] private AnimationClip openClip = default;

    [Header("Particles")]
    [SerializeField] private ParticleSystem gachaParticle = default;
    [SerializeField] private ParticleSystem fogParticle = default;
    [SerializeField] private ParticleSystem landingParticle = default;
    [SerializeField] private ParticleSystem detailParticle = default;
    [SerializeField] private ParticleSystem detailBackParticle = default;

    [Header("Positions")]
    [SerializeField] private Vector3 basePosition = default;
    [SerializeField] private Vector3 fallPosition = default;

    [Header("Actions")]
    public static Action OpenedGachaAction = default;

    private int idleTrigger = Animator.StringToHash("Idle");
    private int openTrigger = Animator.StringToHash("Open");
    private int currentTrigger = default;

    private Tween detailRotateTween = default;
    private Tween gachaIdleTween = default;
    private Tween gachaRotateTween = default;
    private Tween activeGachaTween = default;
    private Detail currentDetail = default;

    private void Awake()
    {
        dependencyContainerSO.GachaponController = this;
    }

    public void PrepereForLevel()
    {
        detailBackParticle.Stop();
        fogParticle.Stop();
        gachaIdleTween.Kill();
        detailRotateTween.Kill();
        gachaRotateTween.Kill();
        activeGachaTween.Kill();
        gachaIdleTween = null;
        transform.position = basePosition;
        gachaBody.localPosition = Vector3.zero;
        gachaBody.transform.localScale = Vector3.one;
        gachaBody.transform.localEulerAngles = new Vector3(gachaBody.transform.localEulerAngles.x, gachaBody.transform.localEulerAngles.y, 0f);
        PrepereDetails();
        IdleAnimation();
    }

    public void ActiveGacha()
    {
        activeGachaTween.Kill();
        gachaBody.transform.localScale = Vector3.one;
        activeGachaTween = gachaBody.transform.DOScale(gachaBody.transform.localScale * 1.1f, 0.2f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }

    public void MenuMove()
    {
        gachaBody.transform.localEulerAngles = new Vector3(gachaBody.transform.localEulerAngles.x, gachaBody.transform.localEulerAngles.y, -10);
        gachaIdleTween = gachaBody.transform.DOMoveY(gachaBody.transform.position.y + 0.2f, 0.8f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        gachaRotateTween = gachaBody.transform.DOLocalRotate(new Vector3(gachaBody.transform.localEulerAngles.x, gachaBody.transform.localEulerAngles.y, 10), 1.6f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }

    public void PrepereDetails()
    {
        foreach (var detail in details)
        {
            detail.gameObject.SetActive(false);
            detail.gameObject.transform.localPosition = Vector3.zero;
            detail.gameObject.transform.eulerAngles = Vector3.zero;
            detail.gameObject.transform.localScale = Vector3.one;
        }
    }

    public void ShowGachapon()
    {


        transform.DOMove(fallPosition, 0.7f).OnComplete(()=> 
        {
            landingParticle.Play();
            transform.DOScaleX(transform.localScale.x + 0.2f, 0.2f);
            transform.DOScaleY(transform.localScale.y - 0.2f, 0.2f).OnComplete(() =>
            {
                transform.DOScaleX(transform.localScale.x - 0.2f, 0.2f);
                transform.DOScaleY(transform.localScale.y + 0.2f, 0.2f).OnComplete(() =>
                {
                    fogParticle.Play();
                    OpenAnimation();
                    StartCoroutine(OpenGachaponCoroutine());
                });
            });
        });
    }

    private IEnumerator OpenGachaponCoroutine()
    {
        yield return new WaitForSeconds(openClip.length + 0.1f);
        
        //gachaParticle.Play();
        //IdleAnimation();
        AddNewDetail();

        var detailBody = details.Find(detail => detail.WorldType == currentDetail.worldType && detail.DetailType == currentDetail.detailType && detail.PresetType == currentDetail.presetType);
        detailBody.transform.localScale = Vector3.zero;
        detailBody.gameObject.SetActive(true);

        detailBody.transform.DOScale(1f, 0.2f);
        detailParticle.Play();
        detailBody.transform.DOLocalMoveY(transform.position.y + 0.5f, 0.5f).OnComplete(()=> 
        {
            detailBody.transform.DOLocalMoveY(transform.position.y + 10, 1f).SetDelay(0.2f);
            transform.DOMoveY(transform.position.y - 10f, 1f).SetDelay(0.2f).OnComplete(()=> 
            {
                detailRotateTween = detailBody.gameObject.transform.DORotate(new Vector3(0, 360, 0), 15f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
                detailBody.transform.DOScale(3f, 0.5f).OnComplete(() =>
                {
                    IdleAnimation();
                    OpenedGachaAction?.Invoke();
                });
            });
        });

        
    }

    private void AddNewDetail()
    {
        int rand = 0;
        rand = Random.Range(0, detailsStorageSO.Details.Count);
        while (playerStorageSO.ConcretePlayer.PlayerDetails.Any(detail => detail.worldType == detailsStorageSO.Details[rand].worldType && detail.detailType == detailsStorageSO.Details[rand].detailType && detail.presetType == detailsStorageSO.Details[rand].presetType))
        {
            rand = Random.Range(0, detailsStorageSO.Details.Count);
        }
        currentDetail = detailsStorageSO.Details.Find(detail => detail.worldType == detailsStorageSO.Details[rand].worldType && detail.detailType == detailsStorageSO.Details[rand].detailType && detail.presetType == detailsStorageSO.Details[rand].presetType);
        dependencyContainerSO.PlayerController.AddDetail(currentDetail);
    }

    public void IdleAnimation()
    {
        animator.ResetTrigger(currentTrigger);
        animator.SetTrigger(idleTrigger);
        currentTrigger = idleTrigger;
    }

    public void OpenAnimation()
    {
        animator.ResetTrigger(currentTrigger);
        animator.SetTrigger(openTrigger);
        currentTrigger = openTrigger;
    }
}
