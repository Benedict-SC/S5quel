using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StarSeekerSequel;
using System.Collections.Generic;
using System;

public class Player : GameObject{

	private float speed = 200f;

	private Position interactCenter;
	private CircleCollider interactRange;
	private Vector2 interactOffset;
	private KeyboardState lastState;
	public Player(string name, Vector3 pos, Texture2D img) : base(name,pos,img)
	{
		this.GiveCircleCollider(10f);
		this.interactRange = new CircleCollider();
		this.interactRange.r = 12f;
		this.interactCenter = new Position(0f, 0f, 0f);
		this.interactOffset = new Vector2(0f, 0f);
		this.interactRange.parent = interactCenter;
	}
	public override void Draw(Camera c)
	{
		base.Draw(c);
		StarSequel.GAME.circle.Circle(offset + this.interactOffset - this.imgOffset, interactRange.r, Color.Blue);
	}

	public override void Update(GameTime gt)
	{
		var kstate = Keyboard.GetState();
		float yVel = 0f;
		float xVel = 0f;
		if (kstate.IsKeyDown(Keys.Up))
			yVel -= speed * (float)gt.ElapsedGameTime.TotalSeconds;

		if (kstate.IsKeyDown(Keys.Down))
			yVel += speed * (float)gt.ElapsedGameTime.TotalSeconds;

		if (kstate.IsKeyDown(Keys.Left))
			xVel -= speed * (float)gt.ElapsedGameTime.TotalSeconds;

		if (kstate.IsKeyDown(Keys.Right))
			xVel += speed * (float)gt.ElapsedGameTime.TotalSeconds;

		X += xVel;
		Y += yVel;

		bool colliding = true;
		int maxChecks = 10;
		List<Collider> allColliders = StarSequel.GAME.activeRoom.AllColliders();
		Vector2 mov = new Vector2(xVel, yVel);
		this.UpdateDirection(mov);
		for (int i = 0; i < maxChecks && colliding; i++)
		{
			Collider collColliding = null;
			foreach(Collider c in allColliders)
			{
				if(c != coll && c.collidesWith(coll))
				{
					collColliding = c;
					break;
				}
			}
			if(collColliding != null)
			{
				Vector2 deflected = collColliding.deflectedVector((CircleCollider)coll, mov);
				X -= mov.X;
				Y -= mov.Y;
				mov = deflected;
				X += mov.X;
				Y += mov.Y;
			}
			else
			{
				colliding = false;
				break;
			}
		}
		if (colliding)//we've tried 10 times and we're still colliding with something
		{
			X -= mov.X;
			Y -= mov.Y; //just don't move
		}

		//trigger dialog box logic
		if(kstate.IsKeyDown(Keys.Space) && !(lastState.IsKeyDown(Keys.Space)))
		{//the spacebar was just pressed
			if(StarSequel.GAME.conv.state == ConvoDisplay.CDState.HIDDEN)
			{
				StarSequel.GAME.conv.Summon();
			}else if (StarSequel.GAME.conv.state == ConvoDisplay.CDState.WAITING)
			{
				StarSequel.GAME.conv.Dismiss();
			}
		}

		lastState = kstate;
	}

	public void UpdateDirection(Vector2 mov)
	{
		float dist = 18f;
		Vector2 idir = new Vector2(0f, 0f);
		if(mov.X > 0)
		{
			idir.X = dist;
		}else if(mov.X < 0)
		{
			idir.X = -dist;
		}
		if (mov.Y > 0)
		{
			idir.Y = dist;
		}
		else if (mov.Y < 0)
		{
			idir.Y = -dist;
		}
		if(MathF.Abs(idir.Y) + MathF.Abs(idir.X) >= dist * 2)
		{
			idir *= 0.707f;
		}
		if(idir.Length() > 0) { 
			this.interactCenter.X = this.X + idir.X;
			this.interactCenter.Y = this.Y + idir.Y;
			this.interactOffset = idir;
		}
	}
} 