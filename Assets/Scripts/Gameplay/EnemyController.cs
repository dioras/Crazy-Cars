using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private DependencyContainer dependencyContainerSO = default;
    [SerializeField] private PlayerStorage playerStorageSO = default;
    [SerializeField] private GameStorage gameStorageSO = default;
    [SerializeField] private LevelStorage levelStorageSO = default;
    [SerializeField] private NicknamesStorage nicknamesStorageSO = default;
    [SerializeField] private Transform shakeContainer = default;
    [SerializeField] private Transform enemyContainer = default;

    [Header("Details")]
    [SerializeField] private List<CarBody> carBodies = default;
    [SerializeField] private List<CarDetail> carDetails = default;

    [Header("Wheels")]
    [SerializeField] private Transform baseWheelsContainer = default;
    [SerializeField] private List<Transform> baseWheels = default;

    [Header("Engine")]
    [SerializeField] private List<Transform> baseEngines = default;

    [Header("Canvas")]
    [SerializeField] private Canvas enemyCanvas = default;
    [SerializeField] private Image enemyIcon = default;
    [SerializeField] private TextMeshProUGUI enemyNameText = default;

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

    [Header("Particles")]
    [SerializeField] private ParticleSystem explosionParticles = default;
    [SerializeField] private ParticleSystem destroyDetailParticles = default;
    [SerializeField] private ParticleSystem motorBurnParticle = default;

    [Header("Camera Positions")]
    [SerializeField] private Transform explosionCamerapPosition = default;

    [Header("Actions")]
    public static Action EnemyMoveAction = default;
    public static Action EnemyCarExplosionAction = default;
    public static Action EnemyDestroyAction = default;

    public float BaseMotorPower { get; set; }
    public float RedZoneMotorPower { get; set; }
    public float CurrentMotorPower { get; set; }
    public int DestroyDetailChance { get; set; }
    public float DestroyDetailPower { get; set; }
    public CarBody CurrentBody { get; set; }
    public List<Transform> CurrentWheels { get; set; }
    public Canvas EnemyCanvas { get => enemyCanvas; set => enemyCanvas = value; }

    private Vector3 startPosition = default;
    private bool particleBusy = default;
    private Camera mainCam = default;
    private bool startCanvas = default;
    private Tween carShakeTween = default;
    private Tween carUpTween = default;
    private Tween carDownTween = default;
    private int prevBodyColor = default;
    private Coroutine motorCoroutine = default;
    private List<CarDetail> detailsList = new List<CarDetail>();
    private Tween carDestroyShakeTween = default;
    private List<ParticleSystem> wheelParticles = default;
    private List<ParticleSystem> forwardWheelParticles = default;
    private List<ParticleSystem> redZoneWheelParticles = default;

    private void Awake()
    {
        dependencyContainerSO.EnemyController = this;
        startPosition = transform.position;
    }

    private void OnEnable()
    {
        mainCam = Camera.main;
        //GameManager.PrepereLevelAction += PrepereForLevel;
        //EngineController.CarExplosionAction += CarExplosion;
        //EnemyCarExplosionAction += EnemyCarExplosion;
        EnemyDestroyAction += EnemyDestroy;
    }

    private void OnDisable()
    {
        //GameManager.PrepereLevelAction -= PrepereForLevel;
        //EngineController.CarExplosionAction -= CarExplosion;
        //EnemyCarExplosionAction -= EnemyCarExplosion;
        EnemyDestroyAction -= EnemyDestroy;
    }

    public void PrepereForLevel()
    {
        //StartCanvasStatus(true);
        enemyCanvas.gameObject.SetActive(true);
        SetWheelParticles();
        carDestroyShakeTween.Kill();
        carUpTween.Kill();
        carDownTween.Kill();
        explosionParticles.Stop();
        transform.position = startPosition;
        transform.eulerAngles = Vector3.zero;
        enemyContainer.localEulerAngles = Vector3.zero;
        shakeContainer.localEulerAngles =  Vector3.zero;
        EnemyWheelParticle(false);
        StartMotor(false);
        StopCarShake();
        enemyIcon.sprite = nicknamesStorageSO.GetRandomIcon();
        enemyNameText.text = nicknamesStorageSO.GetRandomNickname();
        SetBody();
        SetDetails();
        SetCharacteristics();
        ForwardWheelParticle(true);
        RedZoneWheelParticle(false);
        MotorBurnParticleStatus(false);
        RedZoneParticle(false);
    }

    public void SetCharacteristics()
    {
        var temp = levelStorageSO.WorldList.Find(world => world.WorldType == playerStorageSO.ConcretePlayer.WorldType).LevelsList.Find(level => level.LevelNameType == playerStorageSO.ConcretePlayer.LevelNameType);
        CurrentMotorPower = dependencyContainerSO.PlayerController.CurrentMotorPower - (dependencyContainerSO.PlayerController.CurrentMotorPower * (temp.MotorPower/100));
        DestroyDetailChance = temp.DestroyDetailChance;
        DestroyDetailPower = temp.DestroyDetailPower;

        BaseMotorPower = CurrentMotorPower;
        RedZoneMotorPower = CurrentMotorPower * 1.5f;
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

    public void EnemyWheelParticle(bool play)
    {
        if (play)
        {
            if (!particleBusy)
            {
                particleBusy = true;
                foreach (var particle in wheelParticles)
                {
                    particle.Play();
                }
            }
        }
        else
        {
            particleBusy = false;
            foreach (var particle in wheelParticles)
            {
                particle.Stop();
            }
        }
    }

    private void ForwardWheelParticle(bool play)
    {
        foreach (var particle in forwardWheelParticles)
        {
            particle.gameObject.SetActive(play);
        }
    }

    public void StartMotor(bool start) 
    {
        if (start)
        {
            if (motorCoroutine == null)
            {
                motorCoroutine = StartCoroutine(MotorCoroutine());
            }
        }
        else
        {
            if (motorCoroutine != null)
            {
                StopCoroutine(motorCoroutine);
                motorCoroutine = null;
            }
        }
    }

    public void UpCar()
    {
        ForwardWheelParticle(false);
        //CarIsUp = true;
        RedZoneParticle(true);
        RedZoneWheelParticle(true);
        carUpTween = enemyContainer.DORotate(new Vector3(-15f, enemyContainer.localEulerAngles.y, enemyContainer.localEulerAngles.z), 0.3f);
    }
    public void DownCar()
    {
        ForwardWheelParticle(true);
        RedZoneParticle(false);
        RedZoneWheelParticle(false);
        carDownTween = enemyContainer.DORotate(new Vector3(0, enemyContainer.localEulerAngles.y, enemyContainer.localEulerAngles.z), 0.2f).OnComplete(() => 
        { 
            //CarIsUp = false;
        });
    }

    private void RedZoneParticle(bool start)
    {
        for (int i = 0; i < carDetails.Count; i++)
        {
            if (start)
            {
                carDetails[i].StartRedZoneParticles();
            }
            else
            {
                carDetails[i].StopRedZoneParticles();
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


    private IEnumerator MotorCoroutine()
    {
        yield return new WaitForSeconds(3f);
        int destroDetailChance = Random.Range(0, DestroyDetailChance);
        if (destroDetailChance == 0)
        {
            CurrentMotorPower = RedZoneMotorPower;
            UpCar();

            DestroyDetail();
            yield return new WaitForSeconds(Random.Range(0.1f, 2f));
            DestroyDetail();
            yield return new WaitForSeconds(Random.Range(0.1f, 2f));
            DestroyDetail();
            yield return new WaitForSeconds(Random.Range(0.1f, 2f));

            CurrentMotorPower = BaseMotorPower;
            DownCar();

            int explosionChance = Random.Range(0, DestroyDetailChance);

            if (destroDetailChance == 0)
            {
                EnemyCarExplosionAction?.Invoke();
            }
        }

        motorCoroutine = null;
    }

    public void DestroyDetail()
    {
        CurrentBody.DestroyPart(false, false);
    }

    public void DestroyDetailParticle(Vector3 position)
    {
        destroyDetailParticles.transform.position = position;
        destroyDetailParticles.Play();
    }

    private void EnemyCarExplosion()
    {
        CarExplosion();
        //CameraController.SetCameraPositionFromCoordinate?.Invoke(explosionCamerapPosition.position, explosionCamerapPosition.eulerAngles, true, EnemyCarExplosionZoom, 0.2f);
    }

    private void EnemyCarExplosionZoom()
    {
        StartCoroutine(CarExplosionCoroutine());
    }

    private IEnumerator CarExplosionCoroutine()
    {
        MotorBurnParticleStatus(true);
        StopCarShake();
        GamePanelController.RideTriggerStatusAction?.Invoke(false);
        GamePanelController.EngineRawContainerStatusAction?.Invoke(false);
        dependencyContainerSO.TugController.Move = true;
        dependencyContainerSO.EngineController.StopCheckRedZone();
        yield return new WaitForSeconds(1f);

        explosionParticles.Play();

        foreach (var part in CurrentBody.CarParts)
        {
            Vector3 dir = Vector3.zero;
            if (part.transform.localPosition.x > 0)
            {
                dir = new Vector3(Random.Range(2, 3), Random.Range(2, 3), Random.Range(-2, -3));
            }
            else
            {
                dir = new Vector3(Random.Range(-2, -3), Random.Range(2, 3), Random.Range(-2, -3));
            }
            part.transform.DOLocalMove(part.transform.localPosition + dir, 0.5f);
            part.transform.DORotate(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)), 0.5f).OnComplete(() =>
            {
                part.Mesh.enabled = false;
            });
        }

        dependencyContainerSO.PlayerController.CurrentMotorPower *= 5f;
    }

    private void CarExplosion()
    {
        StartMotor(false);
        EnemyWheelParticle(false);
        StopCarShake();
    }

    public void StartCanvasStatus(bool status)
    {
        startCanvas = status;
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
        shakeContainer.localEulerAngles = Vector3.zero;
        carShakeTween.Kill();
        carShakeTween = null;
    }

    private void SetBody()
    {
        foreach (var cars in carBodies)
        {
            cars.gameObject.SetActive(false);
        }

        List<CarBody> tempBody = carBodies.FindAll(body => body.WorldType != playerStorageSO.ConcretePlayer.WorldType);

        int randIndex = Random.Range(0, tempBody.Count);

        while (randIndex == prevBodyColor)
        {
            randIndex = Random.Range(0, tempBody.Count);
        }

        CurrentBody = tempBody[randIndex];
        tempBody[randIndex].gameObject.SetActive(true);
        CurrentBody.SetPartsParameters();

        prevBodyColor = randIndex;
    }

    private void HideDetails()
    {
        foreach (var detail in carDetails)
        {
            detail.gameObject.SetActive(false);
        }
    }

    private void SetDetails()
    {
        HideDetails();
        RandomDetail(DetailType.Wheel);
        RandomDetail(DetailType.Motor);
        RandomDetail(DetailType.Exhaust);
        RandomDetail(DetailType.BodyKit);
    }

    private void RandomDetail(DetailType detailType)
    {
        if (true)
        {

        }
        var details = carDetails.FindAll(detail => detail.DetailType == detailType);
        int randomDetail = Random.Range(0, details.Count + 1);
        if (randomDetail != details.Count)
        {
            details[randomDetail].gameObject.SetActive(true);
            //if (details[randomDetail].NeedChangePosition)
            //{
            //    switch (prevBodyColor)
            //    {
            //        case 0:
            //            details[randomDetail].ShowDetail(WorldType.Alpha);
            //            break;
            //        case 1:
            //            details[randomDetail].ShowDetail(WorldType.Beta);
            //            break;
            //        case 2:
            //            details[randomDetail].ShowDetail(WorldType.Gamma);
            //            break;
            //        default:
            //            break;
            //    }
            //}
            if (detailType == DetailType.Wheel)
            {
                CurrentWheels = details[randomDetail].Wheels;
                baseWheelsContainer.gameObject.SetActive(false);
            }
            else if (detailType == DetailType.Motor)
            {
                foreach (var engine in baseEngines)
                {
                    if (engine != null)
                    {
                        engine.gameObject.SetActive(false);
                    }
                }
            }
            detailsList.Add(details[randomDetail]);
        }
        else
        {
            if (detailType == DetailType.Wheel)
            {
                CurrentWheels = baseWheels;
                baseWheelsContainer.gameObject.SetActive(true);
            }
            else if (detailType == DetailType.Motor)
            {
                foreach (var engine in baseEngines)
                {
                    if (engine != null)
                    {
                        engine.gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    public void DestroyShake()
    {
        carDestroyShakeTween = shakeContainer.DORotate(shakeContainer.eulerAngles + new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10)), 0.2f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }

    public void EnemyDestroy()
    {
        StartCoroutine(EnemyDestroyCoroutine());
    }

    private IEnumerator EnemyDestroyCoroutine()
    {
        CurrentBody.DestroyPart(true, false);
        yield return new WaitForSeconds(0.1f);
        CurrentBody.DestroyPart(true, false);
        yield return new WaitForSeconds(0.1f);
        CurrentBody.DestroyPart(true, false);
        yield return new WaitForSeconds(0.1f);
        CurrentBody.DestroyPart(true, false);
        yield return new WaitForSeconds(0.1f);
        CurrentBody.DestroyPart(true, false);
        yield return new WaitForSeconds(0.1f);
        CurrentBody.DestroyPart(true, false);
    }
}
