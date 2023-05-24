using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDetail : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private WorldType worldType = default;
    [SerializeField] private DetailType detailType = default;
    [SerializeField] private PresetType presetType = default;
    [SerializeField] private List<ParticleSystem> hideParticles = default;
    [SerializeField] private List<ParticleSystem> redZoneParticles = default;

    [SerializeField] private bool needChangePosition = false;

    [Header("Positions")]
    [SerializeField] private Vector3 firstCar = default;
    [SerializeField] private Vector3 secondCar = default;
    [SerializeField] private Vector3 thirdCar = default;

    [Header("Wheel")]
    [SerializeField] private bool isWheel = false;
    [SerializeField] private List<Transform> wheels = default;


    public WorldType WorldType => worldType;
    public DetailType DetailType => detailType;
    public PresetType PresetType => presetType;
    public bool NeedChangePosition => needChangePosition;

    public bool IsWheel => isWheel;
    public List<Transform> Wheels { get => wheels; set => wheels = value; }

    public void ShowDetail(WorldType worldType)
    {
        if (needChangePosition)
        {
            switch (worldType)
            {
                case WorldType.Alpha:
                    transform.localPosition = firstCar;
                    break;
                case WorldType.Beta:
                    transform.localPosition = secondCar;
                    break;
                case WorldType.Gamma:
                    transform.localPosition = thirdCar;
                    break;
                default:
                    break;
            }
        }

        transform.position += new Vector3(0, 10f, 0);
        gameObject.SetActive(true);

        transform.DOMoveY(transform.position.y - 10f, 0.3f).OnComplete(()=> 
        {
            transform.DOScale(transform.localScale + new Vector3(0.1f, 0.1f, 0.1f), 0.1f).OnComplete(() =>
            {
                transform.DOScale(transform.localScale - new Vector3(0.1f, 0.1f, 0.1f), 0.1f);
            });
        });
    }

    public void HideDetail()
    {
        foreach (var hideParticle in hideParticles)
        {
            //hideParticle.transform.position = new Vector3(hideParticle.transform.position.x, transform.position.y, hideParticle.transform.position.z); 
            hideParticle.Play(); 
        }
        gameObject.SetActive(false);
    }

    public void StartRedZoneParticles()
    {
        if (redZoneParticles.Count != 0)
        {
            foreach (var redZoneParticle in redZoneParticles)
            {
                redZoneParticle.Play();
            }
        }
    }
    public void StopRedZoneParticles()
    {
        if (redZoneParticles.Count != 0)
        {
            foreach (var redZoneParticle in redZoneParticles)
            {
                redZoneParticle.Stop();
            }
        }
    }
}
