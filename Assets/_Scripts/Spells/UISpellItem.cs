using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISpellItem : MonoBehaviour
{
    [SerializeField] private Image _spellIcon;
    [SerializeField] private int _originalCooldown;
    [HideInInspector] public int CooldownTurns;
    [SerializeField] private GameObject _cooldownOverlay;
    [SerializeField] private Button _spellButton;
    public TextMeshProUGUI CooldownText;

    public bool OnCooldown;

    [HideInInspector] public SpellHolder SpellReference;

    public void SetSpellAction(Action spellAction)
    {
        _spellButton.onClick.AddListener(() =>
        {
            spellAction?.Invoke();
        });
    }

    public void SetupSpellUIItem(SpellHolder spell)
    {
        _spellIcon.sprite = spell.Mezika.SpellIconSprite;
        _originalCooldown = spell.Mezika.CooldownTurns;
        CooldownTurns = _originalCooldown;
        CooldownText.text = $"{_originalCooldown}";
        SpellReference = spell;
    }

    public void SetOverlayState(bool state)
    {
        _cooldownOverlay.SetActive(state);
    }

    public void Cooldown()
    {
        if (!OnCooldown)
            return;

        CooldownTurns--;
        if (CooldownTurns == 0)
        {
            OnCooldown = false;
            SetOverlayState(false);
            CooldownTurns = _originalCooldown;
            CooldownText.text = $"{_originalCooldown}";
        }
        CooldownText.text = $"{CooldownTurns}";
    }
}