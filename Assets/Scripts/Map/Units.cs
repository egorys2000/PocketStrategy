using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FS.Units
{
    public class Unit
    {
        virtual public int Cost { get; }
        virtual public int Expense { get; }
        virtual public string Name { get; }
        public Cell Cell;
        public Player Owner;

        virtual public bool Movable { get; }
    }

    public class Boy : Unit
    {
        private readonly int cost, expense;
        private readonly bool movable;
        private readonly string name;

        public override int Cost 
        {
            get { return cost; }
        }

        public override string Name
        {
            get { return name; }
        }

        public override int Expense
        {
            get { return expense; }
        }

        public override bool Movable
        {
            get { return movable; }
        }

        public Boy(Cell cell, Player owner, int level) //levels: 0, 1, 2
        {
            this.Cell = cell;
            this.Owner = owner;
            this.cost = 10 + level * 40;
            this.expense = -2 - level * 8;
            this.movable = true;
            this.name = "Boy" + level.ToString();
        }
    }

}
