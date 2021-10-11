using Microsoft.Xna.Framework;
using System;

public class CircleCollider : Collider{

	public float r;
	public override bool collidesWith(ICollider c){
		if(c is CircleCollider)
		{
			return collidesWithCircle((CircleCollider)c);
		}
		return false;
	}
	public bool collidesWithCircle(CircleCollider cc)
	{
		//first check bounding boxes for cheap
		if(parent.X - r > cc.parent.X + cc.r)
		{
			return false;
		}else if(parent.X + r < cc.parent.X - cc.r)
		{
			return false;
		}else if (parent.Y - r > cc.parent.Y + cc.r)
		{
			return false;
		}
		else if (parent.Y + r < cc.parent.Y - cc.r)
		{
			return false;
		}
		//then actual calculation
		if (distToCircle(cc) >= this.r + cc.r)
			return false;
		else
			return true;
	}
	public float distToCircle(CircleCollider cc)
	{
		float xdist = this.parent.X - cc.parent.X;
		float ydist = this.parent.Y - cc.parent.Y;
		return MathF.Sqrt((xdist * xdist) + (ydist * ydist));
	}
	public override Vector2 deflectedVector(CircleCollider cc,Vector2 mov) //assuming this circle is immobile, and cc is moving to collide with it, tell cc how it *can* move instead
	{
		if(mov == Vector2.Zero)
		{
			return Vector2.Zero;//avoid div by 0 in collision code
		}
		//find out the point of collision, and how far it can move before hitting it, and how far further it can't move
			//create the circle from before it collided
		CircleCollider previous = new CircleCollider();
		previous.parent = new Position(cc.parent.X - mov.X, cc.parent.Y - mov.Y, cc.parent.Z);
		previous.r = cc.r;
		if (collidesWith(previous))
		{
			return mov; //if you somehow got stuck in a collision already, don't block movement anymore so you can get out
		}
		//https://gamedev.stackexchange.com/questions/158825/resolving-circle-circle-collision
		Vector2 initialDisplacement = new Vector2(previous.parent.X - this.parent.X, previous.parent.Y - this.parent.Y); //point p
		float collisionDistance = cc.r + r; //R
		//time for some quadratic equation terms
		float PdotV = Vector2.Dot(initialDisplacement,mov);
		float PdotP = Vector2.Dot(initialDisplacement, initialDisplacement);
		float VdotV = Vector2.Dot(mov, mov);
		float rsq = collisionDistance * collisionDistance;
		//and then we do the math
		float underRadical = (PdotV * PdotV) - (VdotV * (PdotP - rsq));
		if(underRadical < 0) {
			//throw new Exception("somehow we've flagged non-colliding circles as colliding (or done the math wrong)"); 
			return Vector2.Zero;
		}
		float t1 = (-PdotV - MathF.Sqrt(underRadical)) / VdotV;
		float t2 = (-PdotV + MathF.Sqrt(underRadical)) / VdotV;
		float t = MathF.Min(t1, t2);
		if (t < 0) t = 0f;
		if( t > 1 ) {
			//throw new Exception("t is somehow bigger than the original vector"); 
			t = 1f;
		}
		//we have t now, so apply it to the move vector to get the parts
		Vector2 unmodifiedTravel = mov * t;
		Vector2 remainingDistance = mov - unmodifiedTravel;
		Vector2 collisionPoint = new Vector2(previous.parent.X,previous.parent.Y) + unmodifiedTravel;
		Vector2 displacement = collisionPoint - new Vector2(parent.X, parent.Y);
		//determine the angle of the tangent at the point of collision, or maybe skip that and just make the two vectors corresponding to the directions away
		Vector2 clockwise = new Vector2(-displacement.Y, displacement.X);
		Vector2 widdershins = new Vector2(displacement.Y, -displacement.X);
		//make sure the angle isn't too direct
		float normdot = Vector2.Dot(Vector2.Normalize(clockwise), Vector2.Normalize(mov));
		if(MathF.Abs(normdot) < 0.01f)
		{
			return Vector2.Zero;
		}
		//take the dot product of both vectors, then pick the positive one to get the closer angle. if it's 0, return the plain how-far-can-you-move vector, since the projection is 0
		Vector2 targetVector; //the one we're going to project onto
		float clockdot = Vector2.Dot(mov, clockwise);
		if (clockdot > 0)
		{ //the clockwise-rotated vector is the direction we go in
			targetVector = clockwise;
		}else if(clockdot < 0)
		{//the other one, 180 degrees away, must be positive
			targetVector = widdershins;
		}
		else
		{//we're colliding dead-on
			targetVector = Vector2.Zero;
		}
		//project the remaining distance vector onto that angle, and add that vector to the original one
		Vector2 projection = Util.ProjectVectorOntoVector(remainingDistance, targetVector);
		Vector2 deflected = unmodifiedTravel + projection;
		//then return that vector
		return deflected;
	}

}