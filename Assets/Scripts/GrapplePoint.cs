using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GrapplePoint : MonoBehaviour
{
    [Tooltip("The range that PeaShooter can hit the grapple point from")]
    [SerializeField]
    [Range(1f, 20f)]
    float Range;


    PeaShooter PeaShooter;

    [SerializeField]
    Transform gunTip;

    //[SerializeField]
    //GameObject Q;

    [SerializeField]
    [Range(.1f, 1f)]
    float MingrappleDist;


    [SerializeField]
    [Range(.5f, 3f)]
    float MaxgrappleDist;

    [SerializeField]
    [Range(1f, 10f)]
    float JointSpring;
    [SerializeField]
    [Range(5f, 10f)]
    float JointDamper;
    [SerializeField]
    [Range(1f, 30f)]
    float JointMassScale;

    private LineRenderer LR;
    private bool InGrapple;
    private float GrappleTimer;
    private SpringJoint joint;


    private void Awake()
    {
        LR = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PeaShooter == null)
        {
            PeaShooter = FindObjectOfType<PeaShooter>();
        }
        if (PeaShooter != null)
        {
            if (Vector3.Distance(this.gameObject.transform.position, PeaShooter.gameObject.transform.position) < Range)
            {
                //Q.SetActive(true);
                if (Input.GetKeyDown(KeyCode.Q) && GrappleTimer < 0 && !InGrapple)
                {
                    InGrapple = true;
                    StartGrapple();
                    GrappleTimer = .5f;
                }
            }
            else
            {
                //Q.SetActive(false);
            }
        }
        if (InGrapple)
        {
            DrawRope();
            if (Input.GetKeyDown(KeyCode.Q) && GrappleTimer < 0 || 
                Input.GetKeyDown(KeyCode.Space) && GrappleTimer < 0)
            {
                StopGrapple();
                InGrapple = false;
            }
        }
        GrappleTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Call Whenever you need to start the grapple
    /// </summary>
    void StartGrapple()
    {
        this.PeaShooter.gameObject.GetComponent<PlayerMovement>().InGrapple = true;
        joint = PeaShooter.gameObject.AddComponent<SpringJoint>();

        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = this.transform.position;

        float Distance = Vector3.Distance(gunTip.position, transform.position);

        //ths distance grapple will try to keep you from the grapple point
        joint.maxDistance = Distance * MaxgrappleDist;
        joint.minDistance = Distance * MingrappleDist;


        joint.spring = JointSpring;
        joint.damper = JointDamper;
        joint.massScale = JointMassScale;


        LR.positionCount = 2;
    }
    /// <summary>
    /// Call Whenever you need to Stop the grapple
    /// </summary>
    void StopGrapple()
    {
        this.PeaShooter.gameObject.GetComponent<PlayerMovement>().InGrapple = false;
        Destroy(joint);
        LR.positionCount = 0;
    }
    void DrawRope()
    {
        if (!joint)
            return;


        LR.SetPosition(0, gunTip.position);
        LR.SetPosition(1, transform.position);
    }
}
