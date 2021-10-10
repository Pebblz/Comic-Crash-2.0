using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
public class FallingPlatformManager : MonoBehaviour
{
    public GameObject[] _fallingPlatforms;


    public void PlatformFalling(GameObject platform, Vector3 Originalpos, Quaternion Originalrot, PhotonView photonView)
    {
        for(int i = 0; i < _fallingPlatforms.Length; i++)
        {
            if(_fallingPlatforms[i] == platform)
            {
                ResetPlatform(platform, i, Originalpos, Originalrot, photonView);
            }
        }
    }
    private void ResetPlatform(GameObject platform, int index, Vector3 Originalpos, Quaternion Originalrot, PhotonView photonView)
    {
        GameObject oldPlatform = _fallingPlatforms[index];
        GameObject temp = Instantiate(platform,Originalpos, Originalrot);
        _fallingPlatforms[index] = temp;
        photonView.RPC("DestroyGameObject", RpcTarget.All, photonView);
    }
    [PunRPC]
    void DestroyGameObject(PhotonView photonView)
    {
        PhotonNetwork.Destroy(photonView);
    }
}
