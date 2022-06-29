using UnityEngine;

public interface ISpell
{
    string SpellName { get; set; }
    string Description { get; set; }

    public void AttemptToCastSpell();

    public void AimSpell();

    public void CastSpell();
}
