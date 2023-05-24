using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private DependencyContainer dependencyContainerSO = default;
    [SerializeField] private PlayerStorage playerStorageSO = default;
    [SerializeField] private GameStorage gameStorageSO = default;
    [SerializeField] private DetailsStorage detailsStorageSO = default;
    [SerializeField] private Transform shakeContainer = default;
    [SerializeField] private Transform playerContainer = default;
    [SerializeField] private float baseSpeedValue = default;

    [Header("Details")]
    [SerializeField] private List<CarBody> carBodies = default;
    [SerializeField] private List<CarDetail> carDetails = default;

    [Header("Wheels")]
    [SerializeField] private Transform baseWheelsContainer = default;
    [SerializeField] private List<Transform> baseWheels = default;

    [Header("Engine")]
    [SerializeField] private List<Transform> baseEngines = default;

    [Header("Positions")]
    [SerializeField] private Vector3 previewPosition = default;
    [SerializeField] private Vector3 previewRotation = default;
    [SerializeField] private Vector3 gamePosition = default;
    [SerializeField] private Vector3 gameRotationn = default;

    [Header("Rope")]
    [SerializeField] private Transform rope = default;

    [Header("First World Wheel Particles")]
    [SerializeField] private List<ParticleSystem> firstWorldwheelParticles = default;
    [SerializeField] private List<ParticleSystem> firstWorldForwardWheelParticles = default;
    [SerializeField] private List<ParticleSystem> firstWorldZoneWheelParticles = default;

    [Header("Second World Wheel Particles")]
    [SerializeField] private List<ParticleSystem> secondWorldwheelParticles = default;
    [SerializeField] private List<ParticleSystem> secondWorldForwardWheelParticles = default;
    [SerializeField] private List<ParticleSystem> secondWorldZoneWheelParticles = default;


    [Header("Third World Wheel Particles")]
    [SerializeField] private List<ParticleSystem> thirdWorldwheelParticles = default;
    [SerializeField] private List<ParticleSystem> thirdWorldForwardWheelParticles = default;
    [SerializeField] private List<ParticleSystem> thirdWorldZoneWheelParticles = default;

    [Header("Other Particles")]
    [SerializeField] private ParticleSystem destroyDetailParticles = default;
    [SerializeField] private ParticleSystem explosionParticles = default;
    [SerializeField] private ParticleSystem motorBurnParticle = default;

    [Header("DestroyParticle Positions")]
    [SerializeField] private Vector3 motorDestroyPosition = default;
    [SerializeField] private Vector3 pipeDestroyPosition = default;
    [SerializeField] private Vector3 wheelDEstroyPosition = default;
    [SerializeField] private Vector3 bullDEstroyPosition = default;

    [Header("Camera Positions")]
    [SerializeField] private Transform explosionCamerapPosition = default;

    [Header("Canvas")]
    [SerializeField] private Canvas playerCanvas = default;

    [Header("Actions")]
    public static Action PlayerMoveAction = default;
    public static Action PlayerDestroyAction = default;

    public float BaseMotorPower { get; set; }
    public float RedZoneMotorPower { get; set; }
    public float CurrentMotorPower { get; set; }
    public float CurrentStrenght { get; set; }
    public float CurrentFriction { get; set; }
    public bool LastDetail { get; set; }
    public CarBody CurrentBody { get; set; }
    public List<Transform> CurrentWheels { get; set; }
    public Canvas PlayerCanvas { get => playerCanvas; set => playerCanvas = value; }
    public bool CarIsUp { get; set; }

    private bool particleBusy = default;
    private Tween carUpTween = default;
    private Tween carDownTween = default;
    private Tween carShakeTween = default;
    private Tween carDestroyShakeTween = default;

    private List<ParticleSystem> wheelParticles = default;
    private List<ParticleSystem> forwardWheelParticles = default;
    private List<ParticleSystem> redZoneWheelParticles = default;

    public float StartLevelTime { get; set; }

    private void Awake()
    {
        dependencyContainerSO.GameStarted = false;
        dependencyContainerSO.PlayerController = this;
    }

    private void OnEnable()
    {
        GameManager.PrepereLevelAction += PrepereForLevel;
        //EngineController.CarExplosionAction += CarExplosion;
        //EnemyController.EnemyCarExplosionAction += StopCar;
        PlayerDestroyAction += PlayerDestroy;
    }

    private void OnDisable()
    {
        GameManager.PrepereLevelAction -= PrepereForLevel;
        //EngineController.CarExplosionAction -= CarExplosion;
        //EnemyController.EnemyCarExplosionAction -= StopCar;
        PlayerDestroyAction -= PlayerDestroy;
    }

    private void PrepereForLevel()
    {
        SetGamePosition();
        transform.eulerAngles = Vector3.zero;
        shakeContainer.localEulerAngles = Vector3.zero;
        playerContainer.localEulerAngles = Vector3.zero;
        SetWheelParticles();
        PlayerWheelParticle(false);
        RedZoneWheelParticle(false);
        explosionParticles.Stop();
        motorBurnParticle.Stop();
        //LastDetail = false;
        carUpTween.Kill();
        carDownTween.Kill();
        carDestroyShakeTween.Kill();
        StopCarShake();
        SetCharacteristics();
        rope.gameObject.SetActive(true);
        playerCanvas.gameObject.SetActive(true);
        CurrentBody.SetPartsParameters();
        CarIsUp = false;
        ForwardWheelParticle(true);
    }

    private void SetCharacteristics()
    {
        float motorPower = 0;
        float strength = 0;
        float friction = 0;

        foreach (var detail in playerStorageSO.ConcretePlayer.InstalledDetails)
        {
            motorPower += detail.motorPower;
            strength += detail.strength;
            friction += detail.friction;
        }
        CurrentMotorPower = gameStorageSO.GameBaseParameters.BaseMotorPower + motorPower;
        CurrentStrenght = gameStorageSO.GameBaseParameters.BaseStrenght + strength;
        CurrentFriction = gameStorageSO.GameBaseParameters.BaseFriction + friction;

        BaseMotorPower = CurrentMotorPower;
        RedZoneMotorPower = CurrentMotorPower * 1.5f;
    }

    private void SetDefaultRotation()
    {
        shakeContainer.localEulerAngles = playerContainer.localEulerAngles = Vector3.zero;
    }

    private void HideBodies()
    {
        foreach (var body in carBodies)
        {
            body.gameObject.SetActive(false);
        }
    }

    public void SetBody()
    {
        transform.eulerAngles = Vector3.zero;
        shakeContainer.localEulerAngles = Vector3.zero;
        playerContainer.localEulerAngles = Vector3.zero;

        playerCanvas.gameObject.SetActive(true);
        MotorBurnParticleStatus(false);
        RedZoneWheelParticle(false);
        RedZoneParticle(false);
        rope.gameObject.SetActive(false);
        dependencyContainerSO.GameStarted = false;
        explosionParticles.Stop();
        motorBurnParticle.Stop();
        carUpTween.Kill();
        carDownTween.Kill();
        carDestroyShakeTween.Kill();
        StopCarShake();
        SetDefaultRotation();
        HideBodies();
        CurrentBody = carBodies[(int)playerStorageSO.ConcretePlayer.WorldType];
        carBodies[(int)playerStorageSO.ConcretePlayer.WorldType].gameObject.SetActive(true);
        CurrentBody.SetPartsParameters();
        SetWheels();
        SetEngine();
    }

    private void HideDetails()
    {
        foreach (var detail in carDetails)
        {
            detail.gameObject.SetActive(false);
        }
    }

    public void SetDetails()
    {
        HideDetails();

        for (int i = 0; i < playerStorageSO.ConcretePlayer.InstalledDetails.Count; i++)
        {
            var temp = carDetails.Find(detail => detail.WorldType == playerStorageSO.ConcretePlayer.InstalledDetails[i].worldType && detail.DetailType == playerStorageSO.ConcretePlayer.InstalledDetails[i].detailType && detail.PresetType == playerStorageSO.ConcretePlayer.InstalledDetails[i].presetType);
            temp.gameObject.SetActive(true);
        }
    }

    public void AddDetail(Detail detail)
    {
        playerStorageSO.ConcretePlayer.PlayerDetails.Add(detail);
        playerStorageSO.ConcretePlayer.HasNewDetails = true;
        GaragePanelController.SetNewDetailAction?.Invoke(detail);
    }

    public void SetDetail(Detail detail)
    {
        if (playerStorageSO.ConcretePlayer.InstalledDetails.Any(det => det.detailType == detail.detailType)) 
        {
            var temp = playerStorageSO.ConcretePlayer.InstalledDetails.Find(det => det.detailType == detail.detailType);
            var currentCarDetail = carDetails.Find(det => det.WorldType == temp.worldType && det.DetailType == temp.detailType && det.PresetType == temp.presetType);
            currentCarDetail.HideDetail();

            int index = playerStorageSO.ConcretePlayer.InstalledDetails.FindIndex(det => det.detailType == temp.detailType);

            if (playerStorageSO.ConcretePlayer.InstalledDetails.Any(det => det.detailType == detail.detailType && det.presetType == detail.presetType))
            {
                var removeCarDetail = carDetails.Find(det => det.WorldType == detail.worldType && det.DetailType == detail.detailType && det.PresetType == detail.presetType);
                removeCarDetail.HideDetail();
                if (removeCarDetail.DetailType == DetailType.Wheel)
                {
                    baseWheelsContainer.gameObject.SetActive(true);
                    CurrentWheels = baseWheels;
                }
                else if (removeCarDetail.DetailType == DetailType.Motor)
                {
                    foreach (var engine in baseEngines)
                    {
                        if (engine != null)
                        {
                            engine.gameObject.SetActive(true);
                        }
                    }
                }
                playerStorageSO.ConcretePlayer.InstalledDetails.RemoveAt(index);
                return;
            }
            else
            {
                playerStorageSO.ConcretePlayer.InstalledDetails.RemoveAt(index);
            }

            playerStorageSO.ConcretePlayer.InstalledDetails.Add(detail);
        }
        else
        {
            playerStorageSO.ConcretePlayer.InstalledDetails.Add(detail);
        }

        var newCarDetail = carDetails.Find(det => det.WorldType == detail.worldType && det.DetailType == detail.detailType && det.PresetType == detail.presetType);
        newCarDetail.ShowDetail(playerStorageSO.ConcretePlayer.WorldType);
        if (newCarDetail.DetailType == DetailType.Wheel)
        {
            CurrentWheels = newCarDetail.Wheels;
            baseWheelsContainer.gameObject.SetActive(false);
        }
        else if (newCarDetail.DetailType == DetailType.Motor)
        {
            foreach (var engine in baseEngines)
            {
                if (engine != null)
                {
                    engine.gameObject.SetActive(false);
                }
            }
        }
    }

    private void SetWheels()
    {
        baseWheelsContainer.gameObject.SetActive(!playerStorageSO.ConcretePlayer.InstalledDetails.Any(detail => detail.detailType == DetailType.Wheel));
        if (playerStorageSO.ConcretePlayer.InstalledDetails.Any(detail => detail.detailType == DetailType.Wheel))
        {
            var temp = playerStorageSO.ConcretePlayer.InstalledDetails.Find(det => det.detailType == DetailType.Wheel);
            var currentCarDetail = carDetails.Find(det => det.WorldType == temp.worldType && det.DetailType == temp.detailType && det.PresetType == temp.presetType);
            CurrentWheels = currentCarDetail.Wheels;
        }
        else
        {
            CurrentWheels = baseWheels;
        }
    }

    private void SetEngine()
    {
        foreach (var engine in baseEngines)
        {
            if (engine != null)
            {
                engine.gameObject.SetActive(!playerStorageSO.ConcretePlayer.InstalledDetails.Any(detail => detail.detailType == DetailType.Motor));
            }
        }
    }

    private void SetWheelParticles()
    {
        switch (playerStorageSO.ConcretePlayer.WorldType)
        {
            case WorldType.Alpha:
                wheelParticles = firstWorldwheelParticles;
                forwardWheelParticles = firstWorldForwardWheelParticles;
                redZoneWheelParticles = firstWorldZoneWheelParticles;
                break;
            case WorldType.Beta:
                wheelParticles = secondWorldwheelParticles;
                forwardWheelParticles = secondWorldForwardWheelParticles;
                redZoneWheelParticles = secondWorldZoneWheelParticles;
                break;
            case WorldType.Gamma:
                wheelParticles = thirdWorldwheelParticles;
                forwardWheelParticles = thirdWorldForwardWheelParticles;
                redZoneWheelParticles = thirdWorldZoneWheelParticles;
                break;
            default:
                break;
        }
    }

    private void ForwardWheelParticle(bool play)
    {
        foreach (var particle in forwardWheelParticles)
        {
            particle.gameObject.SetActive(play);
        }
    }

    public void PlayerWheelParticle(bool play)
    {
        if (play)
        {
            if (!particleBusy)
            {
                particleBusy = true;
                foreach (var particle in wheelParticles)
                {
                    if (particle != null)
                    {
                        particle.Play();
                    }
                }
            }
        }
        else
        {
            particleBusy = false;
            foreach (var particle in wheelParticles)
            {
                if (particle != null)
                {
                    particle.Stop();
                }
            }
        }
    }

    public void RedZoneWheelParticle(bool play)
    {
        if (redZoneWheelParticles != null)
        {
            foreach (var particle in redZoneWheelParticles)
            {
                if (play)
                {
                    particle.Play();
                }
                else
                {
                    particle.Stop();
                }

            }
        }
    }

    public void MotorBurnParticleStatus(bool status)
    {
        if (status)
        {
            motorBurnParticle.Play();
        }
        else
        {
            motorBurnParticle.Stop();
        }
    }

    //public void DestroyDetail()
    //{
    //    if (playerStorageSO.ConcretePlayer.InstalledDetails.Count != 0)
    //    {

    //        var det = carDetails.Find(detail => detail.DetailType == playerStorageSO.ConcretePlayer.InstalledDetails[0].detailType && detail.PresetType == playerStorageSO.ConcretePlayer.InstalledDetails[0].presetType);
    //        switch (det.DetailType)
    //        {
    //            case DetailType.Wheel:
    //                destroyDetailParticles.transform.position = wheelDEstroyPosition;
    //                break;
    //            case DetailType.BodyKit:
    //                destroyDetailParticles.transform.position = bullDEstroyPosition;
    //                break;
    //            case DetailType.Motor:
    //                destroyDetailParticles.transform.position = motorDestroyPosition;
    //                break;
    //            case DetailType.Exhaust:
    //                destroyDetailParticles.transform.position = pipeDestroyPosition;
    //                break;
    //            default:
    //                break;
    //        }
    //        destroyDetailParticles.Play();
    //        det.gameObject.SetActive(false);

    //        //playerStorageSO.ConcretePlayer.PlayerDetails.Remove(playerStorageSO.ConcretePlayer.PlayerDetails.Find(detail => detail.detailType == playerStorageSO.ConcretePlayer.InstalledDetails[0].detailType && detail.presetType == playerStorageSO.ConcretePlayer.InstalledDetails[0].presetType));
    //        //playerStorageSO.ConcretePlayer.InstalledDetails.Remove(playerStorageSO.ConcretePlayer.InstalledDetails[0]);
    //        SetCharacteristics();
    //    }
    //}

    public void DestroyDetail()
    {
        CurrentBody.DestroyPart(false, true);
    }

    public void DestroyDetailParticle(Vector3 position)
    {
        destroyDetailParticles.transform.position = position;
        destroyDetailParticles.Play();
    }

    public void UpCar()
    {
        ForwardWheelParticle(false);
        CarIsUp = true;
        RedZoneParticle(true);
        RedZoneWheelParticle(true);
        carUpTween = playerContainer.DORotate(new Vector3(-15f, playerContainer.localEulerAngles.y, playerContainer.localEulerAngles.z), 0.3f);
    }
    public void DownCar()
    {
        ForwardWheelParticle(true);
        RedZoneParticle(false);
        RedZoneWheelParticle(false);
        carDownTween = playerContainer.DORotate(new Vector3(0, playerContainer.localEulerAngles.y, playerContainer.localEulerAngles.z), 0.2f).OnComplete(()=> { CarIsUp = false; });
    }

    private void RedZoneParticle(bool start)
    {
        for (int i = 0; i < playerStorageSO.ConcretePlayer.InstalledDetails.Count; i++)
        {
            var temp = carDetails.Find(detail => detail.WorldType == playerStorageSO.ConcretePlayer.InstalledDetails[i].worldType && detail.DetailType == playerStorageSO.ConcretePlayer.InstalledDetails[i].detailType && detail.PresetType == playerStorageSO.ConcretePlayer.InstalledDetails[i].presetType);
            if (start)
            {
                temp.StartRedZoneParticles();
            }
            else
            {
                temp.StopRedZoneParticles();
            }
        }
    }

    public void CarShake()
    {
        if (carShakeTween == null)
        {
            if (shakeContainer.localEulerAngles.y != 0)
            {
                carShakeTween = shakeContainer.DORotate(Vector3.zero, 0.2f).OnComplete(() =>
                {
                    carShakeTween = shakeContainer.DORotate(shakeContainer.eulerAngles + new Vector3(0, Random.Range(-2f, 2f), 0), 0.2f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
                });
            }
            else
            {
                carShakeTween = shakeContainer.DORotate(shakeContainer.eulerAngles + new Vector3(0, Random.Range(-2f, 2f), 0), 0.2f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            }
        }
    }

    public void StopCarShake()
    {
        shakeContainer.eulerAngles = Vector3.zero;
        carShakeTween.Kill();
        carShakeTween = null;
    }

    public void CarExplosion()
    {
        //StopCarShake();
        ////PlayerWheelParticle(false);
        ////CameraController.SetCameraPositionFromCoordinate?.Invoke(explosionCamerapPosition.position, explosionCamerapPosition.eulerAngles, true, CarExplosionZoom, 0.2f);
        //explosionParticles.Play();

        //foreach (var part in CurrentBody.CarParts)
        //{
        //    //detail.transform.DOMove(new Vector3(Random.Range(1f, 3f), Random.Range(1f, 3f), Random.Range(1f, 3f)), 0.3f).OnComplete(()=> { detail.Mesh.enabled = false; });
        //    Vector3 dir = Vector3.zero;
        //    if (part.transform.localPosition.x > 0)
        //    {
        //        dir = new Vector3(Random.Range(2, 3), Random.Range(2, 3), Random.Range(-2, -3));
        //    }
        //    else
        //    {
        //        dir = new Vector3(Random.Range(-2, -3), Random.Range(2, 3), Random.Range(-2, -3));
        //    }
        //    part.transform.DOLocalMove(part.transform.localPosition + dir, 0.5f);
        //    part.transform.DORotate(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)), 0.5f).OnComplete(() =>
        //    {
        //        part.Mesh.enabled = false;
        //    });
        //}

        //GamePanelController.RideTriggerStatusAction?.Invoke(false);
        //dependencyContainerSO.TugController.Move = false;
        //dependencyContainerSO.EngineController.StopCheckRedZone();
        //dependencyContainerSO.EnemyController.CurrentMotorPower *= 2f;

        StartCoroutine(CarExplosionCoroutine());
    }

    private IEnumerator CarExplosionCoroutine()
    {
        MotorBurnParticleStatus(true);
        StopCarShake();
        GamePanelController.RideTriggerStatusAction?.Invoke(false);
        GamePanelController.EngineRawContainerStatusAction?.Invoke(false);
        dependencyContainerSO.TugController.Move = false;
        dependencyContainerSO.EngineController.StopCheckRedZone();
        yield return new WaitForSeconds(1f);

        explosionParticles.Play();

        foreach (var part in CurrentBody.CarParts)
        {
            //detail.transform.DOMove(new Vector3(Random.Range(1f, 3f), Random.Range(1f, 3f), Random.Range(1f, 3f)), 0.3f).OnComplete(()=> { detail.Mesh.enabled = false; });
            Vector3 dir = Vector3.zero;
            if (part.transform.localPosition.x > 0)
            {
                dir = new Vector3(Random.Range(7, 8), Random.Range(0, 10f), Random.Range(0, -2));
            }
            else
            {
                dir = new Vector3(Random.Range(-7, -8), Random.Range(0, 10f), Random.Range(0, -2));
            }
            part.transform.SetParent(null);
            part.transform.DOLocalMove(part.transform.localPosition + dir, Random.Range(2, 3));
            part.transform.DOLocalRotate(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)), Random.Range(2, 3)).OnComplete(() =>
            {
                part.Mesh.enabled = false;
            });
        }
        dependencyContainerSO.EnemyController.CurrentMotorPower *= 5f;
    }
        

    //private void CarExplosionZoom()
    //{
    //    StartCoroutine(CarExplosionCoroutine());
    //}

    //private IEnumerator CarExplosionCoroutine()
    //{
    //    explosionParticles.Play();
    //    yield return new WaitForSeconds(1.5f);
    //    GameManager.LevelFinishAction?.Invoke(false);
    //}

    private void StopCar()
    {
        StopCarShake();
        PlayerWheelParticle(false);
        RedZoneWheelParticle(false);
        //GamePanelController.CheckRedZoneAction?.Invoke(false);
        dependencyContainerSO.EngineController.CheckRedZone(false);
    }

    public void SetPreviewPosition()
    {
        transform.SetParent(null);
        transform.position = previewPosition;
        transform.eulerAngles = previewRotation;
    }

    public void SetGamePosition()
    {
        transform.SetParent(null);
        transform.position = gamePosition;
        transform.eulerAngles = gameRotationn;
    }

    public void DestroyShake()
    {
        carDestroyShakeTween = shakeContainer.DORotate(shakeContainer.eulerAngles + new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10)), 0.2f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }

    public void PlayerDestroy()
    {
        StartCoroutine(PlayerDestroyCoroutine());
    }

    private IEnumerator PlayerDestroyCoroutine()
    {
        CurrentBody.DestroyPart(true, true);
        yield return new WaitForSeconds(0.1f);
        CurrentBody.DestroyPart(true, true);
        yield return new WaitForSeconds(0.1f);
        CurrentBody.DestroyPart(true, true);
        yield return new WaitForSeconds(0.1f);
        CurrentBody.DestroyPart(true, true);
        yield return new WaitForSeconds(0.1f);
        CurrentBody.DestroyPart(true, true);
        yield return new WaitForSeconds(0.1f);
        CurrentBody.DestroyPart(true, true);
    }
}
