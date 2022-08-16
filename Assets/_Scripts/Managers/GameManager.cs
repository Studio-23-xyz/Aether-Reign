using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TurnSystem TurnSystemScript;
    public UIManager UIManagerScript;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [ContextMenu("Begin")]
    private void StartGame()
    {
        TurnSystemScript.SetupTurnList();
        UIManagerScript.EndTurnButton.onClick.AddListener(EndTurn);
    }

    public void RegisterUnit(GameObject unit)
    {
        TurnSystemScript.AddToUnitList(unit);
    }

    public void EndTurn()
    {
        TurnSystemScript.TurnEnd();
    }
}
