using Microsoft.Xna.Framework;
using System;
using System.Xml.Schema;
using System.Drawing;

public class RectCollider : Collider{
	public Vector2 size;
	public Vector2 lastDeflect = Vector2.Zero;
	public string lastDefType = "none";
	public override bool collidesWith(ICollider c){
		if(c is CircleCollider)
		{
			return collidesWithCircle((CircleCollider)c);
		}
		else if(c is RectCollider)
		{
			return collidesWithRect((RectCollider)c);
		}
		return false;
	}
	public bool collidesWithCircle(CircleCollider cc)
	{
		PointF circleCenter = new PointF(cc.parent.X, cc.parent.Y);
		PointF origin = new PointF(this.parent.X + this.offset.X, this.parent.Y + this.offset.Y);
		SizeF sf = new SizeF(this.size.X, this.size.Y);
		RectangleF rf = new RectangleF(origin, sf);
		if(rf.Contains(new PointF(cc.parent.X, cc.parent.Y)))
		{
			return true;
		}
		PointF nw = new PointF(origin.X, origin.Y);
		PointF ne = new PointF(origin.X + sf.Width, origin.Y);
		PointF sw = new PointF(origin.X, origin.Y + sf.Height);
		PointF se = new PointF(origin.X + sf.Width, origin.Y + sf.Height);
		if (Util.LineIntersectsCircle(nw, ne, circleCenter, cc.r))
		{
			return true;
		}
		else if(Util.LineIntersectsCircle(ne, se, circleCenter, cc.r))
		{
			return true;
		}
		else if(Util.LineIntersectsCircle(se, sw, circleCenter, cc.r))
		{
			return true;
		}
		else if(Util.LineIntersectsCircle(sw, nw, circleCenter, cc.r))
		{
			return true;
		}

		return false;
	}
	public bool collidesWithRect(RectCollider rc)
	{
		return (	(rc.parent.X + rc.offset.X) < this.parent.X + this.offset.X + this.size.X
				&&	(rc.parent.X + rc.offset.X) + rc.size.X > (this.parent.X + this.offset.X)
				&&	(rc.parent.Y + rc.offset.Y) < this.parent.Y + this.offset.Y + this.size.Y
				&&	(rc.parent.Y + rc.offset.Y) + rc.size.Y > (this.parent.Y + this.offset.Y)
				);
	}

