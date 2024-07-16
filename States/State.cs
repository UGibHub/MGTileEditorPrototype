using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TileEditorPrototype;

public abstract class State {

    protected ContentManager _content;
    protected TileEditor _mainProgram;
    protected GraphicsDevice _graphicsDevice;
    protected SpriteFont _font;
    public Dictionary<string, string> InputDict;


    public State(ContentManager content, GraphicsDevice graphicsDevice, TileEditor mainProgram) {
        _content = content;
        _mainProgram = mainProgram;
        _graphicsDevice = graphicsDevice;
    }

    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

    public abstract void Update(GameTime gameTime);

}