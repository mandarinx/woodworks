using UnityEngine;
using MIConvexHull;

public class Vertex3 : IVertex {

    public double[] Position { get; set; }
	
    public double x => Position[0];
    public double y => Position[1];
    public double z => Position[2];

    public Vertex3(double x, double y, double z) {
        Position = new [] { x, y, z };
    }
	
    public Vector3 ToVector3() {
        return new Vector3((float)Position[0], (float)Position[1], (float)Position[2]);
    }
}
