using Sirenix.OdinInspector;
using UnityEngine;

public class BaseSpell : SerializedMonoBehaviour, ISpell
{
    #region Fields

    public string SpellName;
    [TextArea] public string SpellDescription;

    public int ManaCost;
    public int HealthCost;
    public int Damage;
    public int Heal;
    [SerializeField] private int _spellRange;
    [SerializeField] private int _spellCastPointAOE;
    public int CooldownTurns;
    public int Accuracy;

    public GameObject SpellFX;

    #endregion

    #region Properties

    public int SpellRange => _spellRange;
    public int SpellCastPointAOE => _spellCastPointAOE;

    #endregion

    public void CastSpell(Vector3 castPosition, Vector3 targetPosition)
    {
        var dir = targetPosition - castPosition;
        var rot = Quaternion.LookRotation(dir);
        var fx = Instantiate(SpellFX, targetPosition, Quaternion.identity);
        fx.transform.localScale = Vector3.one * 0.3f;
        Destroy(fx, 2f);
    }
}
