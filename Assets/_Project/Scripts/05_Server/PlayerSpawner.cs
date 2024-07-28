using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject prefab;

    public void PlayerJoined(PlayerRef player)

    {
        if (player == Runner.LocalPlayer)
        {
            Runner.Spawn(prefab, new Vector3(0, 0, 0), Quaternion.identity, player);
        }
    }
}
