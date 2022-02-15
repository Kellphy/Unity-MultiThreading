using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectNetwork : NetworkManager
{
    GameObject spawner;

    public override void OnStartHost()
    {
        spawner = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Spawner"));
        NetworkServer.Spawn(spawner);
        base.OnStartHost();
    }

    public override void OnStopHost()
    {
        NetworkServer.Destroy(spawner);
        base.OnStopHost();
    }
}
