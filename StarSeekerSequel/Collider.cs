using Microsoft.Xna.Framework;

public abstract class Collider : ICollider{
	
	public IPosition parent;
	public Vector2 offset;
	public abstract bool collidesWith(ICollider c);

	public abstract Vector2 deflectedVector(CircleCollider cc, Vector2 mov);
}