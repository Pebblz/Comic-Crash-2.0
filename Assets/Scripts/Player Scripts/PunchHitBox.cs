using UnityEngine;
using Photon.Pun;
public class PunchHitBox : MonoBehaviour
{
    float DestroyTimer = .4f;
    int damage = 2;

    PhotonView photonView;
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }
    void Update()
    {
        DestroyTimer -= Time.deltaTime;
        if (photonView.IsMine)
        {
            if (DestroyTimer <= 0)
                PhotonNetwork.Destroy(gameObject);
        }
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

        if(col.gameObject.tag == "Enemy")
        {
            Enemy enemy = (Enemy)col.gameObject.GetComponent(typeof(Enemy));
            enemy.damage(damage);
        }
    }
}
