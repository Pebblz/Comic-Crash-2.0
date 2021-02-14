using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    [SerializeField]
    Transform CameraTransform;
    [SerializeField]
    float collisionOffset = .2f;
    Vector3 defaultPos;
    Vector3 directionNormalized;
    float defaultDistance;
    Transform Parent;
    // Start is called before the first frame update
    void Start()
    {
        defaultPos = transform.localPosition;
        directionNormalized = defaultPos.normalized;
        defaultDistance = Vector3.Distance(defaultPos, Vector3.zero);
        Parent = transform.parent;
    }


    void FixedUpdate()
    {
        Vector3 CurrentPos = defaultPos;
        RaycastHit hit;
        Vector3 dirTemp = Parent.TransformPoint(defaultPos) - CameraTransform.position;
        if(Physics.SphereCast(CameraTransform.position, collisionOffset, dirTemp, out hit, defaultDistance))
        {
            CurrentPos = (directionNormalized * (hit.distance - collisionOffset));
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, CurrentPos, Time.deltaTime * 15f);
    }
}
