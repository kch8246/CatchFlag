using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("-- Spawn Point --")]
    [SerializeField] private SpawnPlayer spawnPtRed = null;
    [SerializeField] private SpawnPlayer spawnPtBlue = null;
    [SerializeField] private SpawnFlag spawnPtFlag = null;

    [Header("-- Score --")]
    [SerializeField] private UIScore uiScore = null;
    private int scoreRed = 0;
    private int scoreBlue = 0;

    [Header("-- Timer --")]
    [SerializeField] private UITimer uiTimer = null;
    private bool isTimeOver = false;
    private float playTimeSec = 60f;
    private int minute = 0;
    private int seconds = 0;

    [Header("-- UI --")]
    [SerializeField] private UIPlayButton playButton = null;
    [SerializeField] private UITimeOver timeOver = null;

    [SerializeField] private GameObject masterTextGo = null;

    private EGameState gameState = EGameState.Ready;
    private PlayerController myPlayerCtrl = null;

    private void Start()
    {
        Debug.Log(PhotonNetwork.IsConnected);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1) myPlayerCtrl = spawnPtRed.Spawn(GoalCallback);
        else myPlayerCtrl = spawnPtBlue.Spawn(GoalCallback);

        if (PhotonNetwork.IsMasterClient)
            spawnPtFlag.Respawn();

        playButton.SetVisible(true);
        playButton.SetPressedCallback(OnPlayPressed);
        playButton.SetTextWaiting();

        timeOver.SetVisible(false);

        if (PhotonNetwork.IsMasterClient) masterTextGo.SetActive(true);
        else masterTextGo.SetActive(false);
    }

    // 플레이어가 입장할 때 호출되는 함수
    public override void OnPlayerEnteredRoom(Player otherPlayer)
    {
        //Debug.LogFormat("Player Entered Room: {0}",
        //                otherPlayer.NickName);

        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            if (PhotonNetwork.IsMasterClient) playButton.SetTextPlay();
        }
    }

    public void GoalCallback(ETeam _team)
    {
        //Debug.Log("Goal!: " + _team);

        photonView.RPC("AddScoreRPC", RpcTarget.MasterClient, _team);
        spawnPtFlag.Respawn();
    }

    [PunRPC]
    public void AddScoreRPC(ETeam _team)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (_team == ETeam.Red) ++scoreRed;
            else ++scoreBlue;

            uiScore.UpdateScore(scoreRed, scoreBlue);
        }
    }

    [PunRPC]
    public void SetGameStateRPC(EGameState _gameState)
    {
        gameState = _gameState;

        myPlayerCtrl.SetGameState(gameState);
    }

    [PunRPC]
    public void StartTimerRPC(float _playTimeSec)
    {
        StopAllCoroutines();

        playTimeSec = _playTimeSec;
        StartCoroutine(TimerCoroutine());
    }

    public void TimeOver()
    {
        //Debug.Log("Time Over");
        photonView.RPC("SetGameStateRPC", RpcTarget.All, EGameState.TimeOver);

        timeOver.SetVisible(true);
    }

    private IEnumerator TimerCoroutine()
    {
        while (!isTimeOver)
        {
            playTimeSec -= Time.deltaTime;
            if (playTimeSec <= 0f)
            {
                Debug.Log("Time Over");

                playTimeSec = 0f;
                isTimeOver = true;

                TimeOver();
            }

            minute = (int)((playTimeSec % 3600.0f) / 60.0f);
            seconds = (int)(playTimeSec % 60.0f);

            uiTimer.UpdateTimer(minute, seconds);

            yield return null;
        }
    }

    // 플레이 버튼 눌러졌을 때
    public void OnPlayPressed()
    {
        photonView.RPC("SetPlayButtonVisibleRPC", RpcTarget.All, false);

        photonView.RPC("SetGameStateRPC", RpcTarget.All, EGameState.Play);
        photonView.RPC("StartTimerRPC", RpcTarget.All, 60f);
    }

    [PunRPC]
    public void SetPlayButtonVisibleRPC(bool _isActive)
    {
        playButton.SetVisible(_isActive);
    }
}
