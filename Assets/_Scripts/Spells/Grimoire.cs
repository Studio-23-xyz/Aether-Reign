using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class Grimoire : MonoBehaviour
{
    public static Grimoire Instance { get; private set; }
    public SpellBook Darkhold;
    public TextAsset JsonData;

    public List<SpellHolder> AvailableSpells;
    [SerializeField] private List<SpellHolder> _assignedSpells;
    public List<UISpellItem> UISpellItems;

    public GameObject SpellBar;

    private void Start()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
        }
        //FetchData();
    }

    private void FetchData()
    {
        string jsonContent = JsonData.ToString();
        SpellBook spellList = JsonConvert.DeserializeObject<SpellBook>(jsonContent);
        Darkhold = spellList;
    }

    public List<SpellHolder> GetSpells(int count)
    {
        List<SpellHolder> randomSpells = new List<SpellHolder>();
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, AvailableSpells.Count);
            randomSpells.Add(AvailableSpells[randomIndex]);
            AvailableSpells.RemoveAt(randomIndex);
        }

        _assignedSpells = randomSpells;
        return randomSpells;
    }

    public void UpdateUISpellBar(SpellHolder usedSpell)
    {
    }
}