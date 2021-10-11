
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
public sealed class GlobalGraphics{
	private static GlobalGraphics SINGLETON;
    private SpriteBatch _spriteBatch;
	private GraphicsDevice _graphics;
	private SpriteFont _debugFont;

	private GlobalGraphics(GraphicsDevice gd,ContentManager cm){
		this._spriteBatch = new SpriteBatch(gd);
		this._graphics = gd;
		this._debugFont = cm.Load<SpriteFont>("fonts/OpenDyslexic");
	}
	public static void initializeGraphics(GraphicsDevice gd,ContentManager cm){
		if(SINGLETON == null){
			SINGLETON = new GlobalGraphics(gd,cm);
		}
	}
	public static SpriteBatch batch(){
		return SINGLETON._spriteBatch;
	}
	public static GraphicsDevice device(){
		return SINGLETON._graphics;
	}
	public static SpriteFont debugFont()
	{
		return SINGLETON._debugFont;
	}
}