using UnityEngine;

namespace _Scripts.Spells
{
    public class TeleportSpells : SpellBaseSO
    {
        public override void CastSpell(Vector3 castPos, Vector3 targetPos, SpellCastType type)
        {
            base.CastSpell(castPos, targetPos, type);
        }
    }
}
