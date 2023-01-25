using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class UIScore : MonoBehaviourPun
{
    [SerializeField] private TextMeshProUGUI scoreRedText = null;
    [SerializeField] private TextMeshProUGUI scoreBlueText = null;

    public void UpdateScore(int _scoreRed, int _scoreBlue)
    {
        photonView.RPC("UpdateScoreRPC", RpcTarget.All, _scoreRed, _scoreBlue);
    }

    [PunRPC]
    public void UpdateScoreRPC(int _scoreRed, int _scoreBlue)
    {
        scoreRedText.text = _scoreRed.ToString();
        scoreBlueText.text = _scoreBlue.ToString();
    }
}
