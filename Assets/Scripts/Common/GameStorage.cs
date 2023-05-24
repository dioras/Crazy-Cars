using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/GameStorage", fileName = "GameStorage")]
public class GameStorage : ScriptableObject
{
	[Header("GameBaseParameters")]
	[SerializeField] private GameBaseParameters gameBaseParameters = default;

	#region Geters/Seters
	public GameBaseParameters GameBaseParameters { get => gameBaseParameters; }

	#endregion
}

[Serializable]
public class GameBaseParameters
{
	[Header("Characteristics")]
	[SerializeField] private float baseMotorPower = 0f;
	[SerializeField] private float baseStrenght = 0f;
	[SerializeField] private float baseFriction = 0f;

	[Header("Level")]
	[SerializeField] private int baseLevelProfit = 100;
	[SerializeField] private int winLevelProfit = 100;

	[Header("Gachapon")]
	[SerializeField] private int baseGachaponCoast = 200;

	[Header("UI Block")]
	[SerializeField] private float timeToInteractUI = 0.3f;
	[SerializeField] private float timeToShowHiddenButton = 3f;

	[Header("Rate US Block")]
	[SerializeField] private int reRateUsDelta = 259200;
	[SerializeField] private int playerLevelToRateUs = 1;

	[Header("ADS Block")]
	[SerializeField] private int playerLevelToInterstitial = 3;

	[Header("Formating")]
	[SerializeField] private string formatingMoneyString = "{0:c}";

	[Header("Camera Positions Coordinates")]
	[SerializeField] private Vector3 baseCameraPosition = Vector3.zero;
	[SerializeField] private Vector3 baseCameraRotation = Vector3.zero;
	[Header("")]
	[SerializeField] private Vector3 gameCameraPosition = Vector3.zero;
	[SerializeField] private Vector3 gameCameraRotation = Vector3.zero;
	[Header("")]
	[SerializeField] private Vector3 puzzleCameraPosition = Vector3.zero;
	[SerializeField] private Vector3 puzzleCameraRotation = Vector3.zero;

	#region Geters/Seters
	public float BaseMotorPower { get => baseMotorPower; }
	public float BaseStrenght { get => baseStrenght; }
	public float BaseFriction { get => baseFriction; }
	public int BaseLevelProfit { get => baseLevelProfit; }
	public int WinLevelProfit { get => winLevelProfit; }
	public int BaseGachaponCoast { get => baseGachaponCoast; }
	public float TimeToShowHiddenButton { get => timeToShowHiddenButton; }
	public float TimeToInteractUI { get => timeToInteractUI; }
	public int ReRateUsDelta { get => reRateUsDelta; }
	public int PlayerLevelToRateUs { get => playerLevelToRateUs; }
	public int PlayerLevelToInterstitial { get => playerLevelToInterstitial; }
	public string FormatingMoneyString { get => formatingMoneyString; }
	public Vector3 BaseCameraPosition { get => baseCameraPosition; }
	public Vector3 BaseCameraRotation { get => baseCameraRotation; }
	public Vector3 GameCameraPosition { get => gameCameraPosition; }
	public Vector3 GameCameraRotation { get => gameCameraRotation; }
	public Vector3 PuzzleCameraPosition { get => puzzleCameraPosition; }
	public Vector3 PuzzleCameraRotation { get => puzzleCameraRotation; }

	#endregion
}


