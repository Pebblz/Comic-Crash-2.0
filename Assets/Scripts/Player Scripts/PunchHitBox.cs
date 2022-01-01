using UnityEngine;
using Photon.Pun;
public class PunchHitBox : MonoBehaviour
{
    float DestroyTimer = .4f;
    int damage = 2;
    public bool GroundPound;
    PhotonView photonView;
    GameObject player;
    SoundManager soundManager;
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        soundManager = GetComponent<SoundManager>();
    }
    void Update()
    {
        if (!GroundPound)
        {
            DestroyTimer -= Time.deltaTime;
            if (photonView.IsMine)
            {
                if (DestroyTimer <= 0)
                    PhotonNetwork.Destroy(gameObject);
            }
        }
        else
        {
            if (photonView.IsMine)
            {
                if (player == null)
                    player = PhotonFindCurrentClient();
                else
                {
                    if(player.GetComponent<PlayerMovement>().OnGround || player.GetComponent<PlayerMovement>().InWater)
                        PhotonNetwork.Destroy(gameObject);
                }
            }
        }
    }
    GameObject PhotonFindCurrentClient()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject g in players)
        {
            if (g.GetComponent<PhotonView>().IsMine)
                return g;
        }
        return null;
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
        //if(col.gameObject.tag == "BoxBreak")
        //{
        //    soundManager.playBoxBreak(transform.position);
        //    Destroy(col.gameObject);
        //}
        if (col.gameObject.tag == "Enemy" && !col.isTrigger)
        {
            Enemy enemy = (Enemy)col.gameObject.GetComponent(typeof(Enemy));
            enemy.damage(damage);
        }
    }
}
