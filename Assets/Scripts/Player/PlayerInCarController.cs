using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInCarController : MonoBehaviour
{
    [SerializeField] private DependencyContainer dependencyContainerSO = default;
    [SerializeField] private List<Rigidbody> headRigidbody = default;

    private void LateUpdate()
    {
        if (dependencyContainerSO.GameStarted)
        {
            headRigidbody.ForEach((_rigidbody) =>
            {
                if (_rigidbody.IsSleeping())
                {
                    _rigidbody.WakeUp();
                    if (_rigidbody.angularVelocity.x < .1f &&
                        _rigidbody.angularVelocity.y < .1f &&
                        _rigidbody.angularVelocity.z < .1f)
                    {
                        _rigidbody.AddForce(_rigidbody.transform.right * .3f, ForceMode.VelocityChange);
                    }
                }
            });
        }
    }
}
