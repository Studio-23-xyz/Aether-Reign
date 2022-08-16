using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    private List<GameObject> _playerList = new List<GameObject>();
    private List<GameObject> _enemyList = new List<GameObject>();
    public static TurnSystem Instance { get; private set; }

    public List<GameObject> PlayerList => _playerList;
    public List<GameObject> EnemyList => _enemyList;

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
        StartTurnRotation();
    }

    private void StartTurnRotation()
    {
        _currentTurnGo?.GetComponent<ITurnUnit>().BeginTurn();
    }

    public void TurnEnd()
    {
        _currentTurnGo?.GetComponent<ITurnUnit>().EndTurn();
        _turnQueue.Enqueue(_turnQueue.Dequeue());
        _currentTurnGo = _turnQueue.Peek();
        _currentTurnGo.GetComponent<ITurnUnit>().BeginTurn();
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
