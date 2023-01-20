using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpawnPlayer : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab = null;
    [SerializeField] private ETeam team = ETeam.Red;

    private GameObject playerGo = null;

    public void Spawn()
    {
        if (playerPrefab == null) return;

        playerGo = PhotonNetwork.Instantiate(
            "Prefabs\\" + playerPrefab.name,
            transform.position + new Vector3(0f, 0.1f, 0f),
            Quaternion.identity,
            0);
        playerGo.GetComponent<PlayerController>().Init(team);
    }
}
