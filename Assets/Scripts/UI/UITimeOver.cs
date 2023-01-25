using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class UITimeOver : MonoBehaviour
{
    private Button goToLobbyBtn = null;

    private void Awake()
    {
        goToLobbyBtn = GetComponentInChildren<Button>();
        goToLobbyBtn.onClick.AddListener(() => {
            StartCoroutine(LeaveCoroutine());
        });
    }

    public void SetVisible(bool _isVisible)
    {
        gameObject.SetActive(_isVisible);
    }

    private IEnumerator LeaveCoroutine()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
            yield return 0;
        SceneManager.LoadScene("LauncherScene");
    }
}
