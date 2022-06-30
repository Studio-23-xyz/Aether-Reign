using UnityEngine;

public class Ember : SpellHolder
{
    public override void CastSpell(Vector3 targetPoint)
    {
        {
            var spellFx = Instantiate(Mezika.SpellFX, targetPoint, Quaternion.identity);
            Destroy(spellFx, 3f);
        }
    }
}