	//breaks if your circle has a radius greater than 1000;
	public override Vector2 deflectedVector(CircleCollider cc, Vector2 mov) //assuming this circle is immobile, and cc is moving to collide with it, tell cc how it *can* move instead
	{
		if (mov == Vector2.Zero)
		{
			return Vector2.Zero;//avoid div by 0 in collision code
		}
		//find out the point of collision, and how far it can move before hitting it, and how far further it can't move
		//create the circle from before it collided
		CircleCollider previous = new CircleCollider();
		previous.parent = new Position(cc.parent.X - mov.X, cc.parent.Y - mov.Y, cc.parent.Z);
		previous.r = cc.r;
		/*if (collidesWith(previous))
		{
			return mov; //if you somehow got stuck in a collision already, don't block movement anymore so you can get out
		}*/

		//where the starting position was
		bool insideTopAndBottom = previous.parent.Y > this.parent.Y + this.offset.Y && previous.parent.Y < this.parent.Y + this.offset.Y + this.size.Y;
		bool insideLeftAndRight = previous.parent.X > this.parent.X + this.offset.X && previous.parent.X < this.parent.X + this.offset.X + this.size.X;
		bool cornerCase = !insideLeftAndRight && !insideTopAndBottom;
		//where the new position is
		bool collInsideTopAndBottom = cc.parent.Y > this.parent.Y + this.offset.Y && cc.parent.Y < this.parent.Y + this.offset.Y + this.size.Y;
		bool collInsideLeftAndRight = cc.parent.X > this.parent.X + this.offset.X && cc.parent.X < this.parent.X + this.offset.X + this.size.X;
		bool collCornerCase = !collInsideLeftAndRight && !collInsideTopAndBottom;

		if(insideTopAndBottom && collInsideTopAndBottom) //you started on and ended east/west
		{
			if (mov.X == 0) return mov;
			if(mov.X < 0)
			{
				float circleLeftX = cc.parent.X - cc.r;
				float rectRightSide = this.parent.X + this.offset.X + this.size.X;
				this.lastDeflect = new Vector2(mov.X + (rectRightSide - circleLeftX) + 0.1f, mov.Y);
				this.lastDefType = "ortho";
				return this.lastDeflect;
			}
			else
			{
				float circleRightX = cc.parent.X + cc.r;
				float rectLeftSide = this.parent.X + this.offset.X;
				this.lastDeflect = new Vector2(mov.X + (rectLeftSide - circleRightX) - 0.1f, mov.Y);
				this.lastDefType = "ortho";
				return this.lastDeflect;
			}
		}else if(insideLeftAndRight && collInsideLeftAndRight) //you started on and ended north/south
		{
			if (mov.Y == 0) return mov;
			if (mov.Y < 0)
			{
				float circleTopY = cc.parent.Y - cc.r;
				float rectBottomSide = this.parent.Y + this.offset.Y + this.size.Y;
				this.lastDeflect = new Vector2(mov.X, mov.Y + (rectBottomSide - circleTopY) +0.1f);
				this.lastDefType = "ortho";
				return this.lastDeflect;
			}
			else
			{
				float circleBottomY = cc.parent.Y + cc.r;
				float rectTopSide = this.parent.Y + this.offset.Y;
				this.lastDeflect = new Vector2(mov.X, mov.Y + (rectTopSide - circleBottomY) -0.1f);
				this.lastDefType = "ortho";
				return this.lastDeflect;
			}
		}else //you're on a corner
		{
			//detect which corner point it is
			PointF circleCenter = new PointF(cc.parent.X, cc.parent.Y);
			PointF origin = new PointF(this.parent.X + this.offset.X, this.parent.Y + this.offset.Y);
			SizeF sf = new SizeF(this.size.X, this.size.Y);
			RectangleF rf = new RectangleF(origin, sf);
			PointF nw = new PointF(origin.X, origin.Y);
			PointF ne = new PointF(origin.X + sf.Width, origin.Y);
			PointF sw = new PointF(origin.X, origin.Y + sf.Height);
			PointF se = new PointF(origin.X + sf.Width, origin.Y + sf.Height);
			bool topish = MathF.Abs(nw.Y - circleCenter.Y) < MathF.Abs(sw.Y - circleCenter.Y);
			bool leftish = MathF.Abs(nw.X - circleCenter.X) < MathF.Abs(ne.X - circleCenter.X);
			PointF corner;
			if (topish && leftish) corner = nw;
			else if (topish && !leftish) corner = ne;
			else if (!topish && leftish) corner = sw;
			else /*!topish && !leftish*/ corner = se;
			//get the out-direction vector
			float far = 1000f;
			PointF rayTarget = new PointF(corner.X + (leftish ? -far : far), corner.Y + (topish ? -far : far));

			//https://math.stackexchange.com/questions/311921/get-location-of-vector-circle-intersection
			float a = (corner.X - rayTarget.X) * (corner.X - rayTarget.X) + (corner.Y - rayTarget.Y) * (corner.Y - rayTarget.Y);
			float b = 2 * (corner.X - rayTarget.X) * (rayTarget.X - circleCenter.X) + 2 * (corner.Y - rayTarget.Y) * (rayTarget.Y - circleCenter.Y);
			float c = ((rayTarget.X - circleCenter.X) * (rayTarget.X - circleCenter.X) + (rayTarget.Y - circleCenter.Y) * (rayTarget.Y - circleCenter.Y)) - cc.r * cc.r;
			if (a == 0f) 
				return Vector2.Zero;
			float radical = MathF.Sqrt((b * b) - (4 * a * c));
			float t1 = (-b + radical) / (2 * a);
			//float t2 = (-b - radical) / (2 * a);
			PointF p1 = new PointF((corner.X - rayTarget.X) * t1 + rayTarget.X, (corner.Y - rayTarget.Y) * t1 + rayTarget.Y);
			//PointF p2 = new PointF((corner.X - rayTarget.X) * t2 + rayTarget.X, (corner.Y - rayTarget.Y) * t2 + rayTarget.Y);
			Vector2 displacement = new Vector2(corner.X - p1.X, corner.Y - p1.Y);
			displacement.X *= 1.05f;
			displacement.Y *= 1.05f;
			Vector2 deflect = mov + displacement;
			this.lastDeflect = deflect;
			this.lastDefType = "corner";
			return deflect;
		}


		return Vector2.Zero;
	}



}