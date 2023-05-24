using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapController : MonoBehaviour
{
    [SerializeField] private WorldType worldType = default;
    [SerializeField] private DependencyContainer dependencyContainerSO = default;
    [SerializeField] private ParticleSystem playerDestroyParticle = default;
    [SerializeField] private ParticleSystem enemyDestroyParticle = default;

    private ParticleSystem currentParticle = default;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy")
        {
            dependencyContainerSO.GameStarted = false;
            dependencyContainerSO.PlayerController.PlayerWheelParticle(false);
            dependencyContainerSO.PlayerController.RedZoneWheelParticle(false);
            dependencyContainerSO.PlayerController.DownCar();
            dependencyContainerSO.EnemyController.EnemyWheelParticle(false);
            dependencyContainerSO.EnemyController.StartMotor(false);
            dependencyContainerSO.EnemyController.StopCarShake();

            if (other.gameObject.tag == "Player")
            {
                currentParticle = playerDestroyParticle;
                dependencyContainerSO.PlayerController.MotorBurnParticleStatus(false);
                CarDestroy(dependencyContainerSO.EnemyController.transform, dependencyContainerSO.PlayerController.transform, false);
                CameraController.SetPlayerTrapPositionAction?.Invoke();
            }
            else if (other.gameObject.tag == "Enemy")
            {
                currentParticle = enemyDestroyParticle;
                dependencyContainerSO.EnemyController.MotorBurnParticleStatus(false);
                CarDestroy(dependencyContainerSO.PlayerController.transform, dependencyContainerSO.EnemyController.transform, true);
                CameraController.SetEnemyTrapPositionAction?.Invoke();
            }
            //GamePanelController.CheckRedZoneAction?.Invoke(false);
            dependencyContainerSO.EngineController.CheckRedZone(false);
            GamePanelController.EngineRawContainerStatusAction?.Invoke(false);
        }
    }

    private void CarDestroy(Transform winner, Transform loser, bool win)
    {
        GamePanelController.DissableButtonsAction?.Invoke();
        winner.DOMoveZ(winner.position.z + 10f, 2f);

        float pos = 0;
        if (worldType == WorldType.Beta)
        {
            pos = transform.position.z + 1;
        }
        else
        {
            pos = transform.position.z;
        }
        loser.DOMoveZ(pos, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            if (win)
            {
                dependencyContainerSO.EnemyController.EnemyCanvas.gameObject.SetActive(false);
            }
            else
            {
                dependencyContainerSO.PlayerController.PlayerCanvas.gameObject.SetActive(false);
            }
            currentParticle.Play();

            float rotateAngle = 0;
            if (worldType == WorldType.Beta)
            {
                loser.DOMoveZ(loser.position.z - 1f, 0.5f).SetEase(Ease.Linear);
                rotateAngle = -30f;
            }
            else
            {
                rotateAngle = -90f;
            }

            loser.DORotate(new Vector3(rotateAngle, 0, 0), 0.3f).OnComplete(() =>
            {
                if (worldType == WorldType.Alpha || worldType == WorldType.Beta)
                {
                    if (win)
                    {
                        if (worldType == WorldType.Alpha)
                        {
                            dependencyContainerSO.EnemyController.DestroyShake();
                        }
                        dependencyContainerSO.EnemyController.EnemyDestroy();
                    }
                    else
                    {
                        if (worldType == WorldType.Alpha)
                        {
                            dependencyContainerSO.PlayerController.DestroyShake();
                        }
                        dependencyContainerSO.PlayerController.PlayerDestroy();
                    }
                }
                else if (worldType == WorldType.Gamma)
                {
                    loser.DORotate(loser.eulerAngles + new Vector3(0, 0, -360f), 1f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(3);
                }

                float yMoveSpeed = 0;
                if (worldType == WorldType.Beta)
                {
                    yMoveSpeed = 3f;
                    loser.DORotate(new Vector3(-90, 0, 0), yMoveSpeed).SetEase(Ease.Linear);
                }
                else
                {
                    yMoveSpeed = 2f;
                }

                loser.DOMoveY(loser.position.y - 2f, yMoveSpeed).SetEase(Ease.Linear).OnComplete(() =>
                 {
                     currentParticle.Stop();
                     GameManager.LevelFinishAction?.Invoke(win);
                 });
            });
        });
    }
}
