using UnityEngine;
using Photon.Pun;
public class BoxPieces : MonoBehaviour
{
    private float timer;
    PhotonView photonView;
    GameObject player;
    private void Awake()
    {
        timer = Random.RandomRange(4, 7);
        photonView = GetComponent<PhotonView>();
        player = PhotonFindCurrentClient();
    }
    void Update()
    {
        timer -= Time.deltaTime;
        if (player == null)
            player = PhotonFindCurrentClient();

        if (timer <= 0)
        {
            if (player.GetComponent<PhotonView>().IsMine)
            {
                photonView.RPC("DestroyPieces", RpcTarget.All);
            }
        }
    }
    [PunRPC]
    void DestroyPieces()
    {
        PhotonNetwork.Destroy(gameObject);
    }
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player" || col.gameObject.tag == "PlayerPunch")
        {
            Physics.IgnoreCollision(this.gameObject.GetComponent<Collider>(), col.gameObject.GetComponent<Collider>());
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
}
