using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Map : MonoBehaviour
{
    [SerializeField]
    private PlayersList DefaultPlayersList; // Players list PRESET

    [SerializeField]
    public int PlayersAmount;

    [SerializeField]
    private MeshRenderer GlobalMeshRenderer;

    [SerializeField]
    private MeshFilter GlobalMeshFilter;

    [SerializeField]
    private int MAP_RADIUS_LIMIT = 40;
    
    public int UltimateMapRadius = 6;
    
    [SerializeField]
    private List<Cell> _cells;   //Cell is map elementary unit

    public List<Cell> Cells 
    {
        get { return _cells; }
        private set { _cells = value; }
    }

    public Dictionary<Vector2, Cell> WholeMap;

    [SerializeField]
    private float HighlightCellColorChange = 1.5f;

    public void CellHighlight(Cell cell, bool highlight) //both highlights and darkens cell if needed
    {
        if (highlight) cell.ColorFactor = cell.ColorFactor * HighlightCellColorChange;
        else cell.ColorFactor = cell.ColorFactor / HighlightCellColorChange;
    }


    public void CellHighlight(int? cell_idx, bool highlight)
    {
        if (cell_idx != null) 
        {
            var cell = Cells[(int)cell_idx];
            if(highlight)
                cell.ColorFactor = cell.ColorFactor * HighlightCellColorChange;
            else
                cell.ColorFactor = cell.ColorFactor / HighlightCellColorChange;
        }
        
    }

    //RecombineMesh must be launched every time anything changes on map visually
    public void RecombineMesh() //Combine Cells' meshes to bigger one;
    {
        int vertices_per_cell = Cells[0].MeshData.Mesh.vertices.Length;
        int tris_per_cell = Cells[0].MeshData.Mesh.triangles.Length;

        Vector3[] Vertices = new Vector3[Cells.Count * vertices_per_cell];
        Vector3[] Normals = new Vector3[Vertices.Length];
        Color[] Colors = new Color[Vertices.Length];
        int[] Tris = new int[Cells.Count * tris_per_cell];

        for (int i = 0; i < Cells.Count; i++) 
        {
            Cells[i].MeshData.Mesh.vertices.CopyTo(Vertices, vertices_per_cell * i);
            Cells[i].MeshData.Mesh.colors.CopyTo(Colors, vertices_per_cell * i);
            Cells[i].MeshData.Mesh.normals.CopyTo(Normals, vertices_per_cell * i);            
        }
        //Debug.Log(Cells[0].MeshData.Mesh.colors[0]);
        for (int i = 0; i < Cells.Count; i++)
        {
            AddNumberToArr(i * vertices_per_cell, Cells[i].MeshData.Mesh.triangles).CopyTo(Tris, tris_per_cell * i);
        }

        Mesh mesh = new Mesh();

        mesh.vertices = Vertices;
        mesh.colors = Colors;
        mesh.normals = Normals;
        mesh.triangles = Tris;

        GlobalMeshFilter.mesh = mesh;
    }

    [SerializeField]
    public int MinMapRadius, MaxMapRadius;

    [SerializeField]
    private float FillPercentage = 0.70f;

    public static int[] AddNumberToArr(int Number, int[] Arr)
    {
        int[] arr = new int[Arr.Length];
        for (int i = 0; i < Arr.Length; i++) 
        {
            arr[i] = Number + Arr[i];
        }

        return arr; 
    }

    public List<Player> CurrentPlayersList; //PlayersList for current party

    public Vector2 GenerateAndDrawMap() //single method to do all work, returns vector pointing to random created cell
    {
        UltimateMapRadius = (int)Mathf.Clamp(UltimateMapRadius, 0, MAP_RADIUS_LIMIT);

        CurrentPlayersList = new List<Player>();
        // last n elements of DefaultPlayers List:

        for (int i = 0; i < 2 + PlayersAmount; i++) //0 stays for Unclaimed ground, 2 -> water cells
        {
            CurrentPlayersList.Add((Player)DefaultPlayersList.DefaultPlayers[i]);
        }

        WholeMap = GenerateMap.GenerateCellsPositions(UltimateMapRadius, MinMapRadius, MaxMapRadius, FillPercentage,
            ref CurrentPlayersList);

        Cells = WholeMap.Values.ToList();        

        GivePlayersInitialTerritories();

        RecombineMesh();

        return Cells[0].Position;
    }

    private Cell ChooseCellAtTheEdge() // at least 1 neighbouring cell missing
    {
        int rand_idx = (int)Random.Range(0, Cells.Count);
        Cell ChosenCell = Cells[rand_idx];
        //Count non-water neighbours
        int counted_neighbours = 0;

        foreach (Cell Neighbour in ChosenCell.Neighbours) 
        {
            if (Neighbour.CellOwner == CurrentPlayersList[0]) counted_neighbours++;
        }

        if ( (ChosenCell.CellOwner == CurrentPlayersList[0] && counted_neighbours < MapConstants.CellSides &&
            counted_neighbours > 2) || (MapConstants.CellSides == 4) )
            return ChosenCell;
        else return ChooseCellAtTheEdge();
    }

    private Cell ChooseRandomCellOfPlayer(Player Player)
    {
        int rand_idx = (int)Random.Range(0, Player.OwnedCells.Count);
        return Player.OwnedCells[rand_idx];
    }

    private void GivePlayersInitialTerritories()
    {
        for (int i = 2; i < CurrentPlayersList.Count(); i++)
        {
            int cells_to_generate = UltimateMapRadius / 7 + 1; //bigger map - more initial territories

            int cells_generated = 0;

            //Give Player[i] the first cell

            while (cells_generated != 1) 
            {
                Cell ChosenCell = ChooseCellAtTheEdge();
                //Chosen cell is unclaimed ground OR water
                ChosenCell.CellOwner = CurrentPlayersList[i];
                cells_generated++;
            }

            int iteration = 0;
            while (cells_generated < cells_to_generate)
            {
                if (iteration > 500) break;
                Cell ChosenCell = ChooseRandomCellOfPlayer(CurrentPlayersList[i]);

                int rand_idx = (int)Random.Range(0, MapConstants.CellSides);
                Cell Neighbour = null;
                if(rand_idx < ChosenCell.Neighbours.Count) Neighbour = ChosenCell.Neighbours[rand_idx];
                if (Neighbour != null && Neighbour.CellOwner == CurrentPlayersList[0]) 
                {
                    Neighbour.CellOwner = CurrentPlayersList[i];
                    cells_generated++;
                }

                iteration++;
            }

            if(!(cells_generated < cells_to_generate)) 
            {
                Cell ChosenCell = ChooseCellAtTheEdge();
                //Chosen cell is unclaimed ground OR water
                ChosenCell.CellOwner = CurrentPlayersList[i];
                cells_generated++;
            }
        }
    }

}
