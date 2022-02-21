using UnityEngine;

public class NPCBounce : MonoBehaviour
{
    [SerializeField, Min(0f)]
    float acceleration = 10f, speed = 10f;
    [SerializeField, Min(.1f)]
    float SquishAmount = 2f;
    [SerializeField, Range(.1f, 6f)]
    float timeToSquish;
    private Vector3 origanalScale;
    private bool squishTime;
    private bool doneSquishing;
    private bool HoldingSpace;
    private SoundManager soundManager;
    public bool Triggered;
    public GameObject collision;
    private void Start()
    {
        origanalScale = transform.localScale;
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            HoldingSpace = true;
        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            HoldingSpace = false;
        }
        if(squishTime)
        {
            Squish();
        }
        if(Triggered)
        {
            if (collision.tag == "Player" && !collision.GetComponent<BlobBert>() 
                && collision.transform.position.y > gameObject.transform.position.y)
            {
                Rigidbody body = collision.GetComponent<Rigidbody>();
                if (body)
                {
                    collision.GetComponent<PlayerMovement>().Bounce = true;
                    collision.GetComponent<PlayerMovement>().PlayJumpAnimation();
                    Accelerate(body);
                }
                squishTime = true;
            }
        }
        Triggered = false;
    }
    void Accelerate(Rigidbody body)
    {
        Vector3 velocity = transform.InverseTransformDirection(body.velocity);

        if (!body.GetComponent<PlayerMovement>().anim.GetCurrentAnimatorStateInfo(0).IsName("GPFalling"))
        {
            if (acceleration > 0f)
            {
                velocity.y = Mathf.MoveTowards(velocity.y, speed, acceleration * Time.deltaTime);
            }
            else
            {
                velocity.y = speed;
            }
        } else
        {
            if (acceleration > 0f)
            {
                velocity.y = Mathf.MoveTowards(velocity.y, speed, acceleration * Time.deltaTime);
            }
            else
            {
                velocity.y = speed + (speed / 2);
            }
        }
        body.velocity = transform.TransformDirection(velocity);
        if (body.TryGetComponent(out PlayerMovement player))
        {
            player.PreventSnapToGround();
        }
    }
    void Squish()
    {
        if(!doneSquishing)
        {
            if(transform.localScale.y > origanalScale.y / SquishAmount)
            {
                transform.localScale -= new Vector3(0, timeToSquish, 0) * Time.deltaTime;
                transform.position -= new Vector3(0, timeToSquish, 0) * Time.deltaTime;
            } else
            {
                doneSquishing = true;
            }
        } 
        else
        {
            if (transform.localScale.y < origanalScale.y)
            {
                transform.localScale += new Vector3(0, timeToSquish, 0) * Time.deltaTime;
                transform.position += new Vector3(0, timeToSquish, 0) * Time.deltaTime;
            }
            else
            {
                transform.localScale = origanalScale;
                doneSquishing = false;
                squishTime = false;
            }
        }
    }
}
