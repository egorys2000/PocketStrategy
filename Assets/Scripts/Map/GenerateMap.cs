using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using System.Linq;

public static class GenerateMap
{
    static float[] possible_angle;
    public static Vector2[] VectorToReachNeighbour;
    public static float inscribed_radius, distance_to_nearest_nighbour;

    //Unit radius!
    private static void InitialGeometryCalculations(int sides, float phi0) 
    {
        RadiusIdx = 0; //Map radius counter during generation

        float angle = 2 * Mathf.PI / sides;
        possible_angle = new float[sides];

        inscribed_radius = 1 * Mathf.Cos(angle / 2);
        distance_to_nearest_nighbour = inscribed_radius * 2; //to nearest neighbour's center

        VectorToReachNeighbour = new Vector2[MapConstants.CellSides];
        for (int i = 0; i < sides; i++)
        {
            float phi = angle / 2 + angle * i + phi0;
            possible_angle[i] = phi;
            VectorToReachNeighbour[i] = new Vector2(Mathf.Cos(phi), Mathf.Sin(phi)) * distance_to_nearest_nighbour;
            VectorToReachNeighbour[i] = RoundVectorComponents(VectorToReachNeighbour[i]);
        }
    }

    private static void CreatePossiblePositions(int MaxRadius) //Map Radius measured in cells
    {
        PossiblePositions = new Dictionary<Vector2, bool>(); // 0 if cell will NOT be created at this position, non-zero integer indicates possibility

        Vector2 INITIAL_POS = new Vector2(0, 0);
        PossiblePositions.Add(INITIAL_POS, false);

        AddNeighboursIteratively(new List<Vector2> { INITIAL_POS }, MaxRadius);
    }

    private static int RadiusIdx = 0;

    private static Vector2 RoundVectorComponents(Vector2 Vect) 
    {
        int digits = 3;
        float factor = Mathf.Pow(10, digits);

        float x = Mathf.Round(Vect.x * factor) / factor;
        float y = Mathf.Round(Vect.y * factor) / factor;

        return new Vector2(x, y);
    }

    private static void AddNeighboursIteratively(List<Vector2> AroundPos, int MaxRadius) 
    {
        List<Vector2> JustCreatedPositions = new List<Vector2>();

        foreach (var VectorPos in AroundPos)
        {
            foreach (var Vector in VectorToReachNeighbour)
            {
                var NewPosition = VectorPos + Vector;
                NewPosition = RoundVectorComponents(NewPosition);

                bool expanding_outside = (Vector2.Dot(NewPosition, VectorPos) >= 0); //can save 50% time by not creating already created cells

                if (!PossiblePositions.ContainsKey(NewPosition) && expanding_outside)
                {
                    PossiblePositions.Add(NewPosition, false);
                    JustCreatedPositions.Add(NewPosition);
                }
            }
        }
        RadiusIdx++;

        if (RadiusIdx < MaxRadius)
        {
            AddNeighboursIteratively(JustCreatedPositions, MaxRadius);
        }
    }

    private static void PrintDict<T1, T2>(Dictionary<T1, T2> Dict) //Debug
    {
        foreach (var element in Dict) 
        {
            Debug.Log(element);
        }
    }

    private static Dictionary<Vector2, bool> PossiblePositions;
    private static Dictionary<Vector2, Cell> MainMap; // MAIN DICTIONARY, whole class purpose is to generate it

    private static float FillPercentage = 0.70f; //of 1

    //even number of sides ONLY! else neighbouring cell have to be rotated
    //best sides amount: 4 and 6
    public static Dictionary<Vector2, Cell> GenerateCellsPositions(int UltimateMapRadius, int MinMapRadius,
        int MaxMapRadius, float PassedFillPercentage, ref List<Player> CurrentPlayersList)
    {
        //rewrite passed variables
        FillPercentage = PassedFillPercentage;
        MinRealRadius = MinMapRadius;
        MaxRealRadius = MaxMapRadius;

        InitialGeometryCalculations(MapConstants.CellSides, MapConstants.GlobalAngle);

        CreatePossiblePositions(UltimateMapRadius);
        
        for (int i = 0; i < PossiblePositions.Count; i++) 
        {
            var Vector = PossiblePositions.Keys.ToList()[i];
            bool born_cell_here = Chance(FillPercentage);
            PossiblePositions[Vector] = born_cell_here;
        }

        SmoothMap();

        CellsPassedSelection(CurrentPlayersList); //creates MainMap
        SubscribeNeighbours();

        return MainMap;
    }

