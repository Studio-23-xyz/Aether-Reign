using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TurnSystem TurnSystemScript;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void StartGame()
    {
        TurnSystemScript.SetupTurnList();
    }

    public void RegisterUnit(GameObject unit)
    {
        TurnSystemScript.AddToUnitList(unit);
    }
}
