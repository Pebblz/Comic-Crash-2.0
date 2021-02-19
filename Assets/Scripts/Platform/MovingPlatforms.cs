using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatforms : MonoBehaviour
{
    [Tooltip("If set true it'll go up and down, if false it'll move side to side")]
    [SerializeField]
    bool upAndDown;

    [Tooltip("The amount the platform will move")]
    [SerializeField]
    float DistanceToMove;

    [Tooltip("If up and down if false, this'll will be for if the platform will move straight and back or left and right")]
    [SerializeField]
    bool LeftAndRight;

    [Tooltip("The speed at which the platform moves")]
    [SerializeField]
    float speed = 2;

    private float EndPoint;

    private Vector3 StartPoint;

    private bool GoBack;
    // Start is called before the first frame update
    void Start()
    {
        StartPoint = transform.position;
        if (upAndDown)
            EndPoint = StartPoint.y + DistanceToMove;

        if (!upAndDown && !LeftAndRight)
            EndPoint = StartPoint.z + DistanceToMove;

        if (!upAndDown && LeftAndRight)
            EndPoint = StartPoint.x + DistanceToMove;
    }

    // Update is called once per frame
    void Update()
    {
        if (upAndDown)
        {
            if (GoBack)
            {
                this.gameObject.transform.Translate(new Vector3(0, -1 * Time.deltaTime * speed, 0), Space.World);
                if (this.gameObject.transform.position.y <= StartPoint.y)
                {
                    GoBack = false;
                }
            }
            else
            {

                this.gameObject.transform.Translate(new Vector3(0, 1 * Time.deltaTime * speed, 0), Space.World);
                if (this.gameObject.transform.position.y >= EndPoint)
                {
                    GoBack = true;
                }
            }
        }
        else
        {
            if (!LeftAndRight)
            {
                if (GoBack)
                {
                    this.gameObject.transform.Translate(new Vector3(0, 0, -1 * Time.deltaTime * speed), Space.World);
                    if (this.gameObject.transform.position.z <= StartPoint.z)
                    {
                        GoBack = false;
                    }
                } else
                {
                    this.gameObject.transform.Translate(new Vector3(0, 0, 1 * Time.deltaTime * speed), Space.World);
                    if (this.gameObject.transform.position.z >= EndPoint)
                    {
                        GoBack = true;
                    }
                }
            }
            else
            {
                if (GoBack)
                {
                    this.gameObject.transform.Translate(new Vector3(-1 * Time.deltaTime * speed, 0, 0), Space.World);
                    if (this.gameObject.transform.position.x <= StartPoint.x)
                    {
                        GoBack = false;
                    }
                }
                else
                {
                    this.gameObject.transform.Translate(new Vector3(1 * Time.deltaTime * speed, 0, 0), Space.World);
                    if (this.gameObject.transform.position.x >= EndPoint)
                    {
                        GoBack = true;
                    }
                }
            }
        }
    }
}
