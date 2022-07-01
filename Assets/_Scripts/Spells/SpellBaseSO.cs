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
    public int SpellRange;
    [SerializeField] private int _spellCastPointAOE;
    public int CooldownTurns;
    public int Accuracy;
    public float SpellScale = 0.1f;
    public SpellCastType SpellType;

    public Sprite SpellIconSprite;
    public GameObject SpellFX;

    private GameObject _spellFXInstance;

    public UnityEvent OnSpellCast;

    #endregion

    public virtual void PointTargetSpell(Vector3 targetPos)
    {
        _spellFXInstance = Instantiate(SpellFX, targetPos, Quaternion.identity);
    }

    public virtual void ProjectileSpell(Vector3 castPos, Vector3 targetPos)
    {
        var dir = targetPos - castPos;
        _spellFXInstance = Instantiate(SpellFX, dir, Quaternion.LookRotation(dir));
    }

    public virtual void CastSpell(Vector3 castPos, Vector3 targetPos, SpellCastType type)
    {
        if (type == SpellCastType.PointTarget)
        {
            PointTargetSpell(targetPos);
        }

        if (type == SpellCastType.Projectile)
        {
            ProjectileSpell(castPos, targetPos);
        }
        _spellFXInstance.transform.localScale = Vector3.one * SpellScale;
        Destroy(_spellFXInstance, 2.5f);
        OnSpellCast?.Invoke();
    }
}

public enum SpellCastType
{
    PointTarget,
    Projectile,
    AllyTarget,
    EnemyTarget,
    None
}
