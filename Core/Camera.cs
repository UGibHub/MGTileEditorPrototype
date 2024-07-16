using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileEditorPrototype;

public class Camera : Component {

    public Matrix Transformation;
    public Vector2 Position;

    public Camera() {
        Position = new Vector2(0,0);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        
    }

    public override void Update(GameTime gameTime) {
        if (Keyboard.GetState().IsKeyDown(Keys.Left)) {
            Position.X -= 10;
        }else if (Keyboard.GetState().IsKeyDown(Keys.Right)) {
            Position.X += 10;
        }
        if (Keyboard.GetState().IsKeyDown(Keys.Up)) {
            Position.Y -= 10;
        }if (Keyboard.GetState().IsKeyDown(Keys.Down)) {
            Position.Y += 10;
        }

    }

    public void CameraFollow(Camera camera) {

        var Position = Matrix.CreateTranslation(
            -camera.Position.X,
            -camera.Position.Y,
            0);
        
        var Offset = Matrix.CreateTranslation(
            TileEditor.ScreenWidth / 2,
            TileEditor.ScreenHeight / 2,
            0);
        
        Transformation = Position * Offset;

    }
}