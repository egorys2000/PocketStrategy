using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PlayersList", order = 1)]
public class PlayersList : ScriptableObject
{
    [System.Serializable]
    public class DefaultPlayer
    {
        public Color Color;
        public string Name;

        public DefaultPlayer(Color color, string name)
        {
            this.Color = color;
            this.Name = name;
        }

        public static implicit operator Player(DefaultPlayer DP) => new Player(DP.Color, DP.Name);
        public static explicit operator DefaultPlayer(Player P) => new DefaultPlayer(P.Color, P.Name);
    }

    

    public DefaultPlayer[] DefaultPlayers;
}


