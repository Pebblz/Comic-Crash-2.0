using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [SerializeField, Range(1f, 20f)]
    float BounceForce;
    [SerializeField, Range(.1f, 6f)]
    float timeToSquish;
    private Vector3 origanalScale;
    private bool squishTime;
    private bool doneSquishing;
    private void Start()
    {
        origanalScale = transform.localScale;
    }
    private void Update()
    {
        if(squishTime)
        {
            Squish();
        }
    }
    private void OnCollisionEnter(Collision col)
    {
        //blobBert shouldn't be able to jump on the slimeoline
        if(col.gameObject.tag == "Player" && !col.gameObject.GetComponent<BlobBert>())
        {
            if (col.gameObject.transform.position.y > gameObject.transform.position.y)
            {
                //this shoots the player up
                col.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * BounceForce,ForceMode.VelocityChange);
                squishTime = true;
            }
            else
            {
                Vector3 pushDir = transform.position - col.transform.position;

               col.gameObject.GetComponent<Rigidbody>().velocity = pushDir * 10;
            }
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
