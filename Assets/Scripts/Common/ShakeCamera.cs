using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShakeCamera : MonoBehaviour
{
    public static Action<float, float> ShakeAction = default;

    private void OnEnable()
    {
        ShakeAction += Shake;
    }

    private void OnDisable()
    {
        ShakeAction -= Shake;
    }

    private void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1, 1) * magnitude;
            float y = Random.Range(-1, 1) * magnitude;

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
