using UnityEngine;

public interface ISpell
{
    public int SpellRange { get; }
    public int SpellCastPointAOE { get; }
    public void CastSpell(Vector3 castPosition, Vector3 targetPosition);
}
