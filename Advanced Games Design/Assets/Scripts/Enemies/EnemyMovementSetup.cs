﻿using UnityEngine;

public class EnemyMovementSetup
{
    public float speedDampTime = 0.1f;
    public float angularSpeedDampTime = 0.7f;
    public float angleResponseTime = 0.6f;

    private Animator anim;

    public EnemyMovementSetup(Animator animator)
    {
        anim = animator;
    }

    public void Setup(float speed, float angle)
    {
        float angularSpeed = angle / angleResponseTime;

        anim.SetFloat(Animator.StringToHash("Speed"), speed, speedDampTime, Time.deltaTime);
        anim.SetFloat(Animator.StringToHash("AngularSpeed"), angularSpeed, angularSpeedDampTime, Time.deltaTime);
    }
	
}
