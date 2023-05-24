using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class Player
{
	[Header("World Block")]
	[SerializeField] private WorldType worldType = default;
	[SerializeField] private LevelNameType levelNameType = default;

	[Header("Player Currencies Block")]
	[SerializeField] private int coins = 0;
	[SerializeField] private int coinsTmp = 0;

	[Header("Settings")]
	[SerializeField] private bool isMusicOn = true;
	[SerializeField] private bool isSoundOn = true;
	[SerializeField] private bool isVibrationcOn = true;

	[Header("Details")]
	[SerializeField] private List<Detail> playerDetails = new List<Detail>();
	[SerializeField] private List<Detail> installedDetails = new List<Detail>();
	[SerializeField] private List<Detail> newDetailItems = new List<Detail>();
	[SerializeField] private bool hasNewDetails = false;

	[Header("Launch")]
	[SerializeField] private bool firstLaunch = true;
	[SerializeField] private bool firstGacha = true;
	[SerializeField] private bool tutorial = false;

	[Header("Player Achivemant")]
	[SerializeField] private int playerCurrentLevel = 0;

	[Header("RateUS")]
	[SerializeField] private bool isRateUs = false;
	[SerializeField] private JsonDateTime dateLastRateTry = new JsonDateTime();

	[Header("Player Last Session")]
	[SerializeField] private JsonDateTime timeLastSession = new JsonDateTime();

	[Header("No ADS")]
	[SerializeField] private bool isBuyADSOffer = false;

	[Header("Actions")]
	public static Action PlayerChangeCoinAction = default;

	#region Geters/Seters

	public WorldType WorldType { get => worldType; set => worldType = value; }
	public LevelNameType LevelNameType { get => levelNameType; set => levelNameType = value; }
	public int Coins { get => coins; set => coins = value; }
	public int CoinsTmp { get => coinsTmp; set => coinsTmp = value; }
	public List<Detail> PlayerDetails  { get => playerDetails; set => playerDetails = value; }
	public List<Detail> InstalledDetails  { get => installedDetails; set => installedDetails = value; }
	public List<Detail> NewDetailItems { get => newDetailItems; set => newDetailItems = value; }
	public JsonDateTime DateLastRateTry { get => dateLastRateTry; set => dateLastRateTry = value; }
	public bool IsRateUs { get => isRateUs; set => isRateUs = value; }
	public JsonDateTime TimeLastSession { get => timeLastSession; set => timeLastSession = value; }
	public bool IsBuyADSOffer { get => isBuyADSOffer; }
	public bool IsMusicOn { get => isMusicOn; }
	public bool IsSoundOn { get => isSoundOn; }
	public bool IsVibrationcOn { get => isVibrationcOn; }
	public bool FirstLaunch { get => firstLaunch; set => firstLaunch = value; }
	public bool FirstGacha { get => firstGacha; set => firstGacha = value; }
	public bool Tutorial { get => tutorial; set => tutorial = value; }
	public bool HasNewDetails { get => hasNewDetails; set => hasNewDetails = value; }
	public int PlayerCurrentLevel { get => playerCurrentLevel; set => playerCurrentLevel = value; }
	#endregion

	public void SetSoundStatus(bool _status)
	{
		isSoundOn = _status;
	}

	public void SetMusicStatus(bool _status)
	{
		isMusicOn = _status;
	}

	public void SetVibrationtatus(bool _status)
	{
		isVibrationcOn = _status;
	}


}
