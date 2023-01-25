using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private ETeam team;
    
    public ETeam Team { get { return team; } }
}
