using UnityEngine;

public class MazikaSystem : MonoBehaviour
{
    public static MazikaSystem Instance { get; private set; }

    [SerializeField] private float _currentMana;
    [SerializeField] private float _maxMana;

    [SerializeField] private float _manaRegenAmount;
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

    public void RegenMana()
    {
        if (_currentMana < _maxMana)
        {
            _currentMana += _manaRegenAmount;
            if (_currentMana >= _maxMana)
                _currentMana = _maxMana;
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