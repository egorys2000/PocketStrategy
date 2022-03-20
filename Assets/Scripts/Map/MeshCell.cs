using UnityEngine;

//Geometric properies of Cell
public class MeshCell
{
    public Mesh Mesh;
    private const float OutlineColorDifference = 0.15f;

    public MeshCell(Vector3 Pos, Player Owner)
    {
        this.Mesh = BuildCellMesh(Pos, Owner, 1f);
    }

    public Mesh BuildCellMesh(Vector3 centre, Player Owner, float ColorFactor,
        float OutlineWidth = 0.125f, float OutlineColorFactor = 0.75f) 
    {
        int sides = MapConstants.CellSides; //too long name
        Mesh NewMesh = new Mesh();

        // Center point + Inner & outer polygon!!!
        Vector3[] vertices = new Vector3[2 * sides + 1];
        vertices[0] = centre;

        float step_angle = 2 * Mathf.PI / sides;
        float angle = 0;
        for (int counter = 1; counter <= sides; counter++ ) 
        {
            vertices[counter] 
                = centre + new Vector3(Mathf.Cos(angle + MapConstants.GlobalAngle), Mathf.Sin(angle + MapConstants.GlobalAngle), 0) * MapConstants.CellRadius;
            angle += step_angle;
        }

        for (int counter = sides + 1; counter < vertices.Length; counter++)
        {
            vertices[counter]
                = centre + new Vector3(Mathf.Cos(angle + MapConstants.GlobalAngle), Mathf.Sin(angle + MapConstants.GlobalAngle), 0) * MapConstants.CellRadius * (1 - OutlineWidth);
            angle += step_angle;
        }

        Color[] colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length / 2 + 1; i++)
            colors[i] = Owner.Color * OutlineColorFactor * (ColorFactor - OutlineColorDifference);
        for (int i = vertices.Length / 2 + 1; i < vertices.Length; i++)
            colors[i] = Owner.Color * ColorFactor;

        NewMesh.vertices = vertices;
        NewMesh.colors = colors;

        int[] tris = new int[3 * 2 * sides];

        for (int i = 0; i < sides; i++) 
        {
            tris[i * 3 + 0] = (i + 1);
            tris[i * 3 + 1] = 0;
            tris[i * 3 + 2] = (i + 1)%(sides) + 1;
        }
        for (int i = sides; i < 2 * sides - 1; i++)
        {
            tris[i * 3 + 0] = (i + 1);
            tris[i * 3 + 1] = 0;
            tris[i * 3 + 2] = (i + 1) % (2 * sides) + 1;
        }

        NewMesh.triangles = tris;

        Vector3[] normals = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
            normals[i] = Vector3.forward;

        NewMesh.normals = normals;

        this.Mesh = NewMesh;

        return NewMesh;
    }
}
