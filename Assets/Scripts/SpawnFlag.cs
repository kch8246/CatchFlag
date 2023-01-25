using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpawnFlag : MonoBehaviourPun
{
    [SerializeField] private Flag flag = null;

    public void Respawn()
    {
        photonView.RPC("RespawnRPC", RpcTarget.All);
    }

    [PunRPC]
    public void RespawnRPC()
    {
        if (flag == null) return;

        flag.Respawn(transform.position);
    }
}
