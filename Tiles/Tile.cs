using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Tile : Component {

    public Texture2D Texture { get; private set; }
    public string TextureName { get; private set; } // Used for JsonExporting/Importing
    public bool IsCollideable; // Used for Json Exporting/Importing
    public bool IsUsed; // If true, Tile Info gets written to Export Json, else omitted
    public Vector2 Position;
    public Rectangle Rectangle { get {
        return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
    } }

    public Tile(Texture2D texture, string textureName, bool isCollide, int tileX, int tileY) {
        Texture = texture;
        TextureName = textureName;
        IsCollideable = isCollide;
        Position = new Vector2(tileX, tileY);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        spriteBatch.Draw(Texture, Position, Color.White);
    }

    public override void Update(GameTime gameTime) {
        
    }

    public void ChangeTexture(Texture2D newTexture, bool collideOrNot, bool isused, string textureName) {
        Texture = newTexture;
        IsCollideable = collideOrNot;
        IsUsed = isused;
        TextureName = textureName;
    }
}