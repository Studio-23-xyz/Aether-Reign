using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    public static TurnSystem Instance { get; private set; }

    private int _turnNumber;

    private void Awake()
    {
        Instance = this;
        _turnNumber = 0;
    }

    private void NextTurn()
    {

    }
}
