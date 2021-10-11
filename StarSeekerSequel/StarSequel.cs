using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PrimitiveBuddy;
using System;

namespace StarSeekerSequel
{
    public class StarSequel : Game
    {
        public static StarSequel GAME;

        private int GAMEWIDTH = 700;
        private int GAMEHEIGHT = 500;

        public Room activeRoom;
        public ConvoDisplay conv;
        public string debug_out = "...";

        private Texture2D starTex;
        private Player star;
        private SpriteFont font;
        private Camera cam;
        public Primitive circle;

        private GraphicsDeviceManager _graphics;

        public StarSequel()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = GAMEWIDTH;
            _graphics.PreferredBackBufferHeight = GAMEHEIGHT;
            _graphics.ApplyChanges();
            _graphics.ToggleFullScreen(); //these are here because of a bug in monogame 3.8 that prevents screen size changes from taking effect
            _graphics.ToggleFullScreen();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            GAME = this;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            GlobalGraphics.initializeGraphics(GraphicsDevice,Content);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            font = Content.Load<SpriteFont>("fonts/OpenDyslexic");
            activeRoom = new Room(@"Content\json\debugroom.json");
            starTex = Content.Load<Texture2D>("star_se");
            Texture2D testBox = Content.Load<Texture2D>("pronounSelector");
            conv = new ConvoDisplay();

            RenderTarget2D menuTest = Util.FreshCanvas(300, 100);
            GraphicsDevice.SetRenderTarget(menuTest);
            GraphicsDevice.Clear(Color.Transparent);
            SpriteBatch sb = GlobalGraphics.batch();
            sb.Begin();
            sb.Draw(testBox, new Vector2(0f,0f), Color.White);
            sb.DrawString(font, "contents", new Vector2(5f, 5f), new Color(0, 0, 0));
            sb.End();
            GraphicsDevice.SetRenderTarget(null);

            cam = new Camera(0, 0, 700, 500);
            activeRoom.cam = cam;

            GameObject floor = new GameObject("bigfloor", new Vector3(0f, 0f, -100f),Content.Load<Texture2D>("debugroom"));
            floor.DisableCutoutOffset();
            GameObject go1 = new GameObject("first", new Vector3(50f, 50f, 0f), starTex);
            GameObject go2 = new GameObject("second", new Vector3(55f, 60f, 0f), starTex);
            GameObject go3 = new GameObject("bottom", new Vector3(45f, 70f, -10f), starTex);
            GameObject menutestobj = new GameObject("menu", new Vector3(60f, 100f, -15f), menuTest);
            //menutestobj.GiveCircleCollider(90f);
            menutestobj.GiveRectCollider(new Vector2(90f,40f),true);

            GameObject bigcircle1 = new GameObject("circle1", new Vector3(200f, 100f, -15f), menuTest);
            bigcircle1.GiveCircleCollider(120f);
            star = new Player("player", new Vector3(250f, 250f, 0f), starTex);
            activeRoom.RegisterObject(go1);
            activeRoom.RegisterObject(go2);
            activeRoom.RegisterObject(go3);
            activeRoom.RegisterObject(floor);
            activeRoom.RegisterObject(menutestobj);
            activeRoom.RegisterPlayer(star);
            //activeRoom.RegisterObject(bigcircle1);

            circle = new Primitive(GlobalGraphics.device(), GlobalGraphics.batch());
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            activeRoom.Update(gameTime);
            conv.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            SpriteBatch sb = GlobalGraphics.batch();
            sb.Begin();
            activeRoom.Draw(activeRoom.cam);
            conv.Draw(null);
            //sb.DrawString(font, "" + star.X + "," + star.Y, new Vector2(5f, 5f), new Color(0, 0, 0));

            Util.DrawStringInFontAtSize(debug_out, font, 40, null, new Vector2(5f, 5f));
            sb.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
        public int getHeight()
        {
            return GAMEHEIGHT;
        }
        public int getWidth()
        {
            return GAMEWIDTH;
        }
    }
}
