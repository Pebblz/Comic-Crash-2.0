using UnityEngine;

public class PunchHitBox : MonoBehaviour
{
    float DestroyTimer = .4f;
    void Update()
    {
        DestroyTimer -= Time.deltaTime;
        if (DestroyTimer <= 0)
            Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<BoxScript>())
        {
            col.gameObject.GetComponent<BoxScript>().CheckPunchToSeeIfItShouldBreak();
        }
        if (col.gameObject.tag == "Bully")
        {
            col.GetComponent<BullyAI>().Stumble();

            Vector3 pushDir = new Vector3(col.transform.position.x, 0, col.transform.position.z) - 
                new Vector3(transform.position.x, 0, transform.position.z);

            col.GetComponent<BullyAI>().HitBack(pushDir);
;
        }
    }
}
