using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionInterloper : MonoBehaviour
{
	[SerializeField]
	Rigidbody body = default;

	[SerializeField]
	Transform relativeTo = default;

	[SerializeField]
	Vector3 from = default, to = default;

	public void Interpolate(float t)
	{
		Vector3 p;
		if (relativeTo)
		{
			p = Vector3.LerpUnclamped(
				relativeTo.TransformPoint(from), relativeTo.TransformPoint(to), t
			);
		}
		else
		{
			p = Vector3.LerpUnclamped(from, to, t);
		}
		body.MovePosition(p);
	}
}
