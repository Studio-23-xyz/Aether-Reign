using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class SpellBook
{
    public List<Spell> Spells;
}

[Serializable]
public class Spell
{
    public string SpellName;
    public int Mana;
    public int HP;
    public int Damage;
    public int Accuracy;
    public int TurnCooldown;
    public int Heal;
    public int Range;
    public string Effects;
}
