﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingRigidbody : MonoBehaviour
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
    float[] submergence;
    Vector3 gravity;
    [SerializeField]
    Vector3[] buoyancyOffsets = default;
    [SerializeField]
    bool safeFloating = false;
    [SerializeField]
    bool Walkable;
    void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
        submergence = new float[buoyancyOffsets.Length];
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
        float dragFactor = waterDrag * Time.deltaTime / buoyancyOffsets.Length;
        float buoyancyFactor = -buoyancy / buoyancyOffsets.Length;
        for (int i = 0; i < buoyancyOffsets.Length; i++)
        {
            if (submergence[i] > 0f)
            {
                float drag = Mathf.Max(0f, 1f - dragFactor * submergence[i]);
                body.velocity *= drag;
                body.angularVelocity *= drag;
                body.AddForceAtPosition(gravity * (buoyancyFactor * submergence[i]),
                    transform.TransformPoint(buoyancyOffsets[i]), ForceMode.Acceleration);
                submergence[i] = 0f;
            }
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
    private void OnCollisionEnter(Collision collision)
    {
        if (Walkable)
        {
            if (collision.gameObject.tag == "Player" &&
                collision.gameObject.transform.position.y >
                transform.gameObject.transform.position.y)
            {
                body.isKinematic = true;
            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (Walkable)
        {
            if (collision.gameObject.tag == "Player" &&
                collision.gameObject.transform.position.y >
                transform.gameObject.transform.position.y)
            {
                body.isKinematic = true;
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (Walkable)
        {
            if (collision.gameObject.tag == "Player")
            {
                body.isKinematic = false;
            }
        }
    }
    void EvaluateSubmergence()
    {
        Vector3 down = gravity.normalized;
        Vector3 offset = down * -submergenceOffset;
        for (int i = 0; i < buoyancyOffsets.Length; i++)
        {
            Vector3 p = offset + transform.TransformPoint(buoyancyOffsets[i]);
            if (Physics.Raycast(p, down, out RaycastHit hit, submergenceRange + 1f,
                waterMask, QueryTriggerInteraction.Collide))
            {
                submergence[i] = 1f - hit.distance / submergenceRange;
            }
            else if (!safeFloating || Physics.CheckSphere(p, 0.01f, waterMask,
                QueryTriggerInteraction.Collide))
            {
                submergence[i] = 1f;
            }
        }
    }
}