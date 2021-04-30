using UnityEngine;
using System.Collections.Generic;
[RequireComponent(typeof(Rigidbody))]
public class CustomGravityRigidbody : MonoBehaviour
{
	float floatDelay;
	Rigidbody body;
	[SerializeField]
	bool floatToSleep = false;
	[SerializeField]
	float submergenceOffset = 0.5f;
	[SerializeField, Min(0.1f)]
	float submergenceRange = 1f;
	[SerializeField, Min(0f)]
	float buoyancy = 1f;
	[SerializeField, Range(0f, 10f)]
	float waterDrag = 1f;
	[SerializeField]
	LayerMask waterMask = 0;
	float submergence;
	Vector3 gravity;
	[SerializeField]
	Vector3 buoyancyOffset = Vector3.zero;
	void Awake()
	{
		body = GetComponent<Rigidbody>();
		body.useGravity = false;
	}
	void FixedUpdate()
	{
		if (floatToSleep)
		{
			floatDelay = 0f;
			return;
		}
		if (body.velocity.sqrMagnitude < 0.0001f)
		{
			floatDelay += Time.deltaTime;
			if (floatDelay >= 1f)
			{
				return;
			}
		}
		else
		{
			floatDelay = 0f;
		}
		gravity = CustomGravity.GetGravity(body.position);
		if (submergence > 0f)
		{
			float drag = Mathf.Max(0f, 1f - waterDrag * submergence * Time.deltaTime);
			body.velocity *= drag;
			body.angularVelocity *= drag;
			body.AddForceAtPosition(gravity * -(buoyancy * submergence), 
				transform.TransformPoint(buoyancyOffset), ForceMode.Acceleration);
			submergence = 0f;
		}
		body.AddForce(gravity, ForceMode.Acceleration);
	}
	void OnTriggerEnter(Collider other)
	{
		if ((waterMask & (1 << other.gameObject.layer)) != 0)
		{
			EvaluateSubmergence();
		}
	}

	void OnTriggerStay(Collider other)
	{
		if (!body.IsSleeping() && (waterMask & (1 << other.gameObject.layer)) != 0)
		{
			EvaluateSubmergence();
		}
	}

	void EvaluateSubmergence()
	{
		Vector3 upAxis = -gravity.normalized;
		if (Physics.Raycast(body.position + upAxis * submergenceOffset,
			-upAxis, out RaycastHit hit, submergenceRange + 1f, waterMask, QueryTriggerInteraction.Collide
		))
		{
			submergence = 1f - hit.distance / submergenceRange;
		}
		else
		{
			submergence = 1f;
		}
	}
}
public static class CustomGravity
{
	static List<GravitySource> sources = new List<GravitySource>();
	public static Vector3 GetUpAxis(Vector3 position)
	{
		Vector3 g = Vector3.zero;
		for (int i = 0; i < sources.Count; i++)
		{
			g += sources[i].GetGravity(position);
		}
		return -g.normalized;
	}
	public static Vector3 GetGravity(Vector3 position)
	{
		Vector3 g = Vector3.zero;
		for (int i = 0; i < sources.Count; i++)
		{
			g += sources[i].GetGravity(position);
		}
		return g;
	}
	public static Vector3 GetGravity(Vector3 position, out Vector3 upAxis)
	{
		Vector3 g = Vector3.zero;
		for (int i = 0; i < sources.Count; i++)
		{
			g += sources[i].GetGravity(position);
		}
		upAxis = -g.normalized;
		return g;
	}
	public static void Register(GravitySource source)
	{
		Debug.Assert(
			!sources.Contains(source),
			"Duplicate registration of gravity source!", source
		);
		sources.Add(source);
	}
	public static void Unregister(GravitySource source)
	{
		Debug.Assert(
			sources.Contains(source),
			"Unregistration of unknown gravity source!", source
		);
		sources.Remove(source);
	}
}
