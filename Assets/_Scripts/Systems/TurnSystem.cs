using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    public static TurnSystem Instance { get; private set; }

    private static int _turnNumber;

    public int TurnNumber => _turnNumber;

    public TextMeshProUGUI TurnCountUIText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        _turnNumber = 0;
        UpdateTurnCountInUI();
    }

    public void NextTurn()
    {
        _turnNumber++;
        UpdateTurnCountInUI();
    }

    private void UpdateTurnCountInUI()
    {
        TurnCountUIText.text = $"Turn: {_turnNumber}";
    }
}
