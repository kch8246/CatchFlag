using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    private SphereCollider sphereCollider = null;
    private Transform attachTr = null;
    private Jump jump = null;

    private readonly Vector3 offset = new Vector3(0f, 0f, 0.5f);
    private readonly float rotateSpeed = 5f;
    private readonly float rotateAngleLimit = 30f;

    private int ownerActorNum = -1;
    public int OwnerActorNum { get { return ownerActorNum; } }

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        jump = GetComponent<Jump>();
    }

    public void Respawn(Vector3 _spawnPos)
    {
        attachTr = null;
        ownerActorNum = -1;
        sphereCollider.enabled = true;

        transform.position = _spawnPos;
    }

    public bool IsAttach()
    {
        return attachTr != null;
    }

    public void Attach(Transform _attachTr, int _ownerActorNum)
    {
        attachTr = _attachTr;
        ownerActorNum = _ownerActorNum;
        sphereCollider.enabled = false;
    }

    public void Detach()
    {
        if (attachTr == null || ownerActorNum == -1) return;

        attachTr = null;
        ownerActorNum = -1;

        // TODO: 버려지는 위치 검사
        jump.JumpStart(transform.position, transform.position + new Vector3(1.5f, 0f, 0f), JumpDoneCallback);
    }

    private void JumpDoneCallback()
    {
        sphereCollider.enabled = true;
    }

    private void Update()
    {
        transform.rotation = Quaternion.AngleAxis(Mathf.Sin(Time.time * rotateSpeed) * rotateAngleLimit, Vector3.up);

        if (attachTr != null)
            transform.position = attachTr.position + offset;
    }
}
