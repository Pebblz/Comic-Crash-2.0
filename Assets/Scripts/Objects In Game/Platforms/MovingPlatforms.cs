﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(ParentPlayer))]
public class MovingPlatforms : MonoBehaviour
{
    [Header("Direction")]

    [SerializeField, Tooltip("If set true it'll go up and down, if false it'll move side to side. Also don't forget to add the parent player script only for up and down")]
    bool upAndDown;

    [SerializeField, Tooltip("If up and down if false, this'll will be for if the platform will move straight and back or left and right")]
    bool LeftAndRight;

    [SerializeField, Tooltip("If up and down if false, this'll will be for if the platform will move straight and back or left and right")]
    bool ForwardAndBack;

    [SerializeField, Tooltip("If you want it to go up and down and what ever you put for left and right")]
    bool Multiple;


    [SerializeField, Tooltip("The amount of time the platforms stops for when the platform hits the start of end point")]
    float MaxStopTimer;

    [Header("Distance")]

    [SerializeField, Range(-40f, 40f), Tooltip("The amount the platform will move up and down")]
    float DistanceToMoveY;

    [SerializeField, Range(-40f, 40f), Tooltip("The amount the platform will move left nad right or striaght and back")]
    float DistanceToMoveX;

    [SerializeField, Range(-40f, 40f), Tooltip("The amount the platform will move left nad right or striaght and back")]
    float DistanceToMoveZ;

    [Space(10)]

    [SerializeField, Range(1f, 10f), Tooltip("The speed at which the platform moves")]
    float speed = 2;

    [SerializeField, Tooltip("If you want it to activate when step on")]
    bool StepOnToActivate;

    [Tooltip("If this is true the moving platforms active and moving")]
    public bool active = true;

    private float EndPointY, EndPointX, EndPointZ;

    private Vector3 StartPoint;

    private bool GoBackY, GoBackX, GoBackZ, GoBackToStart;

    private bool isSteppedOn;

