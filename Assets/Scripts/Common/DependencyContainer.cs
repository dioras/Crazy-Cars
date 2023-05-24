using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/DependencyContainer", fileName = "DependencyContainer")]
public class DependencyContainer : ScriptableObject
{
    public bool GameStarted { get; set; }
    public TugController TugController { get; set; }
    public PlayerController PlayerController { get; set; }
    public EnemyController EnemyController { get; set; }
    public GachaponController GachaponController { get; set; }
    public LevelGenerator LevelGenerator { get; set; }
    public CountdownController CountdownController { get; set; }
    public RopeGearController RopeGearController { get; set; }
    public EngineController EngineController { get; set; }
    public GarageController GarageController { get; set; }
}
