using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITimer : MonoBehaviour
{
    private TextMeshProUGUI timerText = null;

    private void Awake()
    {
        timerText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void UpdateTimer(int _min, int _sec)
    {
        timerText.text = string.Format("{0}:{1}", _min.ToString("D2"), _sec.ToString("D2"));
    }
}
