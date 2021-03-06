using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Responsible for binding all game mechanics and leading gamble process
public class GameCoreManager : MonoBehaviour
{ 
    [SerializeField]
    private UIManagerScript ScreenController;

    [SerializeField]
    private Map Map;

    [SerializeField]
    private GameObject Canvas, StartScreen;

    private void Start() 
    {
        foreach (Transform child in Canvas.transform)
        {
            child.gameObject.SetActive(false);
        }

        StartScreen.SetActive(true);
    }

    private Cell CellClicked(Vector2 ClickPos) 
    {
        Cell ReturnCell = null;
        foreach (var cell in Map.Cells) 
        {
            if ((ClickPos - cell.Position).magnitude < GenerateMap.inscribed_radius) 
            {
                ReturnCell = cell;
                break;
            }
        }

        return ReturnCell;
    }

    private int? CellClickedIndex(Vector2 ClickPos)
    {
        int? idx = null;
        foreach (var cell in Map.Cells)
        {
            if ((ClickPos - cell.Position).magnitude < GenerateMap.inscribed_radius)
            {
                return idx;
            }
            idx++;
        }

        return idx;
    }

    IEnumerator MainCoroutine;

    private Player _whose_turn;
    public Player WhoseTurn 
    {
        get { return _whose_turn; }
        private set { _whose_turn = value; }
    }
    private int _turn_count;

    public int TurnCount
    {
        get { return _turn_count; }
        private set
        {
            _turn_count = value;
            int Players = Map.CurrentPlayersList.Count - 2;
            WhoseTurn = Map.CurrentPlayersList[value % Players + 2];
            ScreenController.ChangeTurnColor(WhoseTurn.Color);

            Debug.Log("It's " + WhoseTurn.Name + " turn, " + WhoseTurn.OwnedCells.Count.ToString() + " cells in posession");
        }
    }

    public void StartTurnCounter() 
    {
        TurnCount = 0;
    }

    void Awake()
    {
        MainCoroutine = GameCoreRoutine();        
    }

    public void SwitchGameRoutine(bool on) //switch on or switch off?
    {
        if (!on) { StopCoroutine(MainCoroutine); }
        else { StartCoroutine(MainCoroutine); }
    }

    IEnumerator GameCoreRoutine()
    {
        Vector2 TouchBeginPos = new Vector2(Mathf.Infinity, Mathf.Infinity );

        while (ScreenController.PartyRunning)
        {
            bool anything_done = false;
            if (Input.touchCount == 1)
            {
                Touch currentTouch = Input.GetTouch(0);

                if (currentTouch.phase == TouchPhase.Began) 
                {
                    TouchBeginPos = currentTouch.position;
                }

                if (currentTouch.phase == TouchPhase.Ended &&
                    ((Vector2)TouchBeginPos - currentTouch.position).magnitude < 1f) //finger moved on empirically small distance
                {
                    var ClickedCell = CellClicked(ClickedPos());
                    anything_done = true;

                    if (CellsHighlighted) //dehighlight
                    {
                        if (ClickedCell == null)
                            HighlightPlayerCells(WhoseTurn, false);
                        else if (WhoseTurn != ClickedCell.CellOwner)
                            HighlightPlayerCells(WhoseTurn, false);                        
                    }

                    if (ClickedCell != null) //highlight
                    {
                        if (WhoseTurn == ClickedCell.CellOwner)
                        {                            
                            HighlightAreaClicked++;
                            if (!CellsHighlighted) HighlightPlayerCells(WhoseTurn, true);
                        }
                    }
                    
                }
            }

            if(anything_done) Map.RecombineMesh();

            yield return null;
        }
    }

    public void PassTurn() 
    {
        TurnCount = TurnCount + 1;
    }
    [SerializeField]
    private bool CellsHighlighted = false;
    private List<Cell> HighlightedCells = new List<Cell>();
    [SerializeField]
    private int HighlightAreaClicked = 0;
    private void HighlightPlayerCells(Player player, bool on) // all Player player cells get highlighted, others are darkened
    {
        //on = true if highlight, on = false, if dehighlight
        CellsHighlighted = on;
        ScreenController.AppearUnitBuilder(on);
        if (on)
            foreach (var Cell in Map.Cells)
            {
                bool CellNeighbourBelongsToPlayer = false;

                foreach (var Neighbour in Cell.Neighbours)
                {
                    CellNeighbourBelongsToPlayer = CellNeighbourBelongsToPlayer || Neighbour.CellOwner == player;
                }

                if (Cell.CellOwner == player || CellNeighbourBelongsToPlayer)
                {
                    Map.CellHighlight(Cell, true);
                    HighlightedCells.Add(Cell);
                }
                else Map.CellHighlight(Cell, false);

                
            }
        else
        {
            foreach (var Cell in Map.Cells)
            {
                if(HighlightedCells.Contains(Cell))Map.CellHighlight(Cell, false);
                else Map.CellHighlight(Cell, true);
            }
            HighlightedCells = new List<Cell>();
            HighlightAreaClicked = 0;
        }

    }

    private Vector2 ClickedPos() 
    {       
        Vector3 v3 = (Vector3)Input.GetTouch(0).position;
        v3.z = 10f;
        v3 = Camera.main.ScreenToWorldPoint(v3);

        return (Vector2)v3;
    }

}
