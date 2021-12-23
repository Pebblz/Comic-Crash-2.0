using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WallSlideAudio : PlayByState
{
    public override bool determine_if_play()
    {
        var pm = GetComponentInParent<PlayerMovement>();
        if (pm.IsWallSliding)
        {
            return true;
        }

        return false;
    }

}
