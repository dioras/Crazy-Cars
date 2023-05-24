using System;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private float cameraSpeed = 0.3f;
	[SerializeField] private float tweenDuration = 1f;

	[Header("Positions")]
	[SerializeField] private Vector3 basePosition = default;
	[SerializeField] private Vector3 baseRotation = default;
	[SerializeField] private Vector3 menuPosition = default;
	[SerializeField] private Vector3 menuRotation = default;
	[SerializeField] private Vector3 gachaponPosition = default;
	[SerializeField] private Vector3 gachaponRotation = default;
	[SerializeField] private Vector3 playerTrapPosition = default;
	[SerializeField] private Vector3 playerTrapRotation = default;
	[SerializeField] private Vector3 enemyTrapPosition = default;
	[SerializeField] private Vector3 enemyTrapRotation = default;
	[SerializeField] private Vector3 countdownStartPosition = default;
	[SerializeField] private Vector3 countdownStartRotation = default;	
	[SerializeField] private Vector3 countdownEndPosition = default;
	[SerializeField] private Vector3 countdownEndRotation = default;

	[Header("Actions")]
	public static Action<Transform, bool, Action> SetCameraPositionFromTransform = default;
	public static Action<Vector3, Vector3, bool, Action, float> SetCameraPositionFromCoordinate = default;
	public static Action<Transform> SetCameraTarget = default;
	public static Action CameraFinishMoveAction = default;
	public static Action SetBasePositionAction = default;
	public static Action SetMenuPositionAction = default;
	public static Action SetGachaponPositionAction = default;
	public static Action SetPlayerTrapPositionAction = default;
	public static Action SetEnemyTrapPositionAction = default;
	public static Action SetCountdownPositionAction = default;

	private Transform target = default;

	private float ratio = 1.77f;
	private float height = 0f;
	private float width = 0f;

	private Vector3 startPosition = default;
	private Vector3 startRotation = default;

    private void Awake()
    {
		startPosition = transform.position;
		startRotation = transform.eulerAngles;
	}

	private void OnEnable()
	{
		GameManager.PrepereLevelAction += PrepareForLevel;
		SetCameraPositionFromTransform += CameraPositionFromTransform;
		SetCameraPositionFromCoordinate += CameraPositionFromCoordinate;
		SetCameraTarget += SetTarget;
		SetBasePositionAction += SetBasePosition;
		SetGachaponPositionAction += SetGachaponPosition;
		SetPlayerTrapPositionAction += SetPlayerTrapPosition;
		SetEnemyTrapPositionAction += SetEnemyTrapPosition;
		SetCountdownPositionAction += SetCountdownPosition;
		SetMenuPositionAction += SetMenuPosition;
		RecalculateOffsetCameraFromResolution();
	}

	private void OnDisable()
	{
		GameManager.PrepereLevelAction -= PrepareForLevel;
		SetCameraPositionFromTransform -= CameraPositionFromTransform;
		SetCameraPositionFromCoordinate -= CameraPositionFromCoordinate;
		SetCameraTarget -= SetTarget;
		SetBasePositionAction -= SetBasePosition;
		SetGachaponPositionAction -= SetGachaponPosition;
		SetPlayerTrapPositionAction -= SetPlayerTrapPosition;
		SetEnemyTrapPositionAction -= SetEnemyTrapPosition;
		SetCountdownPositionAction -= SetCountdownPosition;
		SetMenuPositionAction -= SetMenuPosition;
		transform.DOKill();
	}

	private void OnDestroy()
	{
		transform.DOKill();
	}

	private void FixedUpdate()
	{
		if (target != null)
		{
			transform.position = Vector3.Lerp(transform.position, target.position, cameraSpeed);
			transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, cameraSpeed);

			if (transform.position == target.position && transform.rotation == target.rotation)
			{
				target = null;
			}
		}
	}

	private void PrepareForLevel()
	{
		transform.position = startPosition;
		transform.eulerAngles = startRotation;
	}

	private void RecalculateOffsetCameraFromResolution()
	{
		height = Screen.height;
		width = Screen.width;

		float currentRatio = height / width;
		float newRatio = currentRatio / ratio;

		Camera.main.fieldOfView *= newRatio;
	}

	private void SetTarget(Transform _target)
	{
		transform.DOKill();
		target = _target;
	}

	private void CameraPositionFromTransform(Transform _transform, bool _isTween = false, Action afterTweenAction = null)
	{
		CameraPositionFromCoordinate(_transform.position, _transform.eulerAngles, _isTween, afterTweenAction, tweenDuration);
	}

	private void CameraPositionFromCoordinate(Vector3 _position, Vector3 _eulerAngles, bool _isTween = false, Action afterTweenAction = null, float _tweenDuration = 0)
	{
		target = null;
		transform.DOKill();

		if (!_isTween)
		{
			transform.position = _position;
			transform.eulerAngles = _eulerAngles;
		}
		else
		{
			transform.DOMove(_position, _tweenDuration).OnComplete(() =>
			{
				if (afterTweenAction != null)
				{
					afterTweenAction?.Invoke();
				}
			});
			transform.DORotate(_eulerAngles, _tweenDuration);
		}
	}

	private void SetBasePosition()
    {
		CameraPositionFromCoordinate(basePosition, baseRotation, true, null, 0.5f);
	}

	private void SetMenuPosition()
	{
		CameraPositionFromCoordinate(menuPosition, menuRotation, false, null);
	}

	private void SetGachaponPosition()
	{
		CameraPositionFromCoordinate(gachaponPosition, gachaponRotation, false, null);
	}

	private void SetPlayerTrapPosition()
	{
		CameraPositionFromCoordinate(playerTrapPosition, playerTrapRotation, true, null, 0.2f);
	}

	private void SetEnemyTrapPosition()
	{
		CameraPositionFromCoordinate(enemyTrapPosition, enemyTrapRotation, true, null, 0.2f);
	}

	private void SetCountdownPosition()
    {
		CameraPositionFromCoordinate(countdownStartPosition, countdownStartRotation, true, CountdownEndPosition, 0.3f);
	}

	private void CountdownEndPosition()
	{
		CameraPositionFromCoordinate(countdownEndPosition, countdownEndRotation, true, AfterCountdownBasePosition, 2.2f);
	}

	private void AfterCountdownBasePosition()
	{
		CameraPositionFromCoordinate(basePosition, baseRotation, true, null, 0.3f);
	}
}
