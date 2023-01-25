using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class UIPlayButton : MonoBehaviourPun
{
    private Button btn = null;
    private TextMeshProUGUI text = null;

    private void Awake()
    {
        btn = GetComponentInChildren<Button>();
        btn.interactable = false;
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetTextWaiting()
    {
        text.text = "Waiting..";
    }

    public void SetTextReady()
    {
        text.text = "Ready..";
    }

    public void SetTextPlay()
    {
        text.text = "Click\nTo\nPlay!";
        btn.interactable = true;
    }

    public void SetVisible(bool _isVisible)
    {
        gameObject.SetActive(_isVisible);
    }

    public void SetPressedCallback(UnityEngine.Events.UnityAction _call)
    {
        btn.onClick.AddListener(_call);
    }
}