    Vector3 lastPosition, lastMove;
    float stopTimer;
    bool once;
    #region MonoBehaviour functions
    void Start()
    {
        StartPoint = transform.position;
        if (upAndDown)
            EndPointY = StartPoint.y + DistanceToMoveY;

        if (ForwardAndBack)
            EndPointZ = StartPoint.z + DistanceToMoveZ;

        if (LeftAndRight)
            EndPointX = StartPoint.x + DistanceToMoveX;
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
            if (Multiple)
            {
                if (ForwardAndBack)
                {
                    FrontToBack();
                }
                if (LeftAndRight)
                {
                    LeftToRight();
                }
            }

        }
        else
        {
            if (ForwardAndBack)
            {
                FrontToBack();
            }
            if(LeftAndRight)
            {
                LeftToRight();
            }
        }
        stopTimer -= Time.deltaTime;
    }
    void LeftToRight()
    {
        if (EndPointX > StartPoint.x)
        {
            if (GoBackX)
            {

                if (this.gameObject.transform.position.x <= StartPoint.x)
                {
                    GoBackX = false;
                    if (!once && stopTimer <= 0)
                    {
                        once = true;
                        stopTimer = MaxStopTimer;
                    }
                }
                else
                {
                    if(stopTimer <= 0)
                        this.gameObject.transform.Translate(new Vector3(-1 * Time.deltaTime * speed, 0, 0), Space.World);
                }
            }
            else
            {

                if (this.gameObject.transform.position.x >= EndPointX)
                {
                    GoBackX = true;
                    if (!once && stopTimer <= 0)
                    {
                        once = true;
                        stopTimer = MaxStopTimer;
                    }
                }
                else
                {
                    if (stopTimer <= 0)
                        this.gameObject.transform.Translate(new Vector3(1 * Time.deltaTime * speed, 0, 0), Space.World);
                }
            }
        }
        else
        {
            if (GoBackX)
            {

                if (this.gameObject.transform.position.x >= StartPoint.x)
                {
                    GoBackX = false;
                    if (!once && stopTimer <= 0)
                    {
                        once = true;
                        stopTimer = MaxStopTimer;
                    }
                }
                else
                {
                    if (stopTimer <= 0)
                        this.gameObject.transform.Translate(new Vector3(1 * Time.deltaTime * speed, 0, 0), Space.World);
                }
            }
            else
            {

                if (this.gameObject.transform.position.x <= EndPointX)
                {
                    GoBackX = true;
                    if (!once && stopTimer <= 0)
                    {
                        once = true;
                        stopTimer = MaxStopTimer;
                    }
                }
                else
                {
                    if (stopTimer <= 0)
                        this.gameObject.transform.Translate(new Vector3(-1 * Time.deltaTime * speed, 0, 0), Space.World);
                }
            }
        }
    }
    void FrontToBack()
    {
        if (EndPointZ > StartPoint.z)
        {
            if (GoBackZ)
            {

                if (this.gameObject.transform.position.z <= StartPoint.z)
                {
                    GoBackZ = false;
                    if (!once && stopTimer <= 0)
                    {
                        once = true;
                        stopTimer = MaxStopTimer;
                    }
                }
                else
                {
                    if (stopTimer <= 0)
                        this.gameObject.transform.Translate(new Vector3(0, 0, -1 * Time.deltaTime * speed), Space.World);
                }
            }
            else
            {
                if (this.gameObject.transform.position.z >= EndPointZ)
                {
                    GoBackZ = true;
                    if (!once && stopTimer <= 0)
                    {
                        once = true;
                        stopTimer = MaxStopTimer;
                    }
                }
                else
                {
                    if (stopTimer <= 0)
                    {
                        this.gameObject.transform.Translate(new Vector3(0, 0, 1 * Time.deltaTime * speed), Space.World);
                        once = false;
                    }
                }
            }
        }
        else
        {
            if (GoBackZ)
            {

                if (this.gameObject.transform.position.z >= StartPoint.z)
                {
                    GoBackZ = false;
                    if (!once && stopTimer <= 0)
                    {
                        once = true;
                        stopTimer = MaxStopTimer;
                    }
                }
                else
                {
                    if (stopTimer <= 0)
                    {
                        this.gameObject.transform.Translate(new Vector3(0, 0, 1 * Time.deltaTime * speed), Space.World);
                        once = false;
                    }
                }
            }
            else
            {
                if (this.gameObject.transform.position.z <= EndPointZ)
                {
                    GoBackZ = true;
                    if (!once && stopTimer <= 0)
                    {
                        once = true;
                        stopTimer = MaxStopTimer;
                    }
                }
                else
                {
                    if (stopTimer <= 0)
                    {
                        this.gameObject.transform.Translate(new Vector3(0, 0, -1 * Time.deltaTime * speed), Space.World);
                        once = false;
                    }
                }
            }
        }
    }
    void UpAndDown()
    {
        if (EndPointY > StartPoint.y)
        {
            if (GoBackY)
            {
                if (this.gameObject.transform.position.y <= StartPoint.y)
                {
                    GoBackY = false;
                    if (!once && stopTimer <= 0)
                    {
                        once = true;
                        stopTimer = MaxStopTimer;
                    }
                }
                else
                {
                    if (stopTimer <= 0)
                    {
                        this.gameObject.transform.Translate(new Vector3(0, -1 * Time.deltaTime * speed, 0), Space.World);
                        once = false;
                    }
                }
            }
            else
            {
                if (this.gameObject.transform.position.y >= EndPointY)
                {
                    GoBackY = true;
                    if (!once && stopTimer <= 0)
                    {
                        once = true;
                        stopTimer = MaxStopTimer;
                    }
                }
                else
                {
                    if (stopTimer <= 0)
                    {
                        this.gameObject.transform.Translate(new Vector3(0, 1 * Time.deltaTime * speed, 0), Space.World);
                        once = false;
                    }
                }
            }
        }
        else
        {
            if (GoBackY)
            {
                if (this.gameObject.transform.position.y >= StartPoint.y)
                {
                    GoBackY = false;
                    if (!once && stopTimer <= 0)
                    {
                        once = true;
                        stopTimer = MaxStopTimer;
                    }
                }
                else
                {
                    if (stopTimer <= 0)
                    {
                        this.gameObject.transform.Translate(new Vector3(0, 1 * Time.deltaTime * speed, 0), Space.World);
                        once = false;
                    }
                }
            }
            else
            {
                if (this.gameObject.transform.position.y <= EndPointY)
                {
                    GoBackY = true;
                    if (!once && stopTimer <= 0)
                    {
                        once = true;
                        stopTimer = MaxStopTimer;
                    }
                }
                else
                {
                    if (stopTimer <= 0)
                    {
                        this.gameObject.transform.Translate(new Vector3(0, -1 * Time.deltaTime * speed, 0), Space.World);
                        once = false;
                    }
                }
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

        if (col.gameObject.tag == "Player" && StepOnToActivate 
            && col.transform.position.y > transform.position.y)
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
