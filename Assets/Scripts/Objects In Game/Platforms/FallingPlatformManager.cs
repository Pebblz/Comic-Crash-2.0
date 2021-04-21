using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatformManager : MonoBehaviour
{
    public GameObject[] _fallingPlatforms;


    public void PlatformFalling(GameObject platform, Vector3 Originalpos, Quaternion Originalrot)
    {
        for(int i = 0; i < _fallingPlatforms.Length; i++)
        {
            if(_fallingPlatforms[i] == platform)
            {
                ResetPlatform(platform, i, Originalpos, Originalrot);
            }
        }
    }
    private void ResetPlatform(GameObject platform, int index, Vector3 Originalpos, Quaternion Originalrot)
    {
        GameObject oldPlatform = _fallingPlatforms[index];
        GameObject temp = Instantiate(platform,Originalpos, Originalrot);
        _fallingPlatforms[index] = temp;
        Destroy(oldPlatform);
    }
}
