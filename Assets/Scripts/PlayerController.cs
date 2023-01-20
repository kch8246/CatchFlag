using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPun
{
    [SerializeField] private Transform flagAttachPointTr = null;

    private Camera mainCam = null;
    private Rigidbody rb = null;

    [SerializeField] private ETeam team = ETeam.Red;

    private float moveSpeed = 200f;
    //private float breakPower = 1f;
    private float boundPower = 200f;
    private readonly float maxSpeed = 8f;    // 속도의 길이

    private void Awake()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (!photonView.IsMine) return;


        ChangeImage();
        //DecelerationReset();
    }

    private void Update()
    {
        if (!photonView.IsMine) return;


        LootAtMousePos();

        //if (Input.GetKey(KeyCode.W)) Acceleration();
        //if (Input.GetKey(KeyCode.E)) Deceleration();
        //if (Input.GetKeyUp(KeyCode.E)) DecelerationReset();

        if (Input.GetKey(KeyCode.W) && rb.velocity.z < maxSpeed) rb.AddForce(Vector3.forward * moveSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.S) && rb.velocity.z > -maxSpeed) rb.AddForce(Vector3.back * moveSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.A) && rb.velocity.x > -maxSpeed) rb.AddForce(Vector3.left * moveSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.D) && rb.velocity.x < maxSpeed) rb.AddForce(Vector3.right * moveSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.R))
        {
            // 깃발을 소유하고 있는 액터와 현재 조작중인 액터가 같은지 검사
            GameObject flagGo = GameObject.FindGameObjectWithTag("Flag");
            if (photonView.Owner.ActorNumber == flagGo.GetComponent<Flag>().OwnerActorNum)
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

    private void LootAtMousePos()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldToScreenPos = mainCam.WorldToScreenPoint(transform.position);
        Vector3 dir = (mousePos - worldToScreenPos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(-angle + 90f, Vector3.up);
    }

    //private void Acceleration()
    //{
    //    if (rb.velocity.magnitude < maxSpeed)
    //        rb.AddForce(transform.forward * moveSpeed * Time.deltaTime);
    //}

    //private void Deceleration()
    //{
    //    breakPower -= 0.1f * Time.deltaTime;
    //    if (breakPower < 0.01f) breakPower = 0.01f;
    //    rb.velocity *= breakPower;
    //}

    //private void DecelerationReset()
    //{
    //    breakPower = 1f;
    //}

    private void OnCollisionEnter(Collision _other)
    {
        if (!photonView.IsMine) return;


        // Drop Flag
        if (_other.gameObject.CompareTag("Player"))
        {
            // Single
            //FlagDrop();

            photonView.RPC("FlagDrop", RpcTarget.All);
            return;
        }        

        // Bounding
        if (_other.gameObject.CompareTag("Wall_U")) rb.AddForce(Vector3.back * (boundPower - Mathf.Abs(rb.velocity.z)));
        if (_other.gameObject.CompareTag("Wall_D")) rb.AddForce(Vector3.forward * (boundPower - Mathf.Abs(rb.velocity.z)));
        if (_other.gameObject.CompareTag("Wall_R")) rb.AddForce(Vector3.left * (boundPower - Mathf.Abs(rb.velocity.x)));
        if (_other.gameObject.CompareTag("Wall_L")) rb.AddForce(Vector3.right * (boundPower - Mathf.Abs(rb.velocity.x)));
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

    public void FlagCatch(Transform _flagTr)
    {
        _flagTr.SetParent(flagAttachPointTr, false);
        _flagTr.localPosition = Vector3.zero;
    }

    [PunRPC]
    public void FlagCatch(int _actorNum)
    {
        PhotonView pv = GetPhotonViewWithActorNumber(_actorNum);

        GameObject flagGo = GameObject.FindGameObjectWithTag("Flag");
        flagGo.GetComponent<Flag>().Attach(pv.transform, pv.Owner.ActorNumber);
    }

    [PunRPC]
    public void FlagDrop()
    {
        GameObject flagGo = GameObject.FindGameObjectWithTag("Flag");
        flagGo.GetComponent<Flag>().Detach();
    }

    private void ChangeImage()
    {
        Texture2D[] texList = Resources.LoadAll<Texture2D>("Textures\\Characters");
        GetComponentInChildren<MeshRenderer>().material.mainTexture = texList[Random.Range(0, texList.Length)];
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
