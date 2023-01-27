using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable, IPunInstantiateMagicCallback
{
    [SerializeField] private GameObject playerInfoPrefab = null;

    private Rigidbody rb = null;
    private Flag flag = null;
    private Character2D char2D = null;

    private readonly float moveSpeed = 250f;
    private readonly float maxSpeed = 5f;           // 속도의 길이
    private readonly float friction = 0.97f;        // 감속정도(낮을수록 빠르게 감속)
    private readonly float remoteMoveSpeed = 5f;    // 원격 플레이어 이동보정 속도

    private ETeam team = ETeam.Red;
    public ETeam Team { get { return team; } }

    private VoidTeamDelegate goalCallback = null;

    private EGameState gameState = EGameState.Ready;

    private UIPlayerInfo playerInfo = null;
    private readonly int maxHp = 5;
    private int hp = 0;

    private Vector3 curPos = Vector3.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        GameObject flagGo = GameObject.FindGameObjectWithTag("Flag");
        flag = flagGo.GetComponent<Flag>();

        char2D = GetComponentInChildren<Character2D>();

        playerInfo = GetComponentInChildren<UIPlayerInfo>();

        curPos = transform.position;
    }

    public void Init(ETeam _team, string _texName, VoidTeamDelegate _goalCallback)
    {
        Debug.LogError("Init: " + team);
        team = _team;
        goalCallback = _goalCallback;

        // 멀티 플레이어 정보 동기화
        if (photonView.IsMine)
        {
            photonView.RPC(
                nameof(ApplyPlayerInfoRPC),
                RpcTarget.AllBuffered,
                photonView.Owner.ActorNumber, team, _texName, photonView.Owner.NickName);
        }
    }

    [PunRPC]
    private void ApplyPlayerInfoRPC(int _actorNum, ETeam _team, string _texName, string _nickName)
    {
        // ActorNumber를 이용해 같은 멀티 플레이어만 적용
        if (photonView.Owner.ActorNumber == _actorNum)
        {
            team = _team;
            char2D.SetCharTexture(_texName);
            playerInfo.SetNickName(_team, _nickName);

            // 처음에는 전체에 한번 날려줘야 함
            hp = maxHp;
            photonView.RPC(
                    nameof(UpdateHpRPC),
                    RpcTarget.All,
                    team, hp, maxHp);
        }
    }

    private void InitHp()
    {
        hp = maxHp;
        UpdateHp();
    }

    private void Damage(int _dmg)
    {
        hp -= _dmg;
        if (hp < 0)
        {
            hp = 0;
            Invoke("InitHp", 3f);
        }
        UpdateHp();
    }

    private void UpdateHp()
    {
        if (photonView.IsMine)
            photonView.RPC(
                nameof(UpdateHpRPC),
                RpcTarget.All,
                team, hp, maxHp);
    }

    [PunRPC]
    private void UpdateHpRPC(ETeam _team, int _hp, int _maxHp)
    {
        playerInfo.UpdateHp(_team, _hp, _maxHp);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 로컬 플레이어의 위치 정보 송신
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else // 원격 플레이어의 위치 정보 수신
        {
            curPos = (Vector3)stream.ReceiveNext();
        }
    }

    private void Update()
    {
        if (GameManager.GameState != EGameState.Play) return;
        if (hp == 0) return;

        if (photonView.IsMine)
        {
            // 이동키가 눌러졌을 때
            bool isMoving = false;
            if (Input.GetKey(KeyCode.W))
            {
                photonView.RPC(nameof(SetCharDirectionRPC), RpcTarget.All, photonView.ViewID, EDirection.Up);
                if (rb.velocity.z < maxSpeed)
                {
                    rb.AddForce(Vector3.forward * moveSpeed * Time.deltaTime);
                    isMoving = true;
                }
            }
            if (Input.GetKey(KeyCode.D))
            {
                photonView.RPC(nameof(SetCharDirectionRPC), RpcTarget.All, photonView.ViewID, EDirection.Right);
                if (rb.velocity.x < maxSpeed)
                {
                    rb.AddForce(Vector3.right * moveSpeed * Time.deltaTime);
                    isMoving = true;
                }
            }
            if (Input.GetKey(KeyCode.S))
            {
                photonView.RPC(nameof(SetCharDirectionRPC), RpcTarget.All, photonView.ViewID, EDirection.Down);
                if (rb.velocity.z > -maxSpeed)
                {
                    rb.AddForce(Vector3.back * moveSpeed * Time.deltaTime);
                    isMoving = true;
                }
            }
            if (Input.GetKey(KeyCode.A))
            {
                photonView.RPC(nameof(SetCharDirectionRPC), RpcTarget.All, photonView.ViewID, EDirection.Left);
                if (rb.velocity.x > -maxSpeed)
                {
                    rb.AddForce(Vector3.left * moveSpeed * Time.deltaTime);
                    isMoving = true;
                }
            }

            // 이동하고 있지 않을 때 좀 더 빠르게 감속
            if (isMoving == false)
            {
                if (rb.velocity.magnitude > 0f)
                    rb.velocity *= friction;
            }

            // 깃발 버리기
            if (Input.GetKey(KeyCode.R))
            {
                // 깃발을 소유하고 있는 액터와 현재 조작중인 액터가 같은지 검사
                if (photonView.Owner.ActorNumber == flag.OwnerActorNum)
                    photonView.RPC(nameof(FlagDropRPC), RpcTarget.All);
            }

#if UNITY_EDITOR
            DebugDrawVelocity();
#endif
        }
        else
        {
            // 원격 플레이어 위치 처리
            transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * remoteMoveSpeed);
            return;
        }
    }

    [PunRPC]
    private void SetCharDirectionRPC(int _viewID, EDirection _dir)
    {
        // ViewID를 이용해 같은 멀티 플레이어만 적용
        if (photonView.ViewID == _viewID)
            char2D.SetDirection(_dir);
    }

    private void OnCollisionEnter(Collision _other)
    {
        if (!photonView.IsMine) return;


        // Drop Flag
        if (_other.gameObject.CompareTag("Player"))
        {
            photonView.RPC(nameof(FlagDropRPC), RpcTarget.All);
            return;
        }

        Damage(1);
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (!photonView.IsMine) return;


        // Catch Flag
        if (_other.gameObject.CompareTag("Flag"))
        {
            photonView.RPC(nameof(FlagCatchRPC), RpcTarget.All, photonView.Owner.ActorNumber);
            return;
        }

        if (_other.gameObject.CompareTag("Goal"))
        {
            Goal goal = _other.GetComponent<Goal>();
            // 골에 지정된 팀과 나의 팀이 같고,
            // 깃발을 들고 있다면 골 인정
            if (goal.Team == team && flag.IsAttach())
            {
                goalCallback?.Invoke(team);
                return;
            }
        }
    }

    private void DebugDrawVelocity()
    {
        Debug.DrawLine(transform.position, transform.position + rb.velocity, Color.yellow);
    }

    [PunRPC]
    public void FlagCatchRPC(int _actorNum)
    {
        PhotonView pv = GetPhotonViewWithActorNumber(_actorNum);
        flag.Attach(pv.transform, pv.Owner.ActorNumber);
    }

    [PunRPC]
    public void FlagDropRPC()
    {
        flag.Detach();
    }

    private PhotonView GetPhotonViewWithActorNumber(int _actorNum)
    {
        PhotonView[] photonViews = FindObjectsOfType<PhotonView>();

        for (int i = 0; i < photonViews.Length; ++i)
        {
            if (photonViews[i].isRuntimeInstantiated == false) continue;

            int viewNum = photonViews[i].Owner.ActorNumber;
            if (viewNum == _actorNum)
            {
                return photonViews[i];
            }
        }

        return null;
    }

    public void SetGameState(EGameState _gameState)
    {
        gameState = _gameState;
    }

    // 커스텀 프로퍼티 정보가 변경되면 호출
    public override void OnPlayerPropertiesUpdate(
        Player _targetPlayer,
        ExitGames.Client.Photon.Hashtable _changedProps)
    {

    }

    // 게임오브젝트가 생성되면 호출
    // IPunInstantiateMagicCallback
    void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
    {

    }
}