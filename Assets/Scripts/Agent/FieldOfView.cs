using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FieldOfView : MonoBehaviour
{
	public event Action<Transform> OnPlayerFound;
	public Vector3 eyeHeightOffset = new Vector3(0, 1.6f, 0);
	public float minDetectionRadius = 1f;
	public float viewRadius;
	[Range(0,360)]
	public float viewAngle;

	public LayerMask targetMask;
	public LayerMask obstacleMask;

	//[HideInInspector]
	public List<Transform> visibleTargets = new List<Transform>();

	PlayerController player;

	void Start()
	{
		player = FindObjectOfType<PlayerController>();
		StartCoroutine("FindTargetsWithDelay", .1f);
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
					OnPlayerFound?.Invoke(target);
				}
				else if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
				{
					if (!Physics.Raycast(position, dirToTarget, dstToTarget, obstacleMask, QueryTriggerInteraction.Ignore))
					{
						visibleTargets.Add(target);
						OnPlayerFound?.Invoke(target);
					}
				}
			}
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
}
