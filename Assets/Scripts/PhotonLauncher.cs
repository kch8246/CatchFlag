using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;


public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    [SerializeField] private string gameVersion = "0.0.1";
    [SerializeField] private byte maxPlyaerPerRoom = 2;

    private string nickName = string.Empty;

    [SerializeField] private Button connectButton = null;


    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        connectButton.interactable = false;

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // Connect Button이 눌러지면 호출
    public void Connect()
    {
        if(string.IsNullOrEmpty(nickName))
        {
            // 닉네임을 입력하지 않았다면 랜덤번호 할당
            nickName = Random.Range(1, 1000).ToString("D4");
        }

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    // InputField_NickName과 연결해 닉네임을 가져옴
    public void OnValueChangedNickName(string _nickName)
    {
        nickName = _nickName;

        // 유저 이름 지정
        PhotonNetwork.NickName = nickName;
    }

    public override void OnConnectedToMaster()
    {
        connectButton.interactable = true;
    }

    public override void OnJoinedRoom()
    {
        SceneManager.LoadScene("PlayScene");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connectButton.interactable = true;

        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlyaerPerRoom });
    }
}