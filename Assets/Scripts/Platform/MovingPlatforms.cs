using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParentPlayer))]
public class MovingPlatforms : MonoBehaviour
{
    [Header("Direction")]

    [Tooltip("If set true it'll go up and down, if false it'll move side to side")]
    [SerializeField]
    bool upAndDown;

    [Tooltip("If up and down if false, this'll will be for if the platform will move straight and back or left and right")]
    [SerializeField]
    bool LeftAndRight;

    [Tooltip("If you want it to go up and down and what ever you put for left and right")]
    [SerializeField]
    bool Both;


    [Header("Distance")]
    [Tooltip("The amount the platform will move up and down")]
    [Range(1f, 10f)]
    [SerializeField]
    float DistanceToMoveY;

    [Tooltip("The amount the platform will move left nad right or striaght and back")]
    [Range(1f, 10f)]
    [SerializeField]
    float DistanceToMoveXZ;

    [Space(10)]
    [Tooltip("The speed at which the platform moves")]
    [Range(1f, 10f)]
    [SerializeField]
    float speed = 2;

    [Tooltip("If you want it to activate when step on")]
    [SerializeField]
    bool StepOnToActivate;

    [Tooltip("If this is true the moving platforms active and moving")]
    public bool active = true;

    private float EndPointY, EndPointX, EndPointZ;

    private Vector3 StartPoint;

    private bool GoBackY, GoBackX, GoBackZ, GoBackToStart;

    private bool isSteppedOn;

    #region MonoBehaviour functions
    void Start()
    {
        StartPoint = transform.position;
        if (upAndDown)
            EndPointY = StartPoint.y + DistanceToMoveY;

        if (!LeftAndRight)
            EndPointZ = StartPoint.z + DistanceToMoveXZ;

        if (LeftAndRight)
            EndPointX = StartPoint.x + DistanceToMoveXZ;
    }

    void Update()
    {
        if (active)
        {
            if (!StepOnToActivate && !GoBackToStart)
            {
                Move();
            }
            if (StepOnToActivate && !GoBackToStart)
            {
                if (isSteppedOn)
                {
                    Move();
                }
                else
                {
                    resetPosition();
                }
            }
            if (GoBackToStart)
            {
                resetPosition();
            }
        }
    }
    #endregion
    #region Movement functions
    private void Move()
    {
        if (upAndDown)
        {
            UpAndDown();
            if (Both)
            {
                if (!LeftAndRight)
                {
                    FrontToBack();
                }
                else
                {
                    LeftToRight();
                }
            }

        }
        else
        {
            if (!LeftAndRight)
            {
                FrontToBack();
            }
            else
            {
                LeftToRight();
            }
        }
    }
    void LeftToRight()
    {
        if (GoBackX)
        {
            this.gameObject.transform.Translate(new Vector3(-1 * Time.deltaTime * speed, 0, 0), Space.World);
            if (this.gameObject.transform.position.x <= StartPoint.x)
            {
                GoBackX = false;
            }
        }
        else
        {
            this.gameObject.transform.Translate(new Vector3(1 * Time.deltaTime * speed, 0, 0), Space.World);
            if (this.gameObject.transform.position.x >= EndPointX)
            {
                GoBackX = true;
            }
        }
    }
    void FrontToBack()
    {
        if (GoBackZ)
        {
            this.gameObject.transform.Translate(new Vector3(0, 0, -1 * Time.deltaTime * speed), Space.World);
            if (this.gameObject.transform.position.z <= StartPoint.z)
            {
                GoBackZ = false;
            }
        }
        else
        {
            this.gameObject.transform.Translate(new Vector3(0, 0, 1 * Time.deltaTime * speed), Space.World);
            if (this.gameObject.transform.position.z >= EndPointZ)
            {
                GoBackZ = true;
            }
        }
    }
    void UpAndDown()
    {
        if (GoBackY)
        {
            this.gameObject.transform.Translate(new Vector3(0, -1 * Time.deltaTime * speed, 0), Space.World);
            if (this.gameObject.transform.position.y <= StartPoint.y)
            {
                GoBackY = false;
            }
        }
        else
        {

            this.gameObject.transform.Translate(new Vector3(0, 1 * Time.deltaTime * speed, 0), Space.World);
            if (this.gameObject.transform.position.y >= EndPointY)
            {
                GoBackY = true;
            }
        }
    }
    void resetPosition()
    {
        transform.position = Vector3.MoveTowards(transform.position, StartPoint, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, StartPoint) < .1)
        {
            GoBackToStart = false;
        }
    }
    #endregion
    #region collision functions
    private void OnCollisionEnter(Collision col)
    {

        if (col.gameObject.tag == "Player" && StepOnToActivate)
        {
            isSteppedOn = true;
        }

    }
    private void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "Player" && StepOnToActivate)
        {
            isSteppedOn = false;
        }
    }
    #endregion
}
