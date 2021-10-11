using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Drawing;

public class Util{

	public static RenderTarget2D FreshCanvas(int w, int h){

        GraphicsDevice device = GlobalGraphics.device();
        //SurfaceFormat surface = SurfaceFormat.Single;

		return new RenderTarget2D(GlobalGraphics.device(), w, h);
	}
	public static Vector2 ProjectVectorOntoVector(Vector2 v, Vector2 u)
	{
		float dot = Vector2.Dot(v, u);
		dot /= (u.Length() * u.Length());
		return u * dot;
	}
	public static bool LineIntersectsCircle(PointF a, PointF b, PointF center, float r)
	{
		Vector2 ac = new Vector2(center.X - a.X, center.Y - a.Y);
		Vector2 ab = new Vector2(b.X - a.X, b.Y - a.Y);
		Vector2 ad = Util.ProjectVectorOntoVector(ac, ab);
		if (Vector2.Dot(ab, ad) <= 0)
		{
			ad = Vector2.Zero; //clamp to A
		}
		if (ad.Length() >= ab.Length())
		{
			ad = ab; //clamp to B;
		}
		Vector2 d = new Vector2(a.X + ad.X, a.Y + ad.Y);
		Vector2 cd = new Vector2(d.X - center.X, d.Y - center.Y);
		if(cd.Length() <= r)
		{
			return true;
		}
		return false;
	}
	public static float StringScaleFactor(string str, SpriteFont sf, int pixelLineHeight)
	{
		Vector2 bigness = sf.MeasureString(str);
		return (float)pixelLineHeight / bigness.Y;
	}
	public static void DrawStringInFontAtSize(string str,SpriteFont sf,int pixelLineHeight,Microsoft.Xna.Framework.Color? col, Vector2? loc)
	{
		//set defaults
		Microsoft.Xna.Framework.Color color = col != null ? (Microsoft.Xna.Framework.Color)col : new Microsoft.Xna.Framework.Color(0, 0, 0);
		Vector2 location = loc != null ? (Vector2)loc : Vector2.Zero;

		//determine scaling
		float shrinkage = StringScaleFactor(str, sf, pixelLineHeight);

		//set up temporary canvas and draw big text
		SpriteBatch sb = GlobalGraphics.batch();
		//sb.Begin();
		sb.DrawString(sf, str, location, color,0f,Vector2.Zero,shrinkage,SpriteEffects.None,0);
		//now we have a big version of the text. draw it small.
		//Microsoft.Xna.Framework.Point ploc = new Microsoft.Xna.Framework.Point((int)(location.X + 0.5), (int)(location.Y + 0.5));
		//Microsoft.Xna.Framework.Point size = new Microsoft.Xna.Framework.Point((int)((bigness.X / sourceComparedToDesired )+0.5), (int)((bigness.Y / sourceComparedToDesired )+0.5));
		//Microsoft.Xna.Framework.Rectangle rect = new Microsoft.Xna.Framework.Rectangle(ploc, size);
		//sb.Draw(bigcanvas, rect, Microsoft.Xna.Framework.Color.White);
		//sb.End();

	}
}