using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EngineController : MonoBehaviour
{
	[Header("Base")]
	[SerializeField] private DependencyContainer dependencyContainerSO = default;
	[SerializeField] private LevelStorage levelStorageSO = default;
	[SerializeField] private GameStorage gameStorageSO = default;
	[SerializeField] private PlayerStorage playerStorageSO = default;

	[SerializeField] private Transform engine = default;
	[SerializeField] private Transform redzoneEngine = default;
	[SerializeField] private float redZoneExplosionScale = default;

	[SerializeField] private List<ParticleSystem> redZoneParticles = default;

	[SerializeField] private TextMeshPro scaleText = default;

	[Header("Engine Slider")]
	[SerializeField] private Transform engineArrow = default;
	[SerializeField] private float baseRedZoneAngle = default;
	[SerializeField] private float redZoneTime = default;

	[Header("Actions")]
	public static Action<bool> CheckRedZoneAction = default;
	public static Action CarExplosionAction = default;

	public static Action EngineSpeedUpAction = default;
	public static Action EngineSpeedDownAction = default;

	private Coroutine redZoneCoroutine = default;
	private Coroutine destroyDetailCoroutine = default;
	private Coroutine explosionCoroutine = default;
	private Tween scaleTween = default;
	private Tween scaleTextTween = default;

	public bool CanCooling { get; set; }

	private void Awake()
    {
		dependencyContainerSO.EngineController = this;
	}

    private void OnEnable()
	{
		GameManager.PrepereLevelAction += PrepareForLevel;
		CheckRedZoneAction += CheckRedZone;
		EngineSpeedUpAction += EngineSpeedUp;
		EngineSpeedDownAction += EngineSpeedDown;
	}

	private void OnDisable()
	{
		GameManager.PrepereLevelAction -= PrepareForLevel;
		CheckRedZoneAction -= CheckRedZone;
		EngineSpeedUpAction -= EngineSpeedUp;
		EngineSpeedDownAction -= EngineSpeedDown;
	}


	private void PrepareForLevel()
	{
		CanCooling = true;
		engineArrow.localRotation = new Quaternion(engineArrow.localRotation.x, engineArrow.localRotation.y, -0.7f, engineArrow.localRotation.w);
		redzoneEngine.localScale = Vector3.one;
		if (redZoneCoroutine != null)
		{
			StopCoroutine(redZoneCoroutine);
		}
		redZoneCoroutine = null;
		if (destroyDetailCoroutine != null)
		{
			StopCoroutine(destroyDetailCoroutine);
		}
		destroyDetailCoroutine = null;

		if (explosionCoroutine != null)
		{
			StopCoroutine(explosionCoroutine);
		}
		explosionCoroutine = null;
		
		RedZoneParticles(false);
		scaleTween.Kill();
		scaleTween = null;
		scaleText.gameObject.SetActive(false);
		scaleTextTween.Kill();
		scaleTextTween = null;
		scaleText.transform.localScale = Vector3.one;
	}

    public void EngineSpeedUp()
	{
        if (engineArrow.localRotation.z <= 0.7f)
        {
            var temp = engineArrow.localRotation;
			temp.z += dependencyContainerSO.PlayerController.CurrentMotorPower * 0.0008f;
			engineArrow.localRotation = temp;
        }
    }

	public void EngineSpeedDown()
	{
		if (engineArrow.localRotation.z > -0.7f)
		{
			var temp = engineArrow.localRotation;
			temp.z -= dependencyContainerSO.PlayerController.CurrentMotorPower * 0.001f * 2f;
			engineArrow.localRotation = temp;
		}
	}

	private void RedZoneParticles(bool play)
    {
        foreach (var particle in redZoneParticles)
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


	public void CheckRedZone(bool start)
	{
        if (start)
        {
            if (engineArrow.localRotation.z >= baseRedZoneAngle)
            {
				dependencyContainerSO.PlayerController.CurrentMotorPower = dependencyContainerSO.PlayerController.RedZoneMotorPower;
				//if (playerStorageSO.ConcretePlayer.InstalledDetails.Count > 0 || (playerStorageSO.ConcretePlayer.InstalledDetails.Count == 0 && !dependencyContainerSO.PlayerController.LastDetail))
				//{
				if (redZoneCoroutine == null)
                    {
                        redZoneCoroutine = StartCoroutine(RedZoneCoroutine());
                    }
                //}
                //else if (playerStorageSO.ConcretePlayer.InstalledDetails.Count == 0 && dependencyContainerSO.PlayerController.LastDetail)
                //{
                //    StopCheckRedZone();
                //    CarExplosionAction?.Invoke();
                //}
            }
            else
            {
				dependencyContainerSO.PlayerController.CurrentMotorPower = dependencyContainerSO.PlayerController.BaseMotorPower;
			}
        }
        else
        {
			dependencyContainerSO.GameStarted = false;
            StopCheckRedZone();
		}
    }

	public void StopCheckRedZone()
	{
		if (redZoneCoroutine != null)
		{
			StopCoroutine(redZoneCoroutine);
		}
		redZoneCoroutine = null;
		if (destroyDetailCoroutine != null)
		{
			StopCoroutine(destroyDetailCoroutine);
		}
		destroyDetailCoroutine = null;

		RedZoneParticles(false);
		scaleTween.Kill();
		scaleTween = null;

		scaleText.gameObject.SetActive(false);
		scaleTextTween.Kill();
		scaleTextTween = null;
		scaleText.transform.localScale = Vector3.one;
		dependencyContainerSO.PlayerController.DownCar();
	}

	private IEnumerator RedZoneCoroutine()
	{
		//dependencyContainerSO.PlayerController.UpCar();
		yield return null;
		RedZoneParticles(true);

		engine.transform.localScale = Vector3.one;
		//warningContainer.transform.localScale = baseWarningContainerScale;
		//warningContainer.gameObject.SetActive(true);
		//scaleTween = warningContainer.DOScale(baseWarningContainerScale * 1.2f, 0.2f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);

		scaleTween = engine.DOScale(Vector3.one * 1.05f, 0.2f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
		scaleText.gameObject.SetActive(true);
		scaleTextTween = scaleText.transform.DOScale(Vector3.one * 1.2f, 0.2f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);

		//GamePanelController.RedZoneSliderStatusAction?.Invoke(true);

		destroyDetailCoroutine = StartCoroutine(DestroyDetailCoroutine());

		while (engineArrow.localRotation.z >= baseRedZoneAngle)
		{
			VibrationController.Vibrate(30);
			dependencyContainerSO.TugController.RedZone = true;
            if (!dependencyContainerSO.PlayerController.CarIsUp)
            {
				dependencyContainerSO.PlayerController.UpCar();

            }
			
			yield return new WaitForSeconds(0.01f);
		}

		RedZoneParticles(false);
		dependencyContainerSO.PlayerController.DownCar();
		StopCoroutine(destroyDetailCoroutine);
		destroyDetailCoroutine = null;

		//warningContainer.gameObject.SetActive(false);
		engine.transform.localScale = Vector3.one;
		scaleTween.Kill();
		scaleTween = null;

		scaleText.gameObject.SetActive(false);
		scaleTextTween.Kill();
		scaleTextTween = null;
		scaleText.transform.localScale = Vector3.one;
		dependencyContainerSO.TugController.RedZone = false;
		StopCoroutine(redZoneCoroutine);
		redZoneCoroutine = null;
	}

	private IEnumerator DestroyDetailCoroutine()
	{
		yield return new WaitForSeconds(redZoneTime);
		dependencyContainerSO.PlayerController.DestroyDetail();
		//dependencyContainerSO.PlayerController.LastDetail = playerStorageSO.ConcretePlayer.InstalledDetails.Count == 0;

		if (engineArrow.localRotation.z >= baseRedZoneAngle)
		{
			StopCoroutine(destroyDetailCoroutine);
			destroyDetailCoroutine = null;

			destroyDetailCoroutine = StartCoroutine(DestroyDetailCoroutine());
		}
	}

	public void RedZoneSliderStatus(bool start)
	{
		if (start && dependencyContainerSO.TugController.RedZone)
		{
			if (redzoneEngine.localScale.x < redZoneExplosionScale)
			{
				redzoneEngine.localScale += new Vector3(2f, 0, 0);
			}
			else
			{
                if (explosionCoroutine == null)
                {
                    explosionCoroutine = StartCoroutine(ExplosionCoroutine());
                }
            }
		}
		else
		{
			if (redzoneEngine.localScale.x > 0 && !dependencyContainerSO.TugController.RedZone)
			{
				if (explosionCoroutine != null)
				{
					StopCoroutine(explosionCoroutine);
					explosionCoroutine = null;
				}
				redzoneEngine.localScale -= new Vector3(1f, 0, 0);
			}
		}
	}

	private IEnumerator ExplosionCoroutine()
    {
		float timer = 0;
        while (timer < 0.5f)
        {
			timer += 0.01f;
			yield return new WaitForSeconds(0.01f);
		}

		dependencyContainerSO.PlayerController.CarExplosion();
	}
}
