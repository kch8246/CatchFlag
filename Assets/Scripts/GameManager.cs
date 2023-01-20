using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private SpawnPlayer spawnPtRed = null;
    [SerializeField] private SpawnPlayer spawnPtBlue = null;
    [SerializeField] private SpawnFlag spawnPtFlag = null;

    private void Start()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1) spawnPtRed.Spawn();
        else spawnPtBlue.Spawn();

        if (PhotonNetwork.IsMasterClient)
            spawnPtFlag.Respawn();
    }

    // PhotonNetwork.LeaveRooom 함수가 호출되면 호출
    public override void OnLeftRoom()
    {
        //Debug.Log("Left Room");

        SceneManager.LoadScene("LauncherScene");
    }

    // 플레이어가 입장할 때 호출되는 함수
    public override void OnPlayerEnteredRoom(Player otherPlayer)
    {
        //Debug.LogFormat("Player Entered Room: {0}",
        //                otherPlayer.NickName);
    }

    // 플레이어가 나갈 때 호출되는 함수
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //Debug.LogFormat("Player Left Room: {0}",
        //                otherPlayer.NickName);
    }

    public void LeaveRoom()
    {
        //Debug.Log("Leave Room");

        PhotonNetwork.LeaveRoom();
    }
}
