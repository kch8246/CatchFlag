using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Spawner : MonoBehaviourPun
{
    [SerializeField] private GameObject playerPrefab = null;

    [SerializeField] private Transform spawnPtRedTr = null;
    [SerializeField] private Transform spawnPtBlueTr = null;
    [SerializeField] private Transform spawnFlagTr = null;
    
    [SerializeField] private Flag flag = null;

    private GameObject playerRedGo = null;
    private GameObject playerBlueGo = null;

    public void SpawnPlayer(ETeam _team, string _texName, VoidTeamDelegate _goalCallback)
    {
        if (playerPrefab == null) return;

        Vector3 pos = _team == ETeam.Red ? spawnPtRedTr.position : spawnPtBlueTr.position;

        GameObject playerGo = PhotonNetwork.Instantiate(
            "Prefabs\\" + playerPrefab.name,
            pos + new Vector3(0f, 0.1f, 0f),
            Quaternion.identity,
            0);
        playerGo.GetComponent<PlayerController>().Init(_team, _texName, _goalCallback);

        if (_team == ETeam.Red) playerRedGo = playerGo;
        else playerBlueGo = playerGo;
    }

    public void RespawnFlag()
    {
        photonView.RPC("RespawnFlagRPC", RpcTarget.All);
    }

    [PunRPC]
    public void RespawnFlagRPC()
    {
        if (flag == null) return;

        flag.Respawn(spawnFlagTr.position);
    }
}
