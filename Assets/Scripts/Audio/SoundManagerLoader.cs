using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class SoundManagerLoader : MonoBehaviour
{
    PhotonView view;

    bool rpc_called = false;
    // Start is called before the first frame update
    private void Update()
    {
        if (!rpc_called)
        {
            view = GetComponent<PhotonView>();
            Debug.Log("Calling RPC");
            view.RPC("load_sound_manager", RpcTarget.All);
            rpc_called = true;
        }
    }

    [PunRPC]
    public void load_sound_manager()
    {
        GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().on_scene_loaded();
    }
}
