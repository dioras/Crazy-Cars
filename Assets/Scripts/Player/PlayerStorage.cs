using System;
using System.Linq;

using UnityEngine;

using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObjects/PlayerStorage", fileName = "PlayerStorage")]
public class PlayerStorage : ScriptableObject
{
	[Header("Basic")]
	[SerializeField] private string playerPrefsSaveString = "_playerSave";
	[SerializeField] private GameStorage gameStorageSO = default;

	[Header("ConcretePlayer")]
	[SerializeField] private Player concretePlayer = new Player();

	#region Geters/Seters
	public Player ConcretePlayer { get => concretePlayer; }
	#endregion
	public void SavePlayer()
	{
		PlayerPrefs.SetString(playerPrefsSaveString, JsonUtility.ToJson(concretePlayer));
	}

	public void LoadPlayer()
	{
		var playerString = PlayerPrefs.GetString(playerPrefsSaveString, "");
		if (playerString != "")
		{
			concretePlayer = JsonUtility.FromJson<Player>(playerString);
		}
		else
		{
			concretePlayer = new Player();
		}

		GameManager.PlayerLoadedAction?.Invoke();
		ApplyPlayerSettings();
	}

	//public void TryRateUS(RateUsResultType _rateUsResultType)
	//{
	//	concretePlayer.DateLastRateTry = DateTime.UtcNow;
	//	concretePlayer.IsRateUs = _rateUsResultType == RateUsResultType.Rate;
	//	Metrica.RateUsEvent(concretePlayer.DateLastRateTry.value == 0 ? RateUsReason.new_player : RateUsReason.after_pause_retry, _rateUsResultType);
	//}

	//public bool PlayerCanRate()
	//{
	//	return !concretePlayer.IsRateUs && concretePlayer.PlayerCurrentMission >= gameStorageSO.GameBaseParameters.PlayerLevelToRateUs &&
	//	(concretePlayer.DateLastRateTry.value == 0 ||
	//	(concretePlayer.DateLastRateTry.value != 0 && (DateTime.UtcNow - concretePlayer.DateLastRateTry).TotalSeconds >= gameStorageSO.GameBaseParameters.ReRateUsDelta));
	//}

	public bool PlayerCanViewInterstitial()
	{
		return false;
	}

	public void ApplyPlayerSettings()
	{
		SoundManager.CanPlayMusic = concretePlayer.IsMusicOn;
		SoundManager.CanPlaySounds = concretePlayer.IsSoundOn;
	}
}
