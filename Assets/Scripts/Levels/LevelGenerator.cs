using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private DependencyContainer dependencyContainerSO = default;
    [SerializeField] private PlayerStorage playerStorageSO = default;
    [SerializeField] private LevelStorage levelStorageSO = default;

    [Header("Wolrd Environment")]
    [SerializeField] private List<Transform> wolrdEnvironmentList = default;

    private Camera mainCamera = default;

    private void Awake()
    {
        dependencyContainerSO.LevelGenerator = this;   
    }

    private void OnEnable()
    {
        //GameManager.PrepereLevelAction += CreateLevel;
    }

    private void OnDisable()
    {
        //GameManager.PrepereLevelAction -= CreateLevel;
    }

    public void SetLevel()
    {
        if (playerStorageSO.ConcretePlayer.FirstLaunch)
        {
            playerStorageSO.ConcretePlayer.WorldType = WorldType.Alpha;
            playerStorageSO.ConcretePlayer.LevelNameType = LevelNameType.First;
        }
        else
        {
            var levelIndex = (int)playerStorageSO.ConcretePlayer.LevelNameType;
            levelIndex++;

            if (levelIndex <= levelStorageSO.WorldList.Find(world => world.WorldType == playerStorageSO.ConcretePlayer.WorldType).LevelsList.Count -1)
            {
                playerStorageSO.ConcretePlayer.LevelNameType = (LevelNameType)levelIndex;
            }
            else
            {
                var worldIndex = (int)playerStorageSO.ConcretePlayer.WorldType;
                worldIndex++;

                if (worldIndex <= levelStorageSO.WorldList.Count - 1)
                {
                    playerStorageSO.ConcretePlayer.WorldType = (WorldType)worldIndex;
                    playerStorageSO.ConcretePlayer.LevelNameType = LevelNameType.First;
                }
                else
                {
                    playerStorageSO.ConcretePlayer.WorldType = WorldType.Alpha;
                    playerStorageSO.ConcretePlayer.LevelNameType = LevelNameType.First;
                }
            }
        }
    }

    private void HideEnvinment()
    {
        foreach (var world in wolrdEnvironmentList)
        {
            world.gameObject.SetActive(false);
        }
    }

    public void CreateLevel()
    {
        HideEnvinment();
        wolrdEnvironmentList[(int)playerStorageSO.ConcretePlayer.WorldType].gameObject.SetActive(true);
        Camera.main.backgroundColor = levelStorageSO.WorldList.Find(world => world.WorldType == playerStorageSO.ConcretePlayer.WorldType).WorldBackgroundColor;
    }
}
