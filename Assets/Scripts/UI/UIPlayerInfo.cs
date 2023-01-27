using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPlayerInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nickNameText = null;
    [SerializeField] private UIHpBar hpBar = null;

    private RectTransform rt = null;

    private Transform targetTr = null;
    private readonly Vector3 offset = new Vector3(0f, 30f, 0f);

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    private void Update()
    {
        //if (targetTr == null) return;

        //Vector3 worldToScreen = Camera.main.WorldToScreenPoint(targetTr.position);
        //rt.position = worldToScreen + offset;
    }

    public void SetTarget(Transform _targetTr)
    {
        targetTr = _targetTr;
    }

    public void SetNickName(ETeam _team, string _nickName)
    {
        nickNameText.color = _team == ETeam.Red ? Color.red : Color.blue;
        nickNameText.text = _nickName;
    }

    public void UpdateHp(ETeam _team, int _hp, int _maxHp)
    {
        hpBar.UpdateHp(_team, _hp, _maxHp);
    }
}
