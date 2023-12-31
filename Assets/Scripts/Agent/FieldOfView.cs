﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FieldOfView : MonoBehaviour
{
	public Vector3 eyeHeightOffset = new Vector3(0, 1.6f, 0);
	public float minDetectionRadius = 1f;
	public float viewRadius;
	[Range(0,360)]
	public float viewAngle;
	public LayerMask targetMask;
	public LayerMask obstacleMask;
	public float knownTargetForgetTime = 5f;
	public float allyAlertRadius = 10f;

	//[HideInInspector]
	public List<Transform> visibleTargets = new List<Transform>();
	//[HideInInspector]
	public List<Transform> knownTargets = new List<Transform>();

	AgentHealth health;
	PlayerController player;
	float timer;
	public bool Alert { get; private set; } = false;

	void Start()
	{
		player = FindObjectOfType<PlayerController>();
		health = GetComponent<AgentHealth>();
        health.OnDamageTaken += Health_OnDamageTaken;
		StartCoroutine("FindTargetsWithDelay", .1f);
	}

    private void Health_OnDamageTaken(DamageSource damageSource, float damage)
    {
        if (damageSource == DamageSource.Player)
        {
			AddKnownTarget(player.transform);
        }
    }

    IEnumerator FindTargetsWithDelay(float delay)
	{
		while (true)
		{
			yield return new WaitForSeconds (delay);
			FindVisibleTargets ();
		}
	}

    void FindVisibleTargets()
	{
		visibleTargets.Clear();
		Collider[] targetsInViewRadius = Physics.OverlapSphere (transform.position + eyeHeightOffset, viewRadius, targetMask);
		Vector3 position = transform.position + eyeHeightOffset;
		for (int i = 0; i < targetsInViewRadius.Length; i++)
		{
			Transform target = targetsInViewRadius [i].transform;
			Vector3 targetPosition = target.position + eyeHeightOffset;
			Vector3 dirToTarget = (targetPosition - position).normalized;
			float dstToTarget = Vector3.Distance(position, targetPosition);
			// detect target if within radius
			if (target == player.transform)
            {
				if (dstToTarget <= minDetectionRadius)
				{
					visibleTargets.Add(target);
				}
				else if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
				{
					if (!Physics.Raycast(position, dirToTarget, dstToTarget, obstacleMask, QueryTriggerInteraction.Ignore))
					{
						visibleTargets.Add(target);
					}
				}
			}
		}
        for (int i = 0; i < knownTargets.Count; i++)
        {
			if (!visibleTargets.Contains(knownTargets[i]))
            {
				Vector3 direction = ((knownTargets[i].position + eyeHeightOffset) - (transform.position + eyeHeightOffset)).normalized;
				if (!Physics.Raycast(transform.position + eyeHeightOffset, direction, Vector3.Distance(knownTargets[i].position, transform.position), obstacleMask, QueryTriggerInteraction.Ignore))
                {
					visibleTargets.Add(knownTargets[i]);
                }
            }
        }
		if (visibleTargets.Count > 0)
        {
			AddKnownTargets(visibleTargets);
			if (!Alert)
            {
				Alert = true;
				AlertNearbyAllies();
			}
        }
		if (visibleTargets.Count == 0)
        {
			Alert = false;
        }
	}

	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
	{
		if (!angleIsGlobal)
		{
			angleInDegrees += transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),0,Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}

    private void Update()
    {
        if (timer > 0)
        {
			timer -= Time.deltaTime;
			if (timer <= 0)
            {
				knownTargets.Clear();
            }
        }
    }

	public void AlertNearbyAllies()
    {
		Collider[] alliesInRadius = Physics.OverlapSphere(transform.position + eyeHeightOffset, allyAlertRadius, targetMask);
		List<FieldOfView> alreadyAlerted = new List<FieldOfView>();
		for (int i = 0; i < alliesInRadius.Length; i++)
        {
			FieldOfView allyFOV = alliesInRadius[i].GetComponentInParent<FieldOfView>();
			if (allyFOV != null)
            {
				if (!allyFOV.Alert && !alreadyAlerted.Contains(allyFOV))
				{
					allyFOV.AddKnownTargets(visibleTargets);
					alreadyAlerted.Add(allyFOV);
				}
			}
        }
	}

	public void AddKnownTargets(List<Transform> targets)
    {
        for (int i = 0; i < targets.Count; i++)
        {
			if (!knownTargets.Contains(targets[i]))
            {
				knownTargets.Add(targets[i]);
				timer = knownTargetForgetTime;
			}
		}
    }

	public void AddKnownTarget(Transform target)
    {
		if (!knownTargets.Contains(target))
        {
			knownTargets.Add(target);
			timer = knownTargetForgetTime;
		}
	}
}
