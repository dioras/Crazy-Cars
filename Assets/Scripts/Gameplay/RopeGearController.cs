using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeGearController : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private DependencyContainer dependencyContainerSO = default;
    [SerializeField] private Transform movebleGear = default;

    public Transform MovebleGear { get => movebleGear; set => movebleGear = value; }

    private void Awake()
    {
        dependencyContainerSO.RopeGearController = this;
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
        movebleGear.eulerAngles = Vector3.zero;
    }
}
