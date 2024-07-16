using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Button : Component {

    private Texture2D _texture;
    private MouseState _lastMouse;
    private MouseState _currentMouse;
    private bool _isHovering;
    public string ValueID;
    public bool NumericalValue;
    public Vector2 Position;
    public string Text;
    public Color PenColour;
    public event EventHandler Click;
    public Rectangle Rectangle { get {
        return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
    } }
    private SpriteFont _font;
    public Vector2 Transform;// Used in tandem with any state that has a "Free moving" camera.

    public Button(Texture2D texture, Vector2 position, SpriteFont font) {
        _texture = texture;
        Position = position;
        _font = font;
        PenColour = Color.Black;
        Transform = new Vector2(-200000, -200000);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        var Colour = Color.Gray;
        if (_isHovering) {
            Colour = Color.DarkGray;
        }
        spriteBatch.Draw(_texture, Rectangle, Colour);

        if (!string.IsNullOrEmpty(Text)) {
            spriteBatch.DrawString(_font,
            Text,
            new Vector2(Rectangle.X + Rectangle.Width / 2 - _font.MeasureString(Text).X / 2, Rectangle.Y + Rectangle.Height/2 - _font.MeasureString(Text).Y / 2) 
            ,PenColour);
        }
        
    }

    public override void Update(GameTime gameTime){
        _lastMouse = _currentMouse;
        _currentMouse = Mouse.GetState();

        // A Janky Fix for Mouse having to use screen relative vs world relative coords per state. Works for
        // now, but should probably be fixed down the line.
        Rectangle mouseRectangle;
        if (Transform.X != -200000 && Transform.Y != -200000) {
            mouseRectangle = new Rectangle((int)Transform.X, (int)Transform.Y, 1, 1);
        } else {
            mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);
        }
        

        _isHovering = false;

        if (mouseRectangle.Intersects(Rectangle)) {
            _isHovering = true;

            if (_currentMouse.LeftButton == ButtonState.Released && _lastMouse.LeftButton == ButtonState.Pressed) {
                Click?.Invoke(this, new EventArgs());
            }
        }
    }
}