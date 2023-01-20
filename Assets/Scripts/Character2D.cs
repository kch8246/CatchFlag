using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character2D : MonoBehaviour
{
    public enum Direction { Up, Right, Down, Left }

    private MeshRenderer mr = null;

    private readonly Vector2 texScale = new Vector2(0.5f, 0.5f);
    private readonly Vector2 texScaleFlipX = new Vector2(-0.5f, 0.5f);

    private readonly Vector2 texOffsetUp = new Vector2(0.5f, 0.5f);
    private readonly Vector2 texOffsetRight = new Vector2(0.5f, 0.0f);
    private readonly Vector2 texOffsetDown = new Vector2(0.0f, 0.5f);
    private readonly Vector2 texOffsetLeft = new Vector2(0.0f, 0.0f);

    private void Awake()
    {
        mr = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        SetDirection(Direction.Down);
    }

    public void SetCharTexture(Texture2D _charTex)
    {
        mr.material.mainTexture = _charTex;
    }

    public void SetDirection(Direction _dir)
    {
        mr.material.SetTextureScale("_MainTex", texScale);

        if (_dir == Direction.Up) 
            mr.material.SetTextureOffset("_MainTex", texOffsetUp);
        else if (_dir == Direction.Right)
        {
            // 오른쪽은 왼쪽 이미지를 뒤집은 것이기 때문에 따로 처리
            mr.material.SetTextureScale("_MainTex", texScaleFlipX);
            mr.material.SetTextureOffset("_MainTex", texOffsetRight);
        }
        else if (_dir == Direction.Down)
            mr.material.SetTextureOffset("_MainTex", texOffsetDown);
        else if (_dir == Direction.Left)
            mr.material.SetTextureOffset("_MainTex", texOffsetLeft);
    }
}
