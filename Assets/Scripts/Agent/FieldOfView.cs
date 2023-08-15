using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FieldOfView : MonoBehaviour
{
	public event Action<Transform> OnPlayerFound;

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
		Collider[] targetsInViewRadius = Physics.OverlapSphere (transform.position, viewRadius, targetMask);

		for (int i = 0; i < targetsInViewRadius.Length; i++)
		{
			Transform target = targetsInViewRadius [i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;
			float dstToTarget = Vector3.Distance(transform.position, target.position);
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
					if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask, QueryTriggerInteraction.Ignore))
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
