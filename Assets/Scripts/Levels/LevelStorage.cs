using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/LevelStorage", fileName = "LevelStorage")]
public class LevelStorage : ScriptableObject
{
	[Header("Worlds")]
	[SerializeField] private List<World> worldList = default;

	#region Geters/Seters
	public List<World> WorldList { get => worldList; }
	#endregion
}

[System.Serializable]
public class World
{
	[SerializeField] private WorldType worldType = default;
	[SerializeField] private Sprite worldSprite = default;
	[SerializeField] private Color worldBackgroundColor = default;
	[Header("Levels")]
	[SerializeField] private List<Level> levelsList = default;

	#region Geters/Seters
	public WorldType WorldType { get => worldType; }
	public Sprite WorldSprite { get => worldSprite; }
	public Color WorldBackgroundColor { get => worldBackgroundColor; }
	public List<Level> LevelsList { get => levelsList; }
	#endregion
}
