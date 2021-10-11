using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Xna.Framework.Graphics;
using StarSeekerSequel;

public class ConvoDisplay : IDrawable
{
	private static float FALLSPEED = 2000f;
	private static float BOXRATIO = 0.37f;
	private GameObject bg;
	private float scale = 1f;
	private float bgH;
	private delegate void nextStep();
	public enum CDState { 
		RISING,
		WRITING,
		WAITING,
		FALLING,
		HIDDEN
	}
	public CDState state =  CDState.HIDDEN;
	public ConvoDisplay()
	{
		bg = new GameObject("bg",-50f,StarSequel.GAME.getHeight(),0,StarSequel.GAME.Content.Load<Texture2D>("textbox"));
		bgH = (StarSequel.GAME.getHeight() * BOXRATIO);
		scale = bgH / bg.img.Height;
	}

	public void Update(GameTime gt)
	{
		StarSequel.GAME.debug_out = "update being called";
		if (state == CDState.RISING)
		{
			bg.Y -= FALLSPEED*(float)gt.ElapsedGameTime.TotalSeconds;
			StarSequel.GAME.debug_out = "y val: " + Math.Floor(bg.Y);
			if (bg.Y < StarSequel.GAME.getHeight() - bgH) {
				bg.Y = StarSequel.GAME.getHeight() - bgH;
				state = CDState.WAITING;
			}
		}
		else if(state == CDState.WRITING)
		{

		}
		else if (state == CDState.WAITING)
		{

		}
		else if (state == CDState.FALLING)
		{
			bg.Y += FALLSPEED * (float)gt.ElapsedGameTime.TotalSeconds;
			if (bg.Y > StarSequel.GAME.getHeight() + 1)
			{
				bg.Y = StarSequel.GAME.getHeight() + 1;
				state = CDState.HIDDEN;
			}
		}
		else if (state == CDState.HIDDEN)
		{

		}
	}
	public void Draw(Camera c)
	{
		Vector2 pos = new Vector2(MathF.Floor(bg.X + 0.5f), MathF.Floor(bg.Y + 0.5f));
		GlobalGraphics.batch().Draw(bg.img, pos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None,0f);
		string test = "test string";
		Vector2 size = GlobalGraphics.debugFont().MeasureString(test);
		Util.DrawStringInFontAtSize("''" + test + "'' is " + size.X + " wide.", GlobalGraphics.debugFont(), 40, Color.White, new Vector2(5f,pos.Y));
	}
	public void Summon()
	{
		state = CDState.RISING;
	}
	public void Dismiss()
	{
		state = CDState.FALLING;
	}

}