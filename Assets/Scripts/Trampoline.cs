using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [SerializeField, Min(0f)]
    float acceleration = 10f, speed = 10f;
    [SerializeField, Range(.1f, 6f)]
    float timeToSquish;
    private Vector3 origanalScale;
    private bool squishTime;
    private bool doneSquishing;
    private bool HoldingSpace;
    private void Start()
    {
        origanalScale = transform.localScale;
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
    }
    private void OnCollisionEnter(Collision col)
    {
        //blobBert shouldn't be able to jump on the slimeoline
        if (col.gameObject.tag == "Player" && !col.gameObject.GetComponent<BlobBert>())
        {
            if (col.gameObject.transform.position.y > gameObject.transform.position.y)
            {
                if (HoldingSpace)
                {
                    //this shoots the player up
                    //col.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * (HighBounceForce + (col.gameObject.GetComponent<PlayerMovement>().jumpHeight)), ForceMode.VelocityChange);
                    //col.gameObject.GetComponent<PlayerMovement>().jumpOnBouncePad(HighBounceForce);
                }
                else
                {
                    //this shoots the player up
                    //col.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * (BounceForce + (col.gameObject.GetComponent<PlayerMovement>().jumpHeight)), ForceMode.VelocityChange);
                    //col.gameObject.GetComponent<PlayerMovement>().jumpOnBouncePad(BounceForce);
                }
                squishTime = true;
            }
            else
            {
                Vector3 pushDir = transform.position - col.transform.position;

                col.gameObject.GetComponent<Rigidbody>().velocity = pushDir * 10;
            }
        }
    }
    void OnTriggerStay(Collider other)
    {
        Rigidbody body = other.attachedRigidbody;
        if (body)
        {
            Accelerate(body);
        }
    }

    void Accelerate(Rigidbody body)
    {
        Vector3 velocity = transform.InverseTransformDirection(body.velocity); ;


        if (acceleration > 0f)
        {
            velocity.y = Mathf.MoveTowards(velocity.y, speed, acceleration * Time.deltaTime);
        }
        else
        {
            velocity.y = speed;
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
            if(transform.localScale.y > origanalScale.y / 2)
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
                doneSquishing = false;
                squishTime = false;
            }
        }
    }
}
