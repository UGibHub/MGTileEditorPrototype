using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileEditorPrototype;

public class HelpMenu : State {

    public Dictionary<string, SpriteFont> FontDict;
    public string CurrentHelp;

    public HelpMenu(ContentManager content, GraphicsDevice graphics, TileEditor mainProgram) : base (content, graphics, mainProgram) {

    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        switch (CurrentHelp) {
            case "mainmenu":
                DrawMainHelp(spriteBatch);
                break;
            case "editoractive":
                DrawEditorHelp(spriteBatch);
                break;
            case "exportximport":
                DrawExportImportHelp(spriteBatch);
                break;
        }

    }

    public override void Update(GameTime gameTime) {
        if (Keyboard.GetState().IsKeyDown(Keys.F1) && _mainProgram.StateSwitchCount > 1) {
            _mainProgram.StateSwitchCount = 0;
            if (_mainProgram.StoringState) {
                _mainProgram.NextState = _mainProgram.PreviousState;
                _mainProgram.PreviousState = _mainProgram.StoredState;
                _mainProgram.StoringState = false;
            } else {
                _mainProgram.NextState = _mainProgram.PreviousState;
            }
        }
        
    }

    private void DrawMainHelp(SpriteBatch spriteBatch) {
        spriteBatch.Begin();
        spriteBatch.DrawString(FontDict["large"], "Main Menu Help", new Vector2(650, 20), Color.DarkOliveGreen);
        spriteBatch.DrawString(FontDict["large"], "Press F1 to Return", new Vector2(550, 940), Color.DarkOliveGreen);
        spriteBatch.End();
    }

    private void DrawEditorHelp(SpriteBatch spriteBatch) {
        spriteBatch.Begin();
        spriteBatch.DrawString(FontDict["large"], "Editor Menu Help", new Vector2(610, 20), Color.DarkOliveGreen);
        spriteBatch.DrawString(FontDict["large"], "Press F1 to Return", new Vector2(550, 940), Color.DarkOliveGreen);
        spriteBatch.End();
    }

    private void DrawExportImportHelp(SpriteBatch spriteBatch) {
        spriteBatch.Begin();
        spriteBatch.DrawString(FontDict["large"], "Import/Export Menu Help", new Vector2(460, 20), Color.DarkOliveGreen);
        spriteBatch.DrawString(FontDict["large"], "Press F1 to Return", new Vector2(550, 940), Color.DarkOliveGreen);
        spriteBatch.End();
    }
}