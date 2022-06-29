using UnityEngine;

public class BaseSpell : MonoBehaviour, ISpell
{
    //public string SpellName;
    //[TextArea] public string SpellDescription;

    public int ManaCost;
    public int HealthCost;
    public int Damage;
    public int Heal;
    public int SpellRange;
    public int SpellCastPointAOE;
    public int CooldownTurns;
    public int Accuracy;

    public string SpellName { get; set; }
    public string Description { get; set; }

    public void AttemptToCastSpell()
    {
        throw new System.NotImplementedException();
    }

    public void AimSpell()
    {
        throw new System.NotImplementedException();
    }

    public void CastSpell()
    {
        throw new System.NotImplementedException();
    }
}
