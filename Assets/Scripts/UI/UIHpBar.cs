using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHpBar : MonoBehaviour
{
    private Image front = null;
    private RectTransform frontRt = null;
    private Vector2 oriSize = Vector2.zero;

    private void Awake()
    {
        front = GetComponentsInChildren<Image>()[1];
        frontRt = front.GetComponent<RectTransform>();
        oriSize = frontRt.sizeDelta;
    }

    public void UpdateHp(ETeam _team, int _hp, int _maxHp)
    {
        front.color = _team == ETeam.Red ? Color.red : Color.blue;

        float ratio = (float)_hp / _maxHp;
        frontRt.sizeDelta = new Vector2(oriSize.x * ratio, oriSize.y);
    }
}
