using UnityEngine;

public class SoccerBall : MonoBehaviour
{
    Rigidbody rb;
    [Tooltip("How far the ball gets kicked")]
    [SerializeField]
    float pushPower = 5.0f;
    Vector3 startpos;

    void Start()
    {
        startpos = this.gameObject.transform.position;
        rb = GetComponent<Rigidbody>();
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            Vector3 direction = (col.transform.position - transform.position).normalized;
            rb.AddForce(-direction * pushPower, ForceMode.Impulse);
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.name == "Out_Of_Bounds")
        {
            this.gameObject.transform.position = startpos;
            rb.velocity = new Vector3(0f, 0f, 0f);
        }
    }
}
