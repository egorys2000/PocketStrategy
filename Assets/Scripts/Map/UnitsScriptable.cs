using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/UnitsList", order = 1)]
public class UnitsScriptable : ScriptableObject
{
    [System.Serializable]
    public class UnitInfo 
    {
        public Sprite Sprite;
        public string Name;
    }

    public UnitInfo[] UnitsInfo;
}


