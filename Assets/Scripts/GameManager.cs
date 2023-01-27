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
    [SerializeField] private Spawner spawner = null;

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

    private static EGameState gameState = EGameState.Ready;
    public static EGameState GameState { get { return gameState; } }

    private void Start()
    {
        // 팀 정하기
        ETeam team = ETeam.Red;
        if (PhotonNetwork.CurrentRoom.PlayerCount != 1) team = ETeam.Blue;

        // 이미지 정하기
        Texture2D[] texList = Resources.LoadAll<Texture2D>("Textures\\Characters");
        Texture2D tex = texList[Random.Range(0, texList.Length)];

        // 커스텀프로퍼티에 저장해두면 다른 곳에서도 사용가능(현재는 사용안함)
        //ExitGames.Client.Photon.Hashtable hash =
        //    new ExitGames.Client.Photon.Hashtable()
        //    {
        //        ["Team"] = team,
        //        ["Tex"] = tex.name
        //    };
        //PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        spawner.SpawnPlayer(team, tex.name, GoalCallback);

        // 깃발은 마스터만 세팅(처음에만)
        if (PhotonNetwork.IsMasterClient)
            spawner.RespawnFlag();

        playButton.SetVisible(true);
        playButton.SetPressedCallback(OnPlayPressed);
        playButton.SetTextWaiting();

        timeOver.SetVisible(false);

        if (PhotonNetwork.IsMasterClient) masterTextGo.SetActive(true);
        else masterTextGo.SetActive(false);

#if UNITY_EDITOR
        // TEST TEST TEST
        if (PhotonNetwork.IsMasterClient) playButton.SetTextPlay();
#endif
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

        photonView.RPC(nameof(AddScoreRPC), RpcTarget.MasterClient, _team);
        spawner.RespawnFlag();
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

        //myPlayerCtrl.SetGameState(gameState);
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
        photonView.RPC(nameof(SetGameStateRPC), RpcTarget.All, EGameState.TimeOver);

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
        photonView.RPC(nameof(SetPlayButtonVisibleRPC), RpcTarget.All, false);

        photonView.RPC(nameof(SetGameStateRPC), RpcTarget.All, EGameState.Play);
        photonView.RPC(nameof(StartTimerRPC), RpcTarget.All, 60f);
    }

    [PunRPC]
    public void SetPlayButtonVisibleRPC(bool _isActive)
    {
        playButton.SetVisible(_isActive);
    }
}
