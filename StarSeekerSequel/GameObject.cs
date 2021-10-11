using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StarSeekerSequel;

public class GameObject: IDrawable, IPosition{
	public string GUID { get; set; }
	public float X{ get; set;}
	public float Y{ get; set;}
	public float Z{ get; set;}
	public Collider coll { get; set; }
	public Texture2D img { get; set; }
	public Vector2 imgOffset;

	public Vector2 offset;
	public Rectangle clipBounds;
	public virtual void Draw(Camera c){
		if(img == null)
		{
			return;
		}
		if(X + img.Width + imgOffset.X< c.X 
			|| Y + img.Height + imgOffset.Y < c.Y
			|| c.X + c.W < X + imgOffset.X
			|| c.Y + c.H < Y + imgOffset.Y)
		{
			return; //don't draw it! it's entirely offscreen!
		}
		else
		{
			Vector2 pos = new Vector2(MathF.Floor(X + imgOffset.X + 0.5f), MathF.Floor(Y + imgOffset.Y + 0.5f));
			clipBounds = new Rectangle(new Point(c.X - (int)pos.X, c.Y - (int)pos.Y), new Point(c.W, c.H));
			offset = new Vector2(0f, 0f);
			if(clipBounds.X < 0)
			{
				offset.X = Math.Abs(clipBounds.X);
				clipBounds.X = 0;
			}
			if(clipBounds.Y < 0)
			{
				offset.Y = Math.Abs(clipBounds.Y);
				clipBounds.Y = 0;
			}
			if(clipBounds.Width > img.Width - clipBounds.X)
			{
				clipBounds.Width = img.Width - clipBounds.X;
			}
			if (clipBounds.Height > img.Height - clipBounds.Y)
			{
				clipBounds.Height = img.Height - clipBounds.Y;
			}
			GlobalGraphics.batch().Draw(img, offset, clipBounds, Color.White);
			if(coll != null && coll is CircleCollider)
			{
				StarSequel.GAME.circle.Circle(offset - imgOffset, ((CircleCollider)coll).r, Color.Green);
			}
			if(coll != null && coll is RectCollider)
			{
				RectCollider rc = (RectCollider)coll;
				StarSequel.GAME.circle.Rectangle(new Rectangle(((int)(offset.X - imgOffset.X + rc.offset.X)), ((int)(offset.Y - imgOffset.Y + rc.offset.Y)), (int)rc.size.X, (int)rc.size.Y),Color.Green);
			}
			//GlobalGraphics.batch().DrawString(GlobalGraphics.debugFont(), "" + clipBounds.X + "," + clipBounds.Y, pos, Color.White);
		}
	}
	public GameObject(string name, Vector3 pos, Texture2D img): this(name,pos.X,pos.Y,pos.Z,img){}
	public GameObject(string name, float x, float y, float z, Texture2D img)
	{
		this.GUID = name;
		this.X = x;
		this.Y = y;
		this.Z = z;
		this.img = img;
		this.ApplyCutoutOffset();
	}
	public virtual void Update(GameTime gt)
	{

	}
	public void GiveCircleCollider(float rad)
	{
		CircleCollider c = new CircleCollider();
		c.r = rad;
		this.coll = c;
		c.parent = this;
	}
	public void GiveRectCollider(Vector2 size, Vector2? offset = null)
	{
		RectCollider c = new RectCollider();
		c.size = size;
		if (offset != null)
		{
			c.offset = (Vector2)offset;
		}
		else
		{
			c.offset = Vector2.Zero;
		}
		this.coll = c;
		c.parent = this;
	}
	public void GiveRectCollider(Vector2 size, bool cutout)
	{
		if (!cutout)
		{
			this.GiveRectCollider(size, null);
		}
		else
		{
			this.GiveRectCollider(size, img != null ? new Vector2(-img.Width / 2, -img.Height) : Vector2.Zero);
		}
	}
	public void ApplyCutoutOffset(bool collider = false)
	{
		if (img == null) return;
		this.imgOffset = new Vector2(-img.Width / 2, -img.Height);
		if (collider && this.coll != null)
		{
			this.coll.offset = new Vector2(-img.Width / 2, -img.Height);
		}
	}
	public void DisableCutoutOffset()
	{
		this.imgOffset = Vector2.Zero;
	}
}