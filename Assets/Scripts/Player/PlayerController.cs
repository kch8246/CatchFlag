using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPun
{
    private Rigidbody rb = null;
    private Flag flag = null;
    private Character2D char2D = null;

    private readonly float moveSpeed = 250f;
    private readonly float maxSpeed = 5f;       // �ӵ��� ����
    private readonly float friction = 0.97f;    // ��������(�������� ������ ����)

    private ETeam team = ETeam.Red;
    public ETeam Team { get { return team; } }

    private VoidTeamDelegate goalCallback = null;

    public EGameState gameState = EGameState.Ready;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        GameObject flagGo = GameObject.FindGameObjectWithTag("Flag");
        flag = flagGo.GetComponent<Flag>();

        char2D = GetComponentInChildren<Character2D>();
    }

    private void Start()
    {
        if (!photonView.IsMine) return;


        char2D.SetRamdomCharTexture();
    }

    private void Update()
    {
        if (gameState == EGameState.Ready || gameState == EGameState.TimeOver) return;
        if (!photonView.IsMine) return;


        // �̵�Ű�� �������� ��
        bool isMoving = false;
        if (Input.GetKey(KeyCode.W))
        {
            char2D.SetDirection(EDirection.Up);
            if (rb.velocity.z < maxSpeed)
            {
                rb.AddForce(Vector3.forward * moveSpeed * Time.deltaTime);
                isMoving = true;
            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            char2D.SetDirection(EDirection.Right);
            if (rb.velocity.x < maxSpeed)
            {
                rb.AddForce(Vector3.right * moveSpeed * Time.deltaTime);
                isMoving = true;
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            char2D.SetDirection(EDirection.Down);
            if (rb.velocity.z > -maxSpeed)
            {
                rb.AddForce(Vector3.back * moveSpeed * Time.deltaTime);
                isMoving = true;
            }
        }
        if (Input.GetKey(KeyCode.A))
        {
            char2D.SetDirection(EDirection.Left);
            if (rb.velocity.x > -maxSpeed)
            {
                rb.AddForce(Vector3.left * moveSpeed * Time.deltaTime);
                isMoving = true;
            }
        }

        // �̵��ϰ� ���� ���� �� �� �� ������ ����
        if (isMoving == false)
        {
            if (rb.velocity.magnitude > 0f)
                rb.velocity *= friction;
        }

        // ��� ������
        if (Input.GetKey(KeyCode.R))
        {
            // ����� �����ϰ� �ִ� ���Ϳ� ���� �������� ���Ͱ� ������ �˻�
            if (photonView.Owner.ActorNumber == flag.OwnerActorNum)
                photonView.RPC("FlagDropRPC", RpcTarget.All);
        }

#if UNITY_EDITOR
        DebugDrawVelocity();
#endif
    }

    public void Init(ETeam _team, VoidTeamDelegate _goalCallback)
    {
        team = _team;
        goalCallback = _goalCallback;
    }

    private void OnCollisionEnter(Collision _other)
    {
        if (!photonView.IsMine) return;


        // Drop Flag
        if (_other.gameObject.CompareTag("Player"))
        {
            photonView.RPC("FlagDropRPC", RpcTarget.All);
            return;
        }        
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (!photonView.IsMine) return;


        // Catch Flag
        if (_other.gameObject.CompareTag("Flag"))
        {
            photonView.RPC("FlagCatchRPC", RpcTarget.All, photonView.Owner.ActorNumber);
            return;
        }

        if (_other.gameObject.CompareTag("Goal"))
        {
            Goal goal = _other.GetComponent<Goal>();
            // �� ������ ���� ���� ���� ����,
            // ����� ��� �ִٸ� �� ����
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
}
