using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class Instantiatior : MonoBehaviourPunCallbacks
{
    public int playerIndex;
    public string prefabName;
    [SerializeField] Transform[] spawnPoints = new Transform[4];

    private void Start()
    {
        PhotonNetwork.Instantiate(prefabName, Vector3.zero, Quaternion.identity);
        BLogger.Print(MType.S, "Player Created");
    }

    public void SpawnPlayer()
    { 
    
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
    }
}
