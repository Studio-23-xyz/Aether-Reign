using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Unit : MonoBehaviour
{
    private Animator _animator;
    private NavMeshAgent _agent;
    public LayerMask TileLayer;
    public bool IsMoving;
    public bool IsAimingSpell;
    public bool IsCastingSpell;
    public Vector3 TileOffset;

    public int SpellsToGet;


    public GameObject SpellBar;
    public GameObject UISpellItemPrefab;

    [SerializeField] private int _actionPoints;
    public List<Spell> UsableSpells = new List<Spell>();

    public List<SpellHolder> AvailableSpells = new List<SpellHolder>();
    [SerializeField] private SpellHolder _currentlySelectedSpell;

    public UnityEvent OnTurnCompleted;
    public UnityEvent<SpellHolder> OnSpellCasted;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        GetActableTiles(_actionPoints, isSpell: false);
        
        AvailableSpells = Grimoire.Instance.GetSpells(SpellsToGet);
        AddSpellsToUI();
    }

    private void AddSpellsToUI()
    {
        //foreach (var spell in SpellGrimoire)
        //{
        //    var spellItem = Instantiate(UISpellItemPrefab, SpellBar.transform);
        //    spellItem.GetComponent<UISpellItem>().SetupSpellUIItem(spell.SpellIcon, spell.CooldownTurns);
        //    spellItem.GetComponent<UISpellItem>().SetSpellAction(() =>
        //    {
        //        UseSpell(spell);
        //    });
        //}

        foreach (var availableSpell in AvailableSpells)
        {
            var spellUI = Instantiate(UISpellItemPrefab, SpellBar.transform);
            spellUI.GetComponent<UISpellItem>().SetupSpellUIItem(availableSpell.Mezika.SpellIconSprite, availableSpell.Mezika.CooldownTurns);
            spellUI.GetComponent<UISpellItem>().SetSpellAction(() =>
            {
                UseSpell(availableSpell);
            });
            Grimoire.Instance.UISpellItems.Add(spellUI.GetComponent<UISpellItem>());
        }
    }

    public void DebugFuction()
    {
        Debug.Log($"Hello world");
    }

    private void UseSpell(SpellHolder spell)
    {
        if (AttemptToCastSpell())
        {
            SetupSpell(spell);
        }
        else
        {
            IsAimingSpell = false;
            GameGrid.Instance.DisableWalkable();
            GetActableTiles(_actionPoints, isSpell: false);
        }
    }

    private void SetupSpell(SpellHolder spell)
    {
        _currentlySelectedSpell = spell;
        IsAimingSpell = true;
        GameGrid.Instance.DisableWalkable();
        GetActableTiles(_currentlySelectedSpell.Mezika.SpellRange, isSpell: true);
    }

    private void GetActableTiles(int tileRange, bool isSpell)
    {
        GameGrid.Instance.GetXZ(transform.position, out int unitX, out int unitZ);
        int lowX = unitX - _actionPoints;
        int lowZ = unitZ - _actionPoints;
        int highX = unitX + _actionPoints;
        int highZ = unitZ + _actionPoints;

        for (int x = lowX; x <= highX; x++)
        {
            for (int y = lowZ; y <= highZ; y++)
            {
                if (!GameGrid.Instance.IsWithinGrid(x,y))
                    continue;

                if (GameGrid.Instance.GetPath(new Vector3(unitX, 0f, unitZ), new Vector3(x, 0f, y)).Count <=
                    _actionPoints)
                {
                    var cell = GameGrid.Instance.GeneratedGrid[x, y].GetComponent<GridCell>();

                    if (!cell.IsOccupied)
                    {
                        NavMeshPath path = new NavMeshPath();
                        if (_agent.CalculatePath(cell.transform.position + TileOffset, path))
                        {
                            cell.SetTileVisibility(true);
                            cell.SetTileVisualColor(isSpell);
                            cell.IsWalkable = true;
                        }
                    }
                }
            }
        }
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 hitPos = GetMouseWorldPosition();
            if (hitPos == Vector3.zero)
                return;
            if (!IsMoving && !IsCastingSpell && !IsAimingSpell)
            {
                NavMeshPath targetPath = new NavMeshPath();
                _agent.CalculatePath(hitPos + TileOffset, targetPath);
                _agent.SetPath(targetPath);
                WaitForMoveFinish();
                //TileMovement(GameGrid.Instance.GetPath(transform.position, hitPos));
            }

            if (IsAimingSpell)
            {
                TurnTowardsSpellCast(hitPos+TileOffset);
                _currentlySelectedSpell.Mezika.CastSpell(transform.position, (hitPos), _currentlySelectedSpell.Mezika.SpellType);
                //SpellGrimoire[0].CastSpell(transform.position, hitPos + TileOffset);
                GameGrid.Instance.DisableWalkable();
                GetActableTiles(_actionPoints, false);
                IsAimingSpell = false;
                OnSpellCasted?.Invoke(_currentlySelectedSpell);
                _currentlySelectedSpell = null;
                
            }
            OnTurnCompleted?.Invoke();
        }
    }

    private bool AttemptToCastSpell()
    {
        if (IsMoving || IsCastingSpell || IsAimingSpell)
            return false;
        Debug.Log($"Attempt successful");
        return true;
    }

    private async void TurnTowardsSpellCast(Vector3 targetPoint)
    {
        var lineToCastPoint = targetPoint - transform.position;
        while (transform.rotation != Quaternion.LookRotation(lineToCastPoint))
        {
            Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lineToCastPoint), 0.1f);
            await UniTask.Yield();
            await UniTask.NextFrame();
        }
    }

    private async void WaitForMoveFinish()
    {
        IsMoving = true;
        _animator.SetBool("IsMoving", true);
        GameGrid.Instance.DisableWalkable();
        while (_agent.hasPath)
        {
            await UniTask.Yield();
            await UniTask.NextFrame();
        }

        IsMoving = false;
        _animator.SetBool("IsMoving", false);
        GetActableTiles(_actionPoints, isSpell: false);
    }

    private async void TileMovement(List<Vector3> path)
    {
        IsMoving = true;
        _animator.SetBool("IsMoving", true);
        foreach (var tilePos in path)
        {
            var direction = (tilePos + TileOffset) - transform.position;
            transform.rotation = Quaternion.LookRotation(direction);

            while (transform.position != tilePos + TileOffset)
            {
                transform.position = Vector3.MoveTowards(transform.position, tilePos + TileOffset, 0.1f);
                await UniTask.Delay(TimeSpan.FromSeconds(0.01f));
            }
            await UniTask.Yield();
            await UniTask.NextFrame();
        }
        IsMoving = false;
        _animator.SetBool("IsMoving", false);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(mouseRay, out var hitInfo, TileLayer))
        {
            if (hitInfo.collider.GetComponent<GridCell>())
            {
                if (hitInfo.collider.GetComponent<GridCell>().IsWalkable)
                    return hitInfo.transform.position;
                else
                {
                    Debug.LogWarning($"Tile not walkable");
                    return Vector3.zero;
                }
            }
        }

        Debug.LogWarning($"No tile hit");
        return Vector3.zero;
    }
}
