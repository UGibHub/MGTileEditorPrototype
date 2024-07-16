using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TileEditorPrototype;
/*
Read the Manual.txt file to understand WTF this is (For future me after I haven't touched this for months..)
Will be in Project folder.
*/
public class TileEditor : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    
    public static int ScreenWidth;
    public static int ScreenHeight;
    public bool StoringState = false;
    public float StateSwitchCount;

    public State CurrentState;
    public State NextState;
    public State PreviousState;
    public State StoredState; // State storage used only in very rare case. If user views help in the import/export
                              // menu, storing two previous states becomes necessary. No other use case I can think 
                              // of at the moment. For multi layer game menu, might want to use a list for this,
                              // something I can clear, or iterate and "remove at" if the state type is no longer
                              // needed..

    public TileEditor()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {

        _graphics.PreferredBackBufferWidth = 1920;
        _graphics.PreferredBackBufferHeight = 1080;
        ScreenWidth = _graphics.PreferredBackBufferWidth;
        ScreenHeight = _graphics.PreferredBackBufferHeight;
        _graphics.ApplyChanges();
        _graphics.IsFullScreen = true;

        CurrentState = new MainMenu(Content,_graphics.GraphicsDevice, this) {
            InputDict = new(),
        };

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        if (NextState != null) {
            CurrentState = NextState;
            NextState = null;
        }
        StateSwitchCount += (float)gameTime.ElapsedGameTime.TotalSeconds;

        //Just in case I ever leave my laptop on with the program running for 11,574 days.
        if (StateSwitchCount >= 1_000_000_000f) { StateSwitchCount = 0f;}

        if (Keyboard.GetState().IsKeyDown(Keys.Escape)) {
            Exit();
        }
        
        CurrentState.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        
        CurrentState.Draw(gameTime, _spriteBatch);
        

        base.Draw(gameTime);
    }
}
