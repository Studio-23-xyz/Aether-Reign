using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SpellSO")]
public class SpellBaseSO : ScriptableObject
{
    #region Fields

    public string SpellName;
    [TextArea] public string SpellDescription;

    [SerializeField] private int _manaCost;
    public int HealthCost;
    public int Damage;
    public int Heal;
    [SerializeField] private int _spellRange;
    [SerializeField] private int _spellCastPointAOE;
    [SerializeField] private int _cooldownTurns;
    public int Accuracy;
    public float SpellScale = 0.1f;

    [SerializeField] private Sprite _spellIconSprite;
    public GameObject SpellFX;

    public UnityEvent OnCast;

    #endregion

    public void Hello()
    {
        OnCast.Invoke();
    }
}
