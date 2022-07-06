using UnityEngine;

namespace _Scripts.Spells
{
    public interface ISpell
    {
        public int SpellRange { get; }
        public int SpellCastPointAOE { get; }
        public int ManaCost { get; }
        public int CooldownTurns { get; }
        public Sprite SpellIcon { get; }
        public void CastSpell(Vector3 castPosition, Vector3 targetPosition);
    }
}
