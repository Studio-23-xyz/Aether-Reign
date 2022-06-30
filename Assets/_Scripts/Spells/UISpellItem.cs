using System;
using UnityEngine;
using UnityEngine.UI;

public class UISpellItem : MonoBehaviour
{
    [SerializeField] private Image _spellIcon;
    [SerializeField] private int _cooldownTurns;
    [SerializeField] private GameObject _cooldownOverlay;
    [SerializeField] private Button _spellButton;

    public void SetSpellAction(Action spellAction)
    {
        _spellButton.onClick.AddListener(() =>
        {
            spellAction?.Invoke();
        });
    }

    public void SetupSpellUIItem(Sprite icon, int cooldownTurn)
    {
        _spellIcon.sprite = icon;
        _cooldownTurns = cooldownTurn;
    }

    public void SetOverlayState(bool state)
    {
        _cooldownOverlay.SetActive(state);
    }
}