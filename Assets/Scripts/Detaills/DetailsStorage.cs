using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/DetailsStorage", fileName = "DetailsStorage")]
public class DetailsStorage : ScriptableObject
{
    [SerializeField] private List<Detail> details = default;

    #region Geters/Seters
    public List<Detail> Details { get => details; }
    #endregion
}

[System.Serializable]
public class Detail
{
    [Header("Base")]
    public WorldType worldType = default;
    public DetailType detailType = default;
    public PresetType presetType = default;
    public Sprite sprite = default;
    public Color backgroundColor = default;

    [Header("Properties")]
    public float motorPower = default;
    public float strength = default;
    public float friction = default;
}
