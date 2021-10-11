using Microsoft.Xna.Framework;
using System;

public class Camera : IUpdateable{
	public int X{ get; set;}
	public int Y{ get; set;}

	public int W{ get; set;}
	public int H{ get; set;}

	public bool Enabled { get; }
	public int UpdateOrder { get; }
	public event EventHandler<EventArgs> EnabledChanged;
	public event EventHandler<EventArgs> UpdateOrderChanged;

	public Camera(int x, int y, int w, int h){
		this.X = x;
		this.Y = y;
		this.W = w;
		this.H = h;
		this.Enabled = true;
		this.UpdateOrder = 1;
	}

	public void Update(GameTime gt)
	{

	}
}