using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using Unity.Netcode;
using UnityEngine;

public class Enemy : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer == false)
        {
            enabled = false;
            return;
        }

        PrimeTween.Tween.Position(transform, Vector3.zero, Vector3.up, 1f, cycles: -1, cycleMode: CycleMode.Yoyo);
    }

}
