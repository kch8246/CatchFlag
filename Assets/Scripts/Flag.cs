using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    private SphereCollider sphereCollider = null;
    private Transform attachTr = null;

    private Vector3 offset = new Vector3(0f, 0f, 0.5f);

    private Jump jump = null;

    private int ownerActorNum = -1;
    public int OwnerActorNum { get { return ownerActorNum; } }

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        jump = GetComponent<Jump>();
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
        if (attachTr == null) return;

        transform.position = attachTr.position + offset;
    }
}