    private static float MinNeighbours = 3, MaxNeighbours = 5; //Hexagon

    private static int MinRealRadius = -1, MaxRealRadius = -1;


    private static void SmoothMap() //Removes some cells
    {
        int random_idx = Random.Range(0, PossiblePositions.Count);

        float MapRealSize = /*Random.Range(min_delete_radius, max_delete_radius)*/ 9 * distance_to_nearest_nighbour;
        Vector2 CentralPosition = PossiblePositions.Keys.ToList()[random_idx] * 0.5f;

        float a, b;
        a = Random.Range(MinRealRadius, MaxRealRadius) * distance_to_nearest_nighbour;
        b = Random.Range(MinRealRadius, MaxRealRadius) * distance_to_nearest_nighbour;

        foreach (var Vector in PossiblePositions.Keys.ToList())  
        {
            //Checking how many neighbours
            int true_neighbours = 0;
            foreach (var VectorToNeighbour in VectorToReachNeighbour) 
            {
                if (PossiblePositions.ContainsKey(Vector + VectorToNeighbour))
                { if (PossiblePositions[Vector + VectorToNeighbour]) true_neighbours++; }                
            }



            PossiblePositions[Vector] =
                ((true_neighbours >= MinNeighbours && true_neighbours <= MaxNeighbours) ||
                Chance(0.15f));

            
        }

        ElipticCutMap(CentralPosition, a, b);
    }

    private static void ElipticCutMap(Vector2 centre, float a, float b) 
    {
        List<Vector2> vectors_to_delete = new List<Vector2>();
        float buffer_zone = 1f;
        foreach (var Vector in PossiblePositions.Keys.ToList())
        {
            Vector2 relative_vector = Vector - centre;
            float x = relative_vector.x;
            float y = relative_vector.y;

            float InsideEllipseExperssion = Mathf.Pow(x / a, 2) + Mathf.Pow(y / b, 2);
            //bool InsideEllipse = Mathf.Pow(x / a, 2) + Mathf.Pow(y / b, 2) <= 1;            
            if (InsideEllipseExperssion > 1) 
            {
                if (InsideEllipseExperssion > 1 + buffer_zone) vectors_to_delete.Add(Vector);
                else if (Chance(0.5f)) vectors_to_delete.Add(Vector);
                else PossiblePositions[Vector] = false;
            }
        }

        foreach (var to_delete in vectors_to_delete) 
        {
            PossiblePositions.Remove(to_delete);
        }
    }

    private static bool Chance(float prob) //prob < 1
    {
        //Random.seed = System.DateTime.Now.Millisecond;
        float random;
        random = Random.Range(0f, 1f);
        return prob > random;
    }

    private static void CellsPassedSelection(List<Player> CurrentPlayersList) 
    {
        var UnclaimedGround = (Player)CurrentPlayersList[0];
        var UnclaimedWater  = (Player)CurrentPlayersList[1];

        MainMap = new Dictionary<Vector2, Cell>();
        PossiblePositions.Distinct();
        foreach (var Vector in PossiblePositions.Keys.ToList())
        {
            if (PossiblePositions[Vector])
            { 
                MainMap.Add(Vector, new Cell(Vector, UnclaimedGround));                
            }
            else MainMap.Add(Vector, new Cell(Vector, UnclaimedWater));
        }
    }

    private static void SubscribeNeighbours() //Neighbour cell - unclaimed 
    {
        foreach (Vector2 VectorToCell in MainMap.Keys.ToList() )
        foreach (Vector2 ToNeighbour in VectorToReachNeighbour)
        {
            var NeighbourPos = RoundVectorComponents( VectorToCell + ToNeighbour);
            if (MainMap.ContainsKey(NeighbourPos))
                MainMap[VectorToCell].Neighbours.Add(MainMap[NeighbourPos]);
        }
    }
}
