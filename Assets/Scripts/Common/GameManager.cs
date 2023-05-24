using System;

using UnityEngine;

public class GameManager : MonoBehaviour
{
	[Header("Base")]
	[SerializeField] private PlayerStorage playerStorageSO = default;
	[SerializeField] private DependencyContainer dependencyContainerSO = default;
	[SerializeField] private LevelStorage levelStorageSO = default;

	[Header("Global Actions")]
	public static Action GameStartAction = default;
	public static Action PlayerLoadedAction = default;
	public static Action PrepereLevelAction = default;
	public static Action LevelStartAction = default;
	public static Action<bool> LevelFinishAction = default;
	
	[Header("Variables")]
	private float startLevelTime = default;
	private bool levelStarted = false;

	private void OnEnable()
	{
		LevelStartAction += StartLevel;
		LevelFinishAction += FinishLevel;
		PlayerLoadedAction += PlayMusic;
	}

	private void Start()
	{
		GameStartAction?.Invoke();
	}

	private void OnDisable()
	{
		LevelStartAction -= StartLevel;
		LevelFinishAction -= FinishLevel;
		PlayerLoadedAction -= PlayMusic;
		playerStorageSO.SavePlayer();
	}

	private void OnDestroy()
	{
		playerStorageSO.SavePlayer();
	}

	private void OnApplicationQuit()
	{
		playerStorageSO.SavePlayer();
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			playerStorageSO.SavePlayer();
		}
	}

	private void PlayMusic()
	{
		SoundManager.PlaySomeSoundContinuous?.Invoke(SoundType.MainMelody, () => true);
	}

	private void StartLevel()
	{
		startLevelTime = Time.time;
		levelStarted = true;
	}

	private void FinishLevel(bool win)
	{
		if (levelStarted)
		{
			//if (playerStorageSO.PlayerCanRate())
			//{
			//	UIController.ShowUIPanelAlongAction(UIPanelType.PU_RateUs);
			//}

			levelStarted = false;
		}
	}
}
