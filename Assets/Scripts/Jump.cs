using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    private bool isJumping = false;
    public bool IsJumping { get { return isJumping; } }

    private Vector3 startPos = Vector3.zero;
    private Vector3 endPos = Vector3.zero;
    private Vector3 curPos = Vector3.zero;

    private float velocityX = 0f;
    private float velocityY = 0f;
    private float velocityZ = 0f;

    private float g = 0f;
    private float endTime = 0f;
    private float maxHeight = 1f;
    private float height = 0f;
    private float endHeight = 0f;
    private float maxTime = 0.1f;
    private float time = 0f;

    private VoidVoidDelegate finishCallback = null;
    private Coroutine jumpCoroutine;

    private void CalcJumpInfo()
    {
        endHeight = endPos.y - startPos.y;
        height = maxHeight - startPos.y;

        g = 2f * height / (maxTime * maxTime);

        velocityY = Mathf.Sqrt(2f * g * height);

        float tmpVelocityY = -2f * velocityY;
        float tmpHeight = 2f * endHeight;

        endTime = (-tmpVelocityY + Mathf.Sqrt(tmpVelocityY * tmpVelocityY - 4f * g * tmpHeight)) / (2f * g);

        velocityX = -(startPos.x - endPos.x) / endTime;
        velocityZ = -(startPos.z - endPos.z) / endTime;
    }

    public void JumpStart(Vector3 _startPos, Vector3 _endPos, VoidVoidDelegate _finishCallback = null)
    {
        if (isJumping) return;
        isJumping = true;

        startPos = _startPos;
        endPos = _endPos;
        CalcJumpInfo();

        finishCallback = _finishCallback;

        jumpCoroutine = StartCoroutine(JumpCoroutine());
    }

    public void StopJump()
    {
        if (jumpCoroutine != null)
        {
            isJumping = false;
            StopCoroutine(jumpCoroutine);
        }
    }

    public void JumpSpeed(float _speed)
    {
        maxTime = _speed;
    }

    private IEnumerator JumpCoroutine()
    {
        isJumping = true;
        transform.localPosition = startPos;

        time = 0f;
        while (time < endTime)
        {
            time += Time.deltaTime;

            curPos.x = startPos.x + velocityX * time;
            curPos.y = startPos.y + (velocityY * time) - (0.5f * g * time * time);
            curPos.z = startPos.z + velocityZ * time;
            transform.localPosition = curPos;

            yield return null;
        }

        transform.localPosition = endPos;
        isJumping = false;

        finishCallback?.Invoke();
    }
}
