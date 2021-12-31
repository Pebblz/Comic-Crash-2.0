using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearIsland : MonoBehaviour
{
    List<Rigidbody> rigidbodies = new List<Rigidbody>();
    Vector3 lastPos;
    Transform _transform;
    [SerializeField] Transform OceanTransform;

    [SerializeField]
    float BottemY = 39.5f;

    [SerializeField]
    float TopY = 66.5f;

    [SerializeField]
    Transform Gear;
    //bool AtTop;
    float progress;
    //float LastOceanPosY;
    void Start()
    {

        progress = TopY - BottemY;

        progress /= 350;
        _transform = transform;
        lastPos = _transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        //if(OceanTransform.position.y < LastOceanPosY)
        //{
        //    _transform.Translate(new Vector3(0, -1 * Time.deltaTime * 3f, 0), Space.World);
        //    LastOceanPosY = OceanTransform.position.y;
        //}
        //if (OceanTransform.position.y > LastOceanPosY)
        //{
        //    _transform.Translate(new Vector3(0, 1 * Time.deltaTime * 3f, 0), Space.World);
        //    LastOceanPosY = OceanTransform.position.y;
        //}
        //if (Gear.eulerAngles.y >= 350)
        //{
        //    AtTop = true;
        //}
        //if(Gear.eulerAngles.y <= 0)
        //{

        //}
        progress = .00286f * Gear.eulerAngles.y;
        Mathf.Clamp(progress, 0, 1);
        //if (AtTop)
        //{
            transform.position = Vector3.Lerp(new Vector3(transform.position.x, BottemY, transform.position.z),
               new Vector3(transform.position.x, TopY, transform.position.z), progress);
        //}
        //else
        //{
        //    transform.position = Vector3.Lerp(new Vector3(transform.position.x, TopY, transform.position.z),
        //       new Vector3(transform.position.x, BottemY , transform.position.z), progress);
        //}
        if (rigidbodies.Count > 0)
        {
            for (int i = 0; i < rigidbodies.Count; i++)
            {
                if (rigidbodies[i] != null)
                {
                    Rigidbody rb = rigidbodies[i];
                    Vector3 vel = new Vector3((_transform.position.x - lastPos.x) + ((rb.velocity.x * Time.deltaTime) / 1.5f),
                                              (_transform.position.y - lastPos.y) + ((rb.velocity.y * Time.deltaTime) / 1.5f),
                                              (_transform.position.z - lastPos.z) + ((rb.velocity.z * Time.deltaTime) / 1.5f));
                    rb.transform.Translate(vel, transform);
                }
            }
        }
        lastPos = _transform.position;
        
    }
    #region collision functions
    private void OnCollisionEnter(Collision col)
    {
        Rigidbody rb = col.collider.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Add(rb);
        }
    }
    private void OnCollisionStay(Collision col)
    {
        Rigidbody rb = col.collider.GetComponent<Rigidbody>();
        if (rb != null && !rigidbodies.Contains(rb))
        {
            Add(rb);
        }
    }
    private void OnCollisionExit(Collision col)
    {
        Rigidbody rb = col.collider.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Remove(rb);
        }
    }
    void Add(Rigidbody rb)
    {
        if (!rigidbodies.Contains(rb))
            rigidbodies.Add(rb);
    }
    void Remove(Rigidbody rb)
    {
        if (rigidbodies.Contains(rb))
            rigidbodies.Remove(rb);
    }
    #endregion
}
