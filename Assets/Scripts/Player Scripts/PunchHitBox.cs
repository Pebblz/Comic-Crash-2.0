using UnityEngine;
using Photon.Pun;
public class PunchHitBox : MonoBehaviour
{
    float DestroyTimer = .4f;
    int damage = 2;
    public bool GroundPound;
    PhotonView photonView;
    GameObject player;
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
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
                    if (player.GetComponent<PlayerMovement>().OnGround || player.GetComponent<PlayerMovement>().InWater)
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
        if (col.gameObject.GetComponent<JunkPile>())
        {
            col.gameObject.GetComponent<JunkPile>().BreakJunkPile();
        }
        if (col.gameObject.tag == "Enemy" && !col.isTrigger)
        {
            Enemy enemy = (Enemy)col.gameObject.GetComponent(typeof(Enemy));
            enemy.damage(damage);
        }
        if (GroundPound && col.gameObject.GetComponent<SandPiles>())
            col.gameObject.GetComponent<SandPiles>().DestroyPile();
    }
}
