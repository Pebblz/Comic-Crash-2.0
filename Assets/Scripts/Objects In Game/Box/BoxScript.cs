using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System.IO;
[RequireComponent(typeof(BoxCollider))]
public class BoxScript : MonoBehaviour
{
    //[SerializeField] GameObject BrokenBox;
    [SerializeField] List<WaysToBreak> Ways;
    private MainCamera cam;
    private bool wasPunched;
    private SoundManager soundManager;
    PhotonView photonView;
    //this will replace the unbroken box with the broken box
    private void Awake()
    {
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
        photonView = GetComponent<PhotonView>();
    }
    [PunRPC]
    void DestroyBox()
    {
        if (wasPunched)
        {
            cam = FindObjectOfType<MainCamera>();
            cam.GetComponent<MainCamera>().Shake(.1f, .1f);
        }
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "BrokenBox"), transform.position, transform.rotation);
        soundManager.playBoxBreak();
        photonView.TransferOwnership(PhotonFindCurrentClient().GetComponent<PhotonView>().ViewID);
        PhotonNetwork.Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision col)
    {
        if (Ways.Contains(WaysToBreak.Shoot))
        {
            //this will check if the thing that hit it is a bullet
            if (col.gameObject.GetComponent<Bullet>() != null)
            {
                photonView.RPC("DestroyBox", RpcTarget.All);
            }

            //somewhere here we need to have code to launch the pieces
            //from where ever the player punched / shot the crate

        }
        if (Ways.Contains(WaysToBreak.Punch))
        {
            if (col.gameObject.tag == "PlayerPunch")
            {
                photonView.RPC("DestroyBox", RpcTarget.All);
            }
        }
        if (Ways.Contains(WaysToBreak.JumpOn))
        {
            if (col.gameObject.tag == "Player")
            {
                if (!col.gameObject.GetComponent<PlayerMovement>().OnGround &&
                    col.gameObject.transform.position.y > gameObject.transform.position.y)
                {
                    col.gameObject.GetComponent<PlayerMovement>().jumpOnEnemy();
                    photonView.RPC("DestroyBox", RpcTarget.All);
                }
            }
        }
        if (Ways.Contains(WaysToBreak.JumpUnder))
        {
            if (!col.gameObject.GetComponent<PlayerMovement>().OnGround &&
                col.gameObject.transform.position.y < gameObject.transform.position.y)
            {
                photonView.RPC("DestroyBox", RpcTarget.All);
            }
        }
        if (Ways.Contains(WaysToBreak.GroundPound))
        {
            if (col.gameObject.GetComponent<PlayerGroundPound>())
            {
                if (col.gameObject.GetComponent<Animator>().GetBool("GroundPound"))
                {
                    DestroyBox();
                }
            }
        }
    }
    public void CheckPunchToSeeIfItShouldBreak()
    {
        if (Ways.Contains(WaysToBreak.Punch))
        {
            wasPunched = true;
            Invoke("DestroyBox", .15f);
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

    /// <summary>
    /// These are all the different ways that a box can get destroyed. Some boxes can get destroyed only in certain ways 
    /// </summary>
    enum WaysToBreak
    {
        JumpOn,
        JumpUnder,
        Shoot,
        Punch,
        GroundPound
    }
}
