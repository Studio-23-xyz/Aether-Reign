using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _Scripts.Spells
{
    public class Grimoire : MonoBehaviour
    {
        public static Grimoire Instance { get; private set; }
        //public SpellBook Darkhold;
        //public TextAsset JsonData;

        public List<SpellHolder> AvailableSpells;
        private List<SpellHolder> _assignedSpells;
        public List<UISpellItem> UISpellItems;

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

        //private void FetchData()
        //{
        //    string jsonContent = JsonData.ToString();
        //    SpellBook spellList = JsonConvert.DeserializeObject<SpellBook>(jsonContent);
        //    Darkhold = spellList;
        //}

        private void Update()
        {
        
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
            CheckSpellUsability();
            return randomSpells;
        }

        private async void CheckSpellUsability()
        {
            while (true)
            {
                foreach (var spellItem in UISpellItems)
                {
                    if (MazikaSystem.Instance.HasEnoughMana(spellItem.SpellReference.Mezika.ManaCost))
                    {
                        spellItem.gameObject.GetComponentInChildren<Button>().interactable = true;
                    }
                    else 
                        spellItem.gameObject.GetComponentInChildren<Button>().interactable = false;
                }

                await UniTask.Delay(TimeSpan.FromSeconds(2.5f));
            }
        }

        public void UpdateUISpellBar(SpellHolder usedSpell)
        {
            foreach (var uiSpellItem in UISpellItems)
            {
                if (uiSpellItem.SpellReference == usedSpell)
                {
                    uiSpellItem.SetOverlayState(true);
                    uiSpellItem.OnCooldown = true;
                }
            }
        }

        public void UpdateUISpellBar()
        {
            foreach (var uiSpellItem in UISpellItems)
            {
                uiSpellItem.Cooldown();
            }
        }
    }
}