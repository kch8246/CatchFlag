using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFlag : MonoBehaviour
{
    [SerializeField] private GameObject flagGo = null;

    public void Respawn()
    {
        if (flagGo == null) return;

        flagGo.transform.position = transform.position;
    }
}
