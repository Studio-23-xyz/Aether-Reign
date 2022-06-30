using UnityEngine;

public abstract class SpellHolder : MonoBehaviour
{
    public SpellBaseSO Mezika;

    public abstract void CastSpell(Vector3 targetPoint);

    void Start()
    {
        Mezika.Hello();
    }
}
