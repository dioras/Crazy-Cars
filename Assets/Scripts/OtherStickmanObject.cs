using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherStickmanObject : MonoBehaviour
{
	[SerializeField] private Animator animator = default;
	[SerializeField] private AnimationOtherStickman animation = default;

    private void Awake()
    {
        PrepereForLevel();
    }

    private void OnEnable()
    {
        PrepereForLevel();
        //GameManager.PrepereLevelAction += PrepereForLevel;
    }

    private void OnDisable()
    {
        //GameManager.PrepereLevelAction -= PrepereForLevel;
    }

    private void PrepereForLevel()
	{
		animator.SetInteger("Animation", (int)animation);
	}
}
