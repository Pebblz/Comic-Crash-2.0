using UnityEngine;
using Photon.Pun;
public class BoxPieces : MonoBehaviour
{
    private float timer;
    private void Awake()
    {
        timer = Random.RandomRange(4, 7);
    }
    void Update()
    {
        timer -= Time.deltaTime;
        
        if (timer <= 0)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player" || col.gameObject.tag == "PlayerPunch")
        {
            Physics.IgnoreCollision(this.gameObject.GetComponent<Collider>(), col.gameObject.GetComponent<Collider>());
        }
    }
}
