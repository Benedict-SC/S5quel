using System.Runtime.CompilerServices;

public class Position : IPosition{
	public float X { get; set; }
	public float Y { get; set; }
	public float Z { get; set; }
	public Position(float nx, float ny, float nz){
		this.X = nx;
		this.Y = ny;
		this.Z = nz;
	}

	public static Position operator +(Position a, Position b)
	{
		return new Position(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
	}
	public static Position operator -(Position a, Position b)
	{
		return new Position(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
	}
}