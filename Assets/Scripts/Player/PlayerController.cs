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
    private readonly float maxSpeed = 5f;       // 속도의 길이
    private readonly float friction = 0.97f;    // 감속 정도(낮을수록 빠르게 감속)

    private ETeam team = ETeam.Red;
    public ETeam Team { get { return team; } }

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
        if (!photonView.IsMine) return;


        // 이동키가 눌러졌을 때
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
                photonView.RPC("FlagDrop", RpcTarget.All);
        }

#if UNITY_EDITOR
        DebugDrawVelocity();
#endif
    }

    public void Init(ETeam _team)
    {
        team = _team;
    }

    private void OnCollisionEnter(Collision _other)
    {
        if (!photonView.IsMine) return;


        // Drop Flag
        if (_other.gameObject.CompareTag("Player"))
        {
            photonView.RPC("FlagDrop", RpcTarget.All);
            return;
        }        
    }

    private void OnTriggerEnter(Collider _other)
    {
        // Catch Flag
        if (_other.gameObject.CompareTag("Flag"))
        {
            photonView.RPC("FlagCatch", RpcTarget.All, photonView.Owner.ActorNumber);
            return;
        }
    }

    private void DebugDrawVelocity()
    {
        Debug.DrawLine(transform.position, transform.position + rb.velocity, Color.yellow);
    }

    [PunRPC]
    public void FlagCatch(int _actorNum)
    {
        PhotonView pv = GetPhotonViewWithActorNumber(_actorNum);
        flag.Attach(pv.transform, pv.Owner.ActorNumber);
    }

    [PunRPC]
    public void FlagDrop()
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
}
