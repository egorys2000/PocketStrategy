using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
//Game properties of Cell
public class Cell
{
    private Player _cell_owner;

    public MeshCell MeshData;

    private float _color_factor = 1f;

    public float ColorFactor 
    {
        get { return _color_factor; } 
        set 
        {
            _color_factor = value;
            this.MeshData.BuildCellMesh(this.Position, this.CellOwner, value); //Rebuild Cell's Mesh with new colors
        }
    }

    //Neighbouring cells starting from 3 o'clock and counter-clockwise
    [System.NonSerialized]
    public List<Cell> Neighbours;

    public Player CellOwner
    {
        
        get => _cell_owner;
        set
        {
            if(_cell_owner != null) _cell_owner.OwnedCells.Remove(this);
            value.OwnedCells.Add(this);
            _cell_owner = value;
            this.MeshData.BuildCellMesh(this.Position, value, ColorFactor); //Rebuild Cell's Mesh with new colors
        }
    }

    [SerializeField]
    private MapToken _cellContent;
    public MapToken CellContent 
    {
        get => _cellContent;
        set 
        {
            CellContent = value;
        }
    }

    //[SerializeField]
    public readonly Vector2 Position;

    public Cell(Vector2 Pos, Player Owner) 
    {
        this.MeshData = new MeshCell(Pos, Owner);
        this.Position = Pos;
        this.CellOwner = Owner;

        this.Neighbours = new List<Cell>();
    }
}