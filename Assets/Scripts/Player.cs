using UnityEngine;
using System.Collections.Generic;

public class Player
{
    public /*readonly*/ Color Color;
    public /*readonly*/ string Name;

    public Player(Color color, string name) 
    {
        this.Color = color;
        this.Name = name;
        this.OwnedCells = new List<Cell>();
    }

    [SerializeField]
    private int _balance = 0;

    public int Balance 
    {
        get { return _balance; }
        private set
        {
            _balance = value;
        }
    }

    public class ConnectivityComponent //Player may have many disjointed territories that have different balance and income
    {
        public int Balance;
        public int Income;
    }

    public List<Cell> OwnedCells;
}
