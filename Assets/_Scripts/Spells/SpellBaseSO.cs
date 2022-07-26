using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

namespace _Scripts.Spells
{
    [CreateAssetMenu(fileName = "SpellSO", menuName = "Spells/")]
    public class SpellBaseSO : ScriptableObject
    {
        #region Fields

        public string SpellName;
        [TextArea] public string SpellDescription;

        public int ManaCost;
        public int HealthCost;
        public int Damage;
        public int Heal;
        public int SpellRange;
        public int SpellCastPointAOE;
        public int CooldownTurns;
        public int Accuracy;
        public float SpellScale = 0.1f;
        public float VFXLength = 0f;
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
            _spellFXInstance = Instantiate(SpellFX, castPos, Quaternion.LookRotation(dir));
        }

        private void CheckForParticleSystems()
        {
            if (_spellFXInstance.GetComponentsInChildren<ParticleSystem>() != null)
            {
                var particleSys = _spellFXInstance.GetComponentsInChildren<ParticleSystem>();
                foreach (var system in particleSys)
                {
                    system.Play();
                }
            }
        }

        private void CheckForVFXParameters(float length)
        {
            if (_spellFXInstance.GetComponentInChildren<VisualEffect>())
            {
                var fxController = _spellFXInstance.GetComponentInChildren<VisualEffect>();
                if (VFXLength != 0)
                    fxController.SetFloat("VFXLength", length);
            }
        }
        
        public virtual void CastSpell(Vector3 castPos, Vector3 targetPos, SpellCastType type)
        {
            MazikaSystem.Instance.UseMana(ManaCost);
            if (type == SpellCastType.PointTarget)
            {
                PointTargetSpell(targetPos);
            }

            if (type == SpellCastType.Projectile)
            {
                ProjectileSpell(castPos, targetPos);
            }
            CheckForParticleSystems();
            var spellLength = Vector3.Distance(castPos, targetPos);
            CheckForVFXParameters(spellLength);
            _spellFXInstance.transform.localScale = Vector3.one * SpellScale;
            Destroy(_spellFXInstance, 5f);
            OnSpellCast?.Invoke();
        }
    }
}
