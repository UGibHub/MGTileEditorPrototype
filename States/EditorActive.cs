using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileEditorPrototype;

public class EditorActive : State {

    public Dictionary<string, Texture2D> Texture_Dict; // Textures for the program to pass around (EditorSpecific)
    public Dictionary<string, SpriteFont> Font_Dict; // Fonts for the program to pass around
    public Dictionary<string, Dictionary<string, string>> Grid_Dict; // String based reference for the grid.
    public Dictionary<string, Texture2D> Game_Textures; // Game Specfific textures
    public List<Tile> Tile_List; // List of all tiles on the grid
    public List<Tile> Tile_Palette; // List of one of each type of tile for "Painting"
    public List<Component> Components; // Buttons
    public Camera Camera; // Camera Object. Arrow Keys control movement of the camera around the grid in this state.
    public MouseState CurrentMouse;
    public MouseState PreviousMouse;
    public Vector2 RelativeMousePos; // Need to invert the transformation matrix from the camera to get the mouse
                                     // Position relative to the world. Needs to be updated constantly.
                                     // Note: This is only necessary in states with a moving "viewPort"
    public List<string> HoverStrings; //List of Strings to actively display next to the mouse via HUD.
    public Tile CurrentTile; // Current Tile Selected from Palette, Left click to apply hovered tile on the grid
    public Tile EmptyTile;  // An empty tile, used to right click and erase the current hovered tile on the grid                   
    public float PlacementTimer; // Allows user to hold down mouse button to "Paint" or "Erase" multiple tiles.
                                 // "Anti Carpel Tunnel"  measures.


    public EditorActive(ContentManager contentManager, GraphicsDevice graphicsDevice, TileEditor mainProgram, Dictionary<string,Texture2D> textureDict, Dictionary<string, SpriteFont> fontDict) : base(contentManager, graphicsDevice, mainProgram) {
        Camera = new();
        HoverStrings = new();
        Texture_Dict = textureDict;
        Font_Dict = fontDict;
        //Loading in empty tile for the editor "right click to erase" function.
        EmptyTile = new Tile(Texture_Dict["EmptyTile"], "EmptyTile", false, 0, 0);

        //Button to goto import/export state, to build and export map file.
        var jumpToImportExport = new Button(Texture_Dict["button200x35"], new Vector2(-250, 0), Font_Dict["small"]) {
            Text = "Import/Export Menu",
        };

        jumpToImportExport.Click += JumpToImportExport_Click; 

        //Putting buttons into list for iteration.
        Components = new(){
            jumpToImportExport,
        };        
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        spriteBatch.Begin(transformMatrix: Camera.Transformation);

        //Collection Draws
        foreach (var tile in Tile_List) {
            tile.Draw(gameTime, spriteBatch);
        }
        foreach (var tile in Tile_Palette) {
            tile.Draw(gameTime,spriteBatch);
        }
        foreach (var component in Components) {
            component.Draw(gameTime, spriteBatch);
        }

        //Drawing Text
        spriteBatch.DrawString(Font_Dict["medium"], Grid_Dict["mapInfo"]["mapName"], new Vector2( -300, -200), Color.OrangeRed);

        //Drawing a mini HUD to the right of the mouse with pertinent Tile information.
        if (HoverStrings.Count > 0) {
            var yScroll = 0;
            foreach (var item in HoverStrings) {
                spriteBatch.DrawString(Font_Dict["small"], item, new Vector2(RelativeMousePos.X + 100 , RelativeMousePos.Y + yScroll), Color.White);
                yScroll += 25;
            }
        }

        spriteBatch.End();
        
    }

    public override void Update(GameTime gameTime) {
        //Timer iterations
        PlacementTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        //Keyboard updates
        if (Keyboard.GetState().IsKeyDown(Keys.F1) && _mainProgram.CurrentState is EditorActive && _mainProgram.StateSwitchCount > 1) {
            _mainProgram.StateSwitchCount = 0;
            _mainProgram.PreviousState = _mainProgram.CurrentState;
            _mainProgram.NextState = new HelpMenu(_content, _graphicsDevice, _mainProgram) {
                CurrentHelp = "editoractive", //Used for a switch in HelpState, Changes what is Displayed.
                FontDict = Font_Dict, //Passes FontDict to Help state, for Re-use
            };
        }

        //Mouse Updates
        RelativeMousePos = Vector2.Transform(new Vector2(CurrentMouse.X, CurrentMouse.Y), Matrix.Invert(Camera.Transformation));
        foreach (var component in Components) {               // Sending World Relative Coords to the buttons, 
            ((Button)component).Transform = RelativeMousePos; // to register the mouse rect properly.
        }
        HoverStrings.Clear();
        MouseUpdate();

        //Collection Updates
        foreach (var tile in Tile_List) {
            tile.Update(gameTime);
        }
        foreach (var tile in Tile_Palette) {
            tile.Update(gameTime);
        }
        foreach (var component in Components) {
            component.Update(gameTime);
        }
        
        //Camera Update
        Camera.Update(gameTime);
        Camera.CameraFollow(Camera);
    }

    //Deals with all mouse collision logic for the state
    private void MouseUpdate() {
        PreviousMouse = CurrentMouse;
        CurrentMouse = Mouse.GetState();

        // Getting Mouse Rect, and Dealing with Tile_List collisions
        var mouseRectangle = new Rectangle((int)RelativeMousePos.X , (int)RelativeMousePos.Y, 1, 1);

        foreach (var tile in Tile_List) {
            //"HoverStrings" HUD logic
            if (mouseRectangle.Intersects(tile.Rectangle)) {
                string line1 = "Texture Name = " + tile.TextureName; 
                string line2 = "Position = " + tile.Position.X.ToString() + " , " + tile.Position.Y.ToString();
                string line3 = "IsCollideable = " + tile.IsCollideable.ToString();
                HoverStrings.Add(line1); 
                HoverStrings.Add(line2); 
                HoverStrings.Add(line3);
                
            }
            // "Place" and "Erase" logic
            if (mouseRectangle.Intersects(tile.Rectangle)) {
                if (CurrentMouse.LeftButton == ButtonState.Pressed && PlacementTimer > .02f) {
                    PlacementTimer = 0f;
                    tile.ChangeTexture(CurrentTile.Texture, CurrentTile.IsCollideable, true, CurrentTile.TextureName);
                }
                if (CurrentMouse.RightButton == ButtonState.Pressed && PlacementTimer > .02f) {
                    PlacementTimer = 0f;
                    tile.ChangeTexture(EmptyTile.Texture, false, false, EmptyTile.TextureName);
                }
            }
        }   

        // Dealing with Tile_Palette collisions
        foreach (var tile in Tile_Palette) {
            if (mouseRectangle.Intersects(tile.Rectangle)) {
                if (CurrentMouse.LeftButton == ButtonState.Released && PreviousMouse.LeftButton == ButtonState.Pressed) {
                    CurrentTile = tile;
                }
            }
        }
    }


    /////////////////Button Methods///////////////////
   
   //Click to switch states to Import/Export
    private void JumpToImportExport_Click(object sender, EventArgs e) {
        if (_mainProgram.StateSwitchCount > 1) {
            _mainProgram.StateSwitchCount = 0;
            _mainProgram.PreviousState = _mainProgram.CurrentState;
            _mainProgram.NextState = new ExportxImport(_content, _graphicsDevice, _mainProgram, Texture_Dict, Font_Dict, Game_Textures) {
                GridDict = Grid_Dict,
                TileList = Tile_List,
                TilePalette = Tile_Palette,
            };
            
        }
    }

    ///////////End of Button Methods/////////////////////
}