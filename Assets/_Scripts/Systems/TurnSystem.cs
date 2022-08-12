using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    private List<GameObject> _playerList;
    private List<GameObject> _enemyList;
    public static TurnSystem Instance { get; private set; }

    private static int _turnNumber;

    public int TurnNumber => _turnNumber;

    public TextMeshProUGUI TurnCountUIText;

    private Queue<GameObject> _turnQueue = new Queue<GameObject>();
    [SerializeField] private GameObject _currentTurnGo = null;

    public delegate void TurnEvent();

    public TurnEvent OnTurnEnd;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        _turnNumber = 0;
        UpdateTurnCountInUI();
        
    }

    public void AddToUnitList(GameObject unit)
    {
        if (unit.CompareTag(StringResources.PlayerTag))
        {
            _playerList.Add(unit);
            Debug.Log($"Added to player list");
        }
        else if (unit.CompareTag(StringResources.EnemyTag))
        {
            _enemyList.Add(unit);
            Debug.Log($"Added to Enemy list");
        }
        else
            Debug.Log($"Failed to add to any list");
    }

    public void SetupTurnList()
    {
        foreach (var playerUnit in _playerList)
        {
            _turnQueue.Enqueue(playerUnit);
        }

        foreach (var enemyUnit in _enemyList)
        {
            _turnQueue.Enqueue(enemyUnit);
        }

        _currentTurnGo = _turnQueue.Peek();
    }

    public void TurnEnd()
    {
        _turnQueue.Enqueue(_turnQueue.Dequeue());
        OnTurnEnd?.Invoke();
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
