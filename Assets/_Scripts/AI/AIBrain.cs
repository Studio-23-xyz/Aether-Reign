using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public abstract class AIBrain : MonoBehaviour, ITurnUnit, IDamageable
{
    public AIUnit AIUnit;

    #region Params

    private string _unitName;
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

    #region Interface Params

    public float Health
    {
        get => _hp;
        set => _hp = value;
    }

    public void RegisterDamage(float damageAmount)
    {
        _hp = _hp - damageAmount;
        if (_hp <= 0f)
            _hp = 0f;
    }

    #endregion

    [SerializeField] private Transform _projectileInstantiationPoint;
    public GameObject ProjectilePrefab;
    private Animator _animator;

    //Todo make state controller
    private AIState _currentState;

    private NavMeshAgent _agent;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        //_animator = GetComponent<Animator>();
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
    public async virtual UniTask Move()
    {
        if (_isActing)
        {
            Debug.Log("Action in progress");
            return;
        }
        var actableTiles = GameGrid.Instance.GetActableTiles(_actionPoints, false, transform, false);
        var newPath = new NavMeshPath();

        var randomlySelectedTile = actableTiles[Random.Range(0, actableTiles.Count)];

        _agent.CalculatePath(randomlySelectedTile.transform.position + new Vector3(0f, 0.5f, 0.5f), newPath);
        _agent.SetPath(newPath);

        await WaitTillMovementIsComplete();
        _agent.ResetPath();
    }

    private async UniTask WaitTillMovementIsComplete()
    {
        _isActing = true;
        //_animator.SetBool("IsMoving", true);
        while (_agent.hasPath)
        {
            await UniTask.Yield();
            await UniTask.NextFrame();
        }
        //_animator.SetBool("IsMoving", false);
        _isActing = false;
    }

    public void BeginTurn()
    {
        DecideAction();
    }

    private async void DecideAction()
    {
        var randomPlayerUnit = Random.Range(0, GameManager.Instance.TurnSystemScript.PlayerList.Count);
        var playerUnit = GameManager.Instance.TurnSystemScript.PlayerList[randomPlayerUnit].transform;
        GameGrid.Instance.GetXZ(transform.position, out var unitX, out var unitZ);
        if (GameGrid.Instance.GetPath(new Vector3(unitX, 0f, unitZ),
                new Vector3(playerUnit.position.x, 0f, playerUnit.position.z)).Count <= _actionPoints)
        {
            await AttackTarget(playerUnit);
            return;
        }
        else
            await Move();
        GameManager.Instance.EndTurn();
    }

    private async UniTask AttackTarget(Transform playerUnit)
    {
        Vector3 directionToPlayer = playerUnit.position - transform.position;
        await RotateTowardsPlayer(directionToPlayer);
        Instantiate(ProjectilePrefab, _projectileInstantiationPoint.position, Quaternion.LookRotation(directionToPlayer, Vector3.up));
    }

    private async UniTask RotateTowardsPlayer(Vector3 playerDirection)
    {
        transform.rotation = Quaternion.LookRotation(playerDirection);
    }

    public void EndTurn()
    {

    }
}
