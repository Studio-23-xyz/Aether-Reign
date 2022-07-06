using UnityEngine;
using UnityEngine.UI;

namespace AetherReign._Scripts.Unit
{
    public class UnitUIHandler : MonoBehaviour
    {
        public Image ManaCircle;
        public Image HealthCircle;

        private void Update()
        {
            ManaCircle.fillAmount = MazikaSystem.Instance.CurrentMana / 100f;
        }
    }
}