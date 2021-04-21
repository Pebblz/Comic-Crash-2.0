using UnityEngine;

public class SoccerBall : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField, Range(5f, 100f), Tooltip("How far the ball gets kicked")]
    float pushPower = 5.0f;
    Vector3 startpos;

    void Start()
    {
        startpos = this.gameObject.transform.position;
        rb = GetComponent<Rigidbody>();
    }

    #region Triggers
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            Vector3 direction = (col.transform.position - transform.position).normalized;
            rb.AddForce(-direction * pushPower, ForceMode.Impulse);
        }
        if (col.gameObject.name == "Out_Of_Bounds")
        {
            this.gameObject.transform.position = startpos;
            rb.velocity = new Vector3(0f, 0f, 0f);
        }
    }
    #endregion
}
