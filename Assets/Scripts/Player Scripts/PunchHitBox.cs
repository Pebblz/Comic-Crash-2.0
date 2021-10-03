using UnityEngine;
using Photon.Pun;
public class PunchHitBox : MonoBehaviour
{
    float DestroyTimer = .4f;
    void Update()
    {
        DestroyTimer -= Time.deltaTime;
        if (DestroyTimer <= 0)
            PhotonNetwork.Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<BoxScript>())
        {
            col.gameObject.GetComponent<BoxScript>().CheckPunchToSeeIfItShouldBreak();
        }
        if (col.gameObject.tag == "Bully")
        {
            Vector3 direction = transform.position - col.transform.position;

            if (Vector3.Dot(-col.gameObject.transform.forward, direction) > 0)
            {
                col.GetComponent<BullyAI>().StartDeath();
            } 
            else
            {
                col.GetComponent<BullyAI>().Stumble();

                Vector3 pushDir = new Vector3(col.transform.position.x, 0, col.transform.position.z) -
                    new Vector3(transform.position.x, 0, transform.position.z);

                col.GetComponent<BullyAI>().HitBack(pushDir, col.GetComponent<BullyAI>().KnockBackBullyPower);
            }
        }
    }
}
