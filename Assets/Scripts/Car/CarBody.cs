using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBody : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private DependencyContainer dependencyContainerSO = default;
    [SerializeField] private WorldType worldType = default;
    [SerializeField] private Transform partsParent = default;
    [SerializeField] private List<CarPart> carParts = default;
    [SerializeField] private List<CarPart> wheelHolders = default;

    public WorldType WorldType => worldType;
    public List<CarPart> WheelHolders => wheelHolders;
    public List<CarPart> CarParts => carParts;

    public void DestroyPart(bool trap, bool player)
    {
        List<CarPart> tempParts = carParts.FindAll(part => part.IsDestroyed == false);
        if (tempParts.Count != 0)
        {
            var part = tempParts[Random.Range(0, tempParts.Count)];

            if (player)
            {
                dependencyContainerSO.PlayerController.DestroyDetailParticle(part.transform.position);
            }
            else
            {
                dependencyContainerSO.EnemyController.DestroyDetailParticle(part.transform.position);
            }

            part.IsDestroyed = true;
            if (trap)
            {
                part.transform.DOLocalMove(part.DestroyPosition, 0.7f).SetEase(Ease.Linear);
                part.transform.DORotate(part.DestroyRotation, 0.7f).SetEase(Ease.Linear).OnComplete(() => {
                    part.Mesh.enabled = false;
                });
            }
            else
            {
                Vector3 dir = Vector3.zero;
                if (part.transform.localPosition.x > 0)
                {
                    dir = new Vector3(Random.Range(7, 8), Random.Range(2, 10f), Random.Range(0, -2));
                }
                else
                {
                    dir = new Vector3(Random.Range(-7, -8), Random.Range(2, 10f), Random.Range(0, -2));
                }
                part.transform.SetParent(null);
                part.transform.DOLocalMove(part.transform.localPosition + dir, 3).OnComplete(() =>
                {
                    part.transform.DOScale(0, 1f).OnComplete(() => { part.Mesh.enabled = false; });
                });
                part.transform.DORotate(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)), 3);
            }


        }
    }

    public void SetPartsParameters()
    {
        foreach (var carPart in carParts)
        {
            carPart.transform.SetParent(partsParent);
            carPart.IsDestroyed = false;
            carPart.Mesh.enabled = true;
            carPart.transform.localScale = Vector3.one;
            carPart.SetStartParameters();
        }
    }
}
