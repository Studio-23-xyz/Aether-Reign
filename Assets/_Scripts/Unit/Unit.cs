using System;
using System.Collections.Generic;
using _Scripts.Spells;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace AetherReign._Scripts.Unit
{
    public class Unit : MonoBehaviour
    {
        private Animator _animator;
        private NavMeshAgent _agent;
        public LayerMask TileLayer;
        private bool _canAct;
        public bool IsMoving;
        public bool IsAimingSpell;
        public bool IsCastingSpell;
        public Vector3 TileOffset;

        public int SpellsToGet;

        public GameObject MoveClickMarker;
        public GameObject CastMarker;

        public GameObject SpellBar;
        public GameObject UISpellItemPrefab;

        [SerializeField] private int _actionPoints;

        public List<SpellHolder> AvailableSpells = new();
        [SerializeField] private SpellHolder _currentlySelectedSpell;

        public UnityEvent OnTurnCompleted;
        public UnityEvent<SpellHolder> OnSpellCasted;

        [SerializeField] private GameObject _lastTargetedTile;
        [SerializeField] private List<GameObject> _tilesWithinSelectedSpellRange;
        [SerializeField] private Transform _spellCastPoint;

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            _animator = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();
            AvailableSpells = Grimoire.Instance.GetSpells(SpellsToGet);
            AddSpellsToUI();

            GameManager.Instance.RegisterUnit(gameObject);
        }

        public void StartTurn()
        {
            _canAct = true;
            GameGrid.Instance.GetActableTiles(_actionPoints, false, transform);
            SpellBar.SetActive(true);
        }

        public void EndTurn()
        {
            _canAct = false;
            GameGrid.Instance.DisableWalkable();
            GameGrid.Instance.DisableSpellAoEVisuals();
            SpellBar.SetActive(false);
        }

        private void AddSpellsToUI()
        {
            foreach (var availableSpell in AvailableSpells)
            {
                var spellUI = Instantiate(UISpellItemPrefab, SpellBar.transform);
                spellUI.GetComponent<UISpellItem>().SetupSpellUIItem(availableSpell);
                spellUI.GetComponent<UISpellItem>().SetSpellAction(() => { UseSpell(availableSpell); });
                Grimoire.Instance.UISpellItems.Add(spellUI.GetComponent<UISpellItem>());
            }
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
                GameGrid.Instance.GetActableTiles(_actionPoints, false, transform);
            }
        }

        private void SetupSpell(SpellHolder spell)
        {
            if (!MazikaSystem.Instance.HasEnoughMana(spell.Mezika.ManaCost))
                return;
            _currentlySelectedSpell = spell;
            IsAimingSpell = true;
            GameGrid.Instance.DisableWalkable();
            _tilesWithinSelectedSpellRange = GameGrid.Instance.GetActableTiles(_currentlySelectedSpell.Mezika.SpellRange, true, transform);
        }

        private async void Update()
        {
            if (!_canAct)
                return;
            if (Input.GetMouseButtonDown(0))
            {
                var hitPos = GetMouseWorldPosition();
                if (hitPos == Vector3.zero)
                    return;
                if (!IsMoving && !IsCastingSpell && !IsAimingSpell)
                {
                    var targetPath = new NavMeshPath();
                    _agent.CalculatePath(hitPos + TileOffset, targetPath);

                    _agent.SetPath(targetPath);

                    var marker = Instantiate(MoveClickMarker, hitPos + TileOffset, Quaternion.identity);
                    marker.transform.localScale = Vector3.one * 0.4f;
                    Destroy(marker, 2f);

                    await WaitForMoveFinish();
                }

                if (IsAimingSpell)
                {
                    //await TurnTowardsSpellCast(hitPos + TileOffset);
                    _currentlySelectedSpell.Mezika.CastSpell(_spellCastPoint.position, hitPos + TileOffset + new Vector3(0f, 0.1f),
                        _currentlySelectedSpell.Mezika.SpellType);

                    var marker = Instantiate(CastMarker, hitPos + TileOffset, Quaternion.identity);
                    marker.transform.localScale = Vector3.one * 0.4f;
                    Destroy(marker, 2f);

                    GameGrid.Instance.DisableWalkable();
                    GameGrid.Instance.GetActableTiles(_actionPoints, false, transform);
                    IsAimingSpell = false;
                    OnSpellCasted?.Invoke(_currentlySelectedSpell);
                    _currentlySelectedSpell = null;
                }

                OnTurnCompleted?.Invoke();
            }

            if (IsAimingSpell)
            {
                var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(mouseRay, out var hitInfo, TileLayer))
                {
                    if (_tilesWithinSelectedSpellRange.Contains(hitInfo.collider.gameObject))
                    {
                        if (hitInfo.collider.gameObject != _lastTargetedTile && _lastTargetedTile != null)
                        { 
                            _lastTargetedTile.GetComponent<GridCell>().SpellAoEIsActive = false; 
                            GameGrid.Instance.DisableSpellAoEVisuals(); 
                            _lastTargetedTile = null;
                        }
                        var tile = hitInfo.collider.GetComponent<GridCell>();

                        if (tile.SpellAoEIsActive == false)
                        {
                            _lastTargetedTile = hitInfo.collider.gameObject;
                            tile.SpellAoEIsActive = true;
                            var spellAoETiles =
                                GameGrid.Instance.GetSpellAoETiles(_currentlySelectedSpell.Mezika.SpellCastPointAOE,
                                    hitInfo.transform);
                            foreach (var spellAoETile in spellAoETiles)
                            {
                                spellAoETile.GetComponent<GridCell>().SpellAoEVisual.SetActive(true);
                            }
                        }
                    }
                    else if (_lastTargetedTile != null)
                    {
                        if (hitInfo.collider.gameObject != _lastTargetedTile)
                        { 
                            _lastTargetedTile.GetComponent<GridCell>().SpellAoEIsActive = false; 
                            GameGrid.Instance.DisableSpellAoEVisuals(); 
                            _lastTargetedTile = null;
                        }
                    }
                }
            }
        }

        private bool AttemptToCastSpell()
        {
            if (IsMoving || IsCastingSpell || IsAimingSpell)
                return false;
            return true;
        }

        private async UniTask TurnTowardsSpellCast(Vector3 targetPoint)
        {
            var lineToCastPoint = targetPoint - transform.position + TileOffset;
            while (transform.rotation != Quaternion.LookRotation(lineToCastPoint))
            {
                Debug.Log("heheh");
                Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lineToCastPoint), 0.1f);
                await UniTask.Yield();
                await UniTask.NextFrame();
            }
        }

        private async UniTask WaitForMoveFinish()
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
            GameGrid.Instance.GetActableTiles(_actionPoints, false, transform);
        }

        private Vector3 GetMouseWorldPosition()
        {
            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(mouseRay, out var hitInfo, TileLayer))
                if (hitInfo.collider.GetComponent<GridCell>())
                {
                    if (hitInfo.collider.GetComponent<GridCell>().IsWalkable)
                    {
                        return hitInfo.transform.position;
                    }

                    Debug.LogWarning("Tile not walkable");
                    return Vector3.zero;
                }

            Debug.LogWarning("No tile hit");
            return Vector3.zero;
        }
    }
}
