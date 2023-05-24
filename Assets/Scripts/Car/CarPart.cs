using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPart : MonoBehaviour
{
    [SerializeField] private MeshRenderer mesh = default;
    [SerializeField] private Vector3 destroyPosition = default;
    [SerializeField] private Vector3 destroyRotation = default;

    public bool IsDestroyed { get; set; } = false;
    public MeshRenderer Mesh => mesh;
    public Vector3 DestroyPosition => destroyPosition;
    public Vector3 DestroyRotation => destroyRotation;

    private Vector3 startPosition = default;
    private Vector3 startRotation = default;

    private void Awake()
    {
        startPosition = transform.localPosition;
        startRotation = transform.localEulerAngles;
    }

    public void SetStartParameters()
    {
        transform.localPosition = startPosition;
        transform.localEulerAngles = startRotation;
    }

    public void DestroyPart()
    {
        IsDestroyed = true;
    }
}
