using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TurnSystem TurnSystemScript;
    public UIManager UIManagerScript;
    public Spawner SpawnerScript;

    #region Grid Params

    public Vector2 GridDimensions = new Vector2(6, 6);
    public float GridSpacing = 1f;

    #endregion

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        FetchNecessaryComponents();
    }

    private void FetchNecessaryComponents()
    {
        TurnSystemScript = GetComponentInChildren<TurnSystem>();
        UIManagerScript = GetComponentInChildren<UIManager>();
        SpawnerScript = GetComponentInChildren<Spawner>();
    }

    void Start()
    {
        Invoke("SetupGame", 1f);
    }

    public async void SetupGame()
    {
        await GameGrid.Instance.InitiateGrid((int)GridDimensions.x, (int)GridDimensions.y, GridSpacing);
        await SpawnerScript.SpawnUnits();
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        StartGame();
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
