using UnityEngine;

namespace _Scripts.Spells
{
    public class BaseSpell : MonoBehaviour, ISpell
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
        [SerializeField] private Sprite _spellIconSprite;
        public int Accuracy;
        public float SpellScale = 0.1f;

        public GameObject SpellFX;

        #endregion

        #region Properties

        public int SpellRange => _spellRange;
        public int SpellCastPointAOE => _spellCastPointAOE;
        public int ManaCost => _manaCost;
        public int CooldownTurns => _cooldownTurns;
        public Sprite SpellIcon => _spellIconSprite;

        #endregion

        public void CastSpell(Vector3 castPosition, Vector3 targetPosition)
        {
            var dir = targetPosition - castPosition;
            var rot = Quaternion.LookRotation(dir);
            var fx = Instantiate(SpellFX, targetPosition, Quaternion.identity);
            fx.transform.localScale = Vector3.one * SpellScale;
            Destroy(fx, 2f);
        }
    }
}
