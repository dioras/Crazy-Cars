using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level
{
	[Header("Base")]
	[SerializeField] private LevelNameType levelNameType = default;

	[Header("Enemy Properties")]
	[SerializeField] private float motorPower = default;
	[SerializeField] private int destroyDetailChance = default;
	[SerializeField] private float destroyDetailPover = default;

	#region Geters/Seters

	public LevelNameType LevelNameType { get => levelNameType; }
	public float MotorPower { get => motorPower; }
	public int DestroyDetailChance { get => destroyDetailChance; }
	public float DestroyDetailPower { get => destroyDetailPover; }
	#endregion
}

