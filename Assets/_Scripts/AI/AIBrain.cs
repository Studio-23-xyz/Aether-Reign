using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public abstract class AIBrain : MonoBehaviour
{
    public AIUnit AIUnit;

    #region Params

    public string _unitName;
    private float _hp;
    private float _maxHp;
    private float _mp;
    private float _maxMp;
    private float _atkPower;
    private float _def;
    private float _dodge;
    private int _actionPoints;

    #endregion

    #region Functional Booleans

    private bool _isActing;

    #endregion

    //Todo make state controller
    private AIState _currentState;

    private NavMeshAgent _agent;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        SetupUnit(AIUnit);
        GameManager.Instance.RegisterUnit(gameObject);
    }

    private void SetupUnit(AIUnit aiUnit)
    {
        _unitName = aiUnit.UnitName;
        _maxHp = aiUnit.MaxHp;
        _hp = _maxHp;
        _maxMp = aiUnit.MaxMp;
        _mp = _maxMp;
        _actionPoints = aiUnit.ActionPoints;
        _atkPower = aiUnit.Stats.AttackStat;
        _def = aiUnit.Stats.DefenseStat;
        _dodge = aiUnit.Stats.DodgeStat;

        gameObject.tag = StringResources.EnemyTag;
    }

    public void SetState(AIState newState)
    {
        _currentState = newState;
    }

    [ContextMenu("Move")]
    public async virtual void Move()
    {
        if (_isActing)
        {
            Debug.Log("Action in progress");
            return;
        }
        var actableTiles = GameGrid.Instance.GetActableTiles(_actionPoints, false, transform, false);
        var newPath = new NavMeshPath();

        var randomlySelectedTile = actableTiles[Random.Range(0, actableTiles.Count)];

        _agent.CalculatePath(randomlySelectedTile.transform.position + new Vector3(0f, 0.5f, 0f), newPath);
        _agent.SetPath(newPath);

        await WaitTillMovementIsComplete();
    }

    private async UniTask WaitTillMovementIsComplete()
    {
        _isActing = true;
        while (_agent.hasPath)
        {
            await UniTask.Yield();
            await UniTask.NextFrame();
        }
        _isActing = false;
    }
}
