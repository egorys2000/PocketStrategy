using UnityEngine;
using FS.Units;

[System.Serializable]
public class MapToken: MonoBehaviour
{
    private Unit unit;
    public Unit Unit 
    {
        get { return unit; }   
        private set { unit = value; }
    }

    [SerializeField]
    Sprite UnitSprite;

    private Mesh mesh;
    public Mesh Mesh 
    {
        get { return mesh;}
        private set { mesh = value; }
    }
}
