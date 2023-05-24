using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationVibration : MonoBehaviour
{
    public void AnimationVibrationAction()
    {
        VibrationController.Vibrate(30);
        ShakeCamera.ShakeAction?.Invoke(0.15f, 0.1f);
    }
}
