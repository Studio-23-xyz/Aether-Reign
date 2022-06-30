using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class Grimoire : MonoBehaviour
{
    public static Grimoire Instance { get; private set; }
    public SpellBook Darkhold;
    public TextAsset JsonData;

    private void Start()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
        }
        FetchData();
    }

    private void FetchData()
    {
        string jsonContent = JsonData.ToString();
        SpellBook spellList = JsonConvert.DeserializeObject<SpellBook>(jsonContent);
        Darkhold = spellList;
    }

    public List<Spell> GetSpells(int count)
    {
        List<Spell> randomSpells = new List<Spell>();
        for (int i = 0; i < count; i++)
        {
            int randomSpell = Random.Range(0, Darkhold.Spells.Count);
            randomSpells.Add(Darkhold.Spells[randomSpell]);
            Darkhold.Spells.RemoveAt(randomSpell);
        }
        return randomSpells;
    }
}