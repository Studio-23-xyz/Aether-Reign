using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class DataEntry : MonoBehaviour
{
    public List<Person> PersonList;

    public SpellBook Grimoire;
    public TextAsset JsonData;

    private void Start()
    {
        ShowData();
    }

    public void ShowData()
    {
        string jsonContent = JsonData.ToString();
        SpellBook A = JsonConvert.DeserializeObject<SpellBook>(jsonContent);
        Grimoire = A;
    }
}
