using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TugController : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private DependencyContainer dependencyContainerSO = default;
    public bool Move { get; set; }
    public bool RedZone { get; set; }

    private void Awake()
    {
        dependencyContainerSO.TugController = this;
        dependencyContainerSO.GameStarted = false;
    }

    private void OnEnable()
    {
        GameManager.PrepereLevelAction += PrepereForLevel;
        GameManager.LevelStartAction += LevelStart;
        //EngineController.CarExplosionAction += CarExplosion;
        //EnemyController.EnemyCarExplosionAction += CarExplosion;
    }

    private void OnDisable()
    {
        GameManager.PrepereLevelAction -= PrepereForLevel;
        GameManager.LevelStartAction -= LevelStart;
        //EngineController.CarExplosionAction -= CarExplosion;
        //EnemyController.EnemyCarExplosionAction -= CarExplosion;
    }

    private void PrepereForLevel()
    {
        RedZone = false;
        dependencyContainerSO.GameStarted = false;
        Move = false;
    }

    private void LevelStart()
    {
        dependencyContainerSO.GameStarted = true;
    }

    private void FixedUpdate()
    {
        if (dependencyContainerSO.GameStarted)
        {
            if (Move)
            {
                //GamePanelController.EngineSpeedUpAction?.Invoke();
                dependencyContainerSO.EngineController.EngineSpeedUp();
                dependencyContainerSO.PlayerController.PlayerWheelParticle(true);
                dependencyContainerSO.EngineController.RedZoneSliderStatus(true);
                //GamePanelController.RedZoneSliderStatusAction?.Invoke(true);

                dependencyContainerSO.PlayerController.transform.position += new Vector3(0, 0, (dependencyContainerSO.PlayerController.CurrentMotorPower - dependencyContainerSO.EnemyController.CurrentMotorPower) * 0.001f);
                foreach (var wheel in dependencyContainerSO.PlayerController.CurrentWheels)
                {
                    wheel.RotateAround(wheel.position, wheel.right, dependencyContainerSO.PlayerController.CurrentMotorPower);
                }
                dependencyContainerSO.EnemyController.transform.position -= new Vector3(0, 0, (dependencyContainerSO.PlayerController.CurrentMotorPower - dependencyContainerSO.EnemyController.CurrentMotorPower) * 0.001f);
                dependencyContainerSO.RopeGearController.MovebleGear.eulerAngles += new Vector3(0, (dependencyContainerSO.PlayerController.CurrentMotorPower - dependencyContainerSO.EnemyController.CurrentMotorPower) * 0.1f, 0);
                dependencyContainerSO.PlayerController.CarShake();
            }
            else
            {
                //GamePanelController.EngineSpeedDownAction?.Invoke();
                dependencyContainerSO.EngineController.RedZoneSliderStatus(false);
                //GamePanelController.RedZoneSliderStatusAction?.Invoke(false);
                dependencyContainerSO.EngineController.EngineSpeedDown();
                dependencyContainerSO.PlayerController.PlayerWheelParticle(false);


                dependencyContainerSO.PlayerController.transform.position -= new Vector3(0, 0, (dependencyContainerSO.EnemyController.CurrentMotorPower) * 0.001f);
                foreach (var wheel in dependencyContainerSO.PlayerController.CurrentWheels)
                {
                    wheel.RotateAround(wheel.position, -wheel.right, (dependencyContainerSO.EnemyController.CurrentMotorPower) * 0.1f);
                }
                dependencyContainerSO.EnemyController.transform.position += new Vector3(0, 0, (dependencyContainerSO.EnemyController.CurrentMotorPower) * 0.001f);
                dependencyContainerSO.RopeGearController.MovebleGear.eulerAngles -= new Vector3(0, (dependencyContainerSO.EnemyController.CurrentMotorPower) * 0.1f, 0);
                dependencyContainerSO.PlayerController.StopCarShake();
            }

            foreach (var wheel in dependencyContainerSO.EnemyController.CurrentWheels)
            {
                wheel.RotateAround(wheel.position, wheel.right, dependencyContainerSO.EnemyController.CurrentMotorPower);
            }
            dependencyContainerSO.EnemyController.EnemyWheelParticle(true);
            dependencyContainerSO.EnemyController.StartMotor(true);
            dependencyContainerSO.EnemyController.CarShake();
            //GamePanelController.CheckRedZoneAction?.Invoke(true);
            dependencyContainerSO.EngineController.CheckRedZone(true);
        }
    }

    private void CarExplosion()
    {
        dependencyContainerSO.GameStarted = false;
    }
}
