using System;
using UnityEngine;

public class MazikaSystem : MonoBehaviour
{
    public static MazikaSystem Instance { get; private set; }

    [SerializeField] private float _currentMana;
    [SerializeField] private float _maxMana;

    [SerializeField] private float _manaRegenRate;
    [SerializeField] private float _timeToRegenStart;
    private float _counter;

    public float CurrentMana => _currentMana;

    private void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        Initialize();
    }

    private void Initialize()
    {
        _currentMana = _maxMana;
    }

    private void Update()
    {
        if (_currentMana < _maxMana)
        {
            _counter += Time.deltaTime;
            if (_counter >= _timeToRegenStart)
            {
                _currentMana += _manaRegenRate * Time.deltaTime;
            }
        }
    }

    public bool HasEnoughMana(int spellCost)
    {
        if (spellCost > _currentMana)
            return false;
        return true;
    }

    public void UseMana(int manaToUse)
    {
        if (HasEnoughMana(manaToUse))
        {
            _currentMana -= manaToUse;
            _counter = 0f;
        }
    }
}