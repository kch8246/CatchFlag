using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character2D : MonoBehaviour
{
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
        SetDirection(EDirection.Down);
    }

    //public void SetRamdomCharTexture()
    //{
    //    Texture2D[] texList = Resources.LoadAll<Texture2D>("Textures\\Characters");
    //    SetCharTexture(texList[Random.Range(0, texList.Length)]);
    //}

    //public void SetCharTexture(Texture2D _charTex)
    //{
    //    mr.material.mainTexture = _charTex;
    //}

    public void SetCharTexture(string _charTexName)
    {
        mr.material.mainTexture = Resources.Load<Texture2D>("Textures\\Characters\\" + _charTexName);
    }

    public void SetDirection(EDirection _dir)
    {
        mr.material.SetTextureScale("_MainTex", texScale);

        if (_dir == EDirection.Up) 
            mr.material.SetTextureOffset("_MainTex", texOffsetUp);
        else if (_dir == EDirection.Right)
        {
            // 오른쪽은 왼쪽 이미지를 뒤집은 것이기 때문에 따로 처리
            mr.material.SetTextureScale("_MainTex", texScaleFlipX);
            mr.material.SetTextureOffset("_MainTex", texOffsetRight);
        }
        else if (_dir == EDirection.Down)
            mr.material.SetTextureOffset("_MainTex", texOffsetDown);
        else if (_dir == EDirection.Left)
            mr.material.SetTextureOffset("_MainTex", texOffsetLeft);
    }
}
