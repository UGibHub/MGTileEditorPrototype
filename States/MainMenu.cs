using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileEditorPrototype;

public class MainMenu : State {
    private List<Component> _components; //List of components to draw
    private Button _inputReference; // A reference for the last button clicked, used to edit the buttons
                                    // text field in real time.                                 
    public Dictionary<string, Texture2D> TextureDict; // Textures for the program to pass around (Editor Specific)
    public Dictionary<string, SpriteFont> FontDict; // Fonts for the program to pass around
    public Dictionary<string, Dictionary<string, string>> GridDict; //String Based Reference. Will be used to output
                                                                    //Json after Edits complete, and load eventually.    
    public Dictionary<string, Texture2D> GameTextures; //Dict for game Textures (Game Specific).
    public List<Tile> TileList; // List of tiles, used for updates, and making the interface work in the editor screen
    public List<Tile> TilePalette; // Tile Selection for the editor.
    private bool _isGenerated = false; // Governs some DrawStrings showing, and is checked for Grid Generation. 
    
    
    public MainMenu(ContentManager contentManager, GraphicsDevice graphicsDevice, TileEditor mainProgram) : base (contentManager, graphicsDevice, mainProgram) {
        TextureDict = new Dictionary<string, Texture2D> () {
            { "background" , _content.Load<Texture2D>("TileEditorAssets/Decorative/EditorArtandIcon")},
            { "button100x35" , _content.Load<Texture2D>("TileEditorAssets/Buttons/WideButton100x35")},
            { "button200x35" , _content.Load<Texture2D>("TileEditorAssets/Buttons/WideButton200x35")},
            
            //The Below Tile will have to be reloaded per project. The PNG will have to be resized for whatever
            //tilesize I am using.
            { "EmptyTile" , _content.Load<Texture2D>("TileEditorAssets/Sprites/EmptyTile25x25")},
        };
        FontDict = new Dictionary<string, SpriteFont> () {
            {"small" , _content.Load<SpriteFont>("TileEditorAssets/Font/SmallFont")},
            {"medium" , _content.Load<SpriteFont>("TileEditorAssets/Font/MediumFont")},
            {"large" , _content.Load<SpriteFont>("TileEditorAssets/Font/LargeFont")},
        };

        //Builds all the textures for the Tilemap editor, the game specific ones. Here is where we will alter code
        //for the per project textures. Just make sure the folder structure is identical to the target game project.
        //Ignoring the "GameAssets" top folder of course. This will be used for auto-load of textures later, when applying
        //to the main project.
        //Loading textures to be used in the editor.
        GameTextures = new() {
            {"BasicTile", _content.Load<Texture2D>("GameAssets/Textures/Tiles/BasicTile25x25")},
        };

        //Will be used to generate a new tilemap once all the parameters are entered.
        var newProjectButton = new Button(TextureDict["button200x35"], new Vector2( 50, 400), FontDict["small"]) {
            Text = "Generate Tilemap",
        };

        newProjectButton.Click += NewProjectButton_Click;

        // Tile Width button
        var tileWidthInput = new Button(TextureDict["button100x35"], new Vector2(100, 100), FontDict["small"]) {
            Text = "",
            ValueID = "tileWidth",
            NumericalValue = true,
        };

        tileWidthInput.Click += TileWidthInput_Click;

        // Tile Height Button
        var tileHeightInput = new Button(TextureDict["button100x35"], new Vector2(100, 150), FontDict["small"]) {
            Text = "",
            ValueID = "tileHeight",
            NumericalValue = true,
        };

        tileHeightInput.Click += TileHeightInput_Click;

        //Grid Width Button
        var gridWidthInput = new Button(TextureDict["button100x35"], new Vector2(100, 200), FontDict["small"]) {
            Text = "",
            ValueID = "gridWidth",
            NumericalValue = true,
        };

        gridWidthInput.Click += GridWidthInput_Click;

        //Grid Height Button
        var gridHeightInput = new Button(TextureDict["button100x35"], new Vector2(100, 250), FontDict["small"]) {
            Text = "",
            ValueID = "gridHeight",
            NumericalValue = true,
        };

        gridHeightInput.Click += GridHeightInput_Click;

        //Map Name Button
        var mapNameInput = new Button(TextureDict["button200x35"], new Vector2(100, 300), FontDict["small"]) {
            Text = "",
            ValueID = "mapName",
            NumericalValue = false,
        };

        mapNameInput.Click += MapNameInput_Click;

        //Move to Editor State Button
        var moveToEditor = new Button(TextureDict["button200x35"], new Vector2(50, 500), FontDict["small"]) {
            Text = "Swap to Editor.",
            ValueID = "EditorScreen",
            NumericalValue = false,
        };

        moveToEditor.Click += MoveToEditor_Click;

        //Import/Export Button
        var importExportMove = new Button(TextureDict["button200x35"], new Vector2(50, 900), FontDict["small"]) {
            Text = "Import/Export Menu",
        };

        importExportMove.Click += ImportExportMove_Click;


        // Populating the _components list with the created button elements.
        _components = new(){
            newProjectButton,
            tileWidthInput,
            tileHeightInput,
            gridWidthInput,
            gridHeightInput,
            mapNameInput,
            moveToEditor,
            importExportMove,
        }; 
        
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        spriteBatch.Begin();
        
        //Decorative elements on screen.
        spriteBatch.Draw(TextureDict["background"], new Vector2(700, 200), Color.White);


        //Text On Screen
        spriteBatch.DrawString(FontDict["large"], "G's Tile Editor", new Vector2(625, 100), Color.DarkOliveGreen);
        spriteBatch.DrawString(FontDict["small"], "Press F1 anywhere to view a contextual help screen", new Vector2(1200, 250), Color.DarkOliveGreen);
        spriteBatch.DrawString(FontDict["small"], "Click fields to enter/edit Values (Press Enter to confirm.)", new Vector2(20, 50), Color.DarkOliveGreen);
        spriteBatch.DrawString(FontDict["small"], "Tile Width", new Vector2(210, 110), Color.DarkOliveGreen);
        spriteBatch.DrawString(FontDict["small"], "Tile Height", new Vector2(210, 160), Color.DarkOliveGreen);
        spriteBatch.DrawString(FontDict["small"], "Grid Width (In Tiles)", new Vector2(210, 210), Color.DarkOliveGreen);
        spriteBatch.DrawString(FontDict["small"], "Grid Height (In Tiles)", new Vector2(210, 260), Color.DarkOliveGreen);
        spriteBatch.DrawString(FontDict["small"], "Map Name", new Vector2(320, 310), Color.DarkOliveGreen);
        spriteBatch.DrawString(FontDict["small"], "Fill all fields, then click button to generate Tilemap", new Vector2(20, 360), Color.DarkOliveGreen);
        if (_isGenerated) {
            spriteBatch.DrawString(FontDict["small"], "Tilemap Ready!", new Vector2(300, 405), Color.OrangeRed);
            spriteBatch.DrawString(FontDict["small"], "<-- Click to Start Building!", new Vector2(300, 510), Color.OrangeRed);
        }
        
        //Button drawing
        foreach (var item in _components) {
            item.Draw(gameTime, spriteBatch);
        }

        spriteBatch.End();
    }

    

    public override void Update(GameTime gameTime) {
        //Help menu trigger. Each state should have one of these.
        if (Keyboard.GetState().IsKeyDown(Keys.F1) && _mainProgram.CurrentState is MainMenu && _mainProgram.StateSwitchCount > 1) {
            _mainProgram.StateSwitchCount = 0;
            _mainProgram.PreviousState = _mainProgram.CurrentState;
            _mainProgram.NextState = new HelpMenu(_content, _graphicsDevice, _mainProgram) {
                CurrentHelp = "mainmenu", //Used for a switch in HelpState, Changes what is Displayed.
                FontDict = FontDict, //Passes FontDict to Help state, for Re-use
            };
        }
        //Iterating Buttons for updates
        foreach (var item in _components) {
            item.Update(gameTime);
        }
        
    }
    
    /////////////Button Methods/////////////////

    // Will generate a new tilemap if all the appropriate fields are filled.
    private void NewProjectButton_Click(object sender, EventArgs e) {
        try {
            if (InputDict["tileWidth"] != "" && InputDict["tileHeight"] != "" && InputDict["gridWidth"] != "" &&
            InputDict["gridHeight"] != "" && InputDict["mapName"] != "" && !_isGenerated) {
                GridBuilder();
            }
        } catch { }
    }
    // Click to Input the tile width
    private void TileWidthInput_Click(object sender, EventArgs e) {
        _mainProgram.Window.TextInput += TextInputHandler;    
        _inputReference = (Button)sender;
    }
    //Click to input Tile Height
    private void TileHeightInput_Click(object sender, EventArgs e) {
        _mainProgram.Window.TextInput += TextInputHandler;    
        _inputReference = (Button)sender;
    }
    //Click to input Grid width
    private void GridWidthInput_Click(object sender, EventArgs e) {
        _mainProgram.Window.TextInput += TextInputHandler;    
        _inputReference = (Button)sender;
    }
    //Click to input Grid Height
    private void GridHeightInput_Click(object sender, EventArgs e) {
        _mainProgram.Window.TextInput += TextInputHandler;    
        _inputReference = (Button)sender;
    }
    //Click to input Map Name
    private void MapNameInput_Click(object sender, EventArgs e) {
        _mainProgram.Window.TextInput += TextInputHandler;    
        _inputReference = (Button)sender;
    }
    //Click to swap states to Editor
    private void MoveToEditor_Click(object sender, EventArgs e) {
        try {    
            if (TileList.Count > 0 && _mainProgram.StateSwitchCount > 1) {
                _mainProgram.StateSwitchCount = 0;
                _mainProgram.NextState = new EditorActive(_content, _graphicsDevice, _mainProgram, TextureDict, FontDict) {
                    Grid_Dict = GridDict,
                    Game_Textures = GameTextures,
                    Tile_List = TileList,
                    Tile_Palette = TilePalette,
                };
            }
        } catch {  }
    }
    //Click to Swap states to Import/Export
    private void ImportExportMove_Click(object sender, EventArgs e) {
        if (_mainProgram.StateSwitchCount > 1) {
            _mainProgram.StateSwitchCount = 0;
            _mainProgram.PreviousState = _mainProgram.CurrentState;
            _mainProgram.NextState = new ExportxImport(_content, _graphicsDevice, _mainProgram, TextureDict, FontDict, GameTextures) {};
        }
    }

    ////////////End of Button Methods////////////
    


    //Event handler for Keyboard based Text Input.
    private void TextInputHandler(object sender, TextInputEventArgs args) {
        var pressedKey = args.Key;
        var character = args.Character;
        if (pressedKey == Keys.Enter) {
            if (_inputReference.NumericalValue) {
                InputChecker();
            } else {
                InputDict[_inputReference.ValueID] = _inputReference.Text;
            }
            _mainProgram.Window.TextInput -= TextInputHandler;  
        } else if (pressedKey == Keys.Back) {
            if (_inputReference.Text.Length > 0) {
                string replace = _inputReference.Text.Remove(_inputReference.Text.Length - 1);
                _inputReference.Text = replace;
            }
        } else {
            _inputReference.Text += character;
        }
    }

    //If Button requires a numerical value input, check for numerical value, if rejected, clear string
    //if valid, add value to the InputDict (Used to calculate the dimensions of the new tilemap.)
    private void InputChecker() {
        try {
            int test = Convert.ToInt32(_inputReference.Text);
            InputDict[_inputReference.ValueID] = _inputReference.Text;
        } catch {
            _inputReference.Text = "";
        }
    }

    //Take all entered fields and builds both the grid reference dict, and the tile list, and populates both
    //with default values. Then fires the Build texture method.
    private void GridBuilder() {
        int tileWidth = Convert.ToInt32(InputDict["tileWidth"]);
        int tileHeight = Convert.ToInt32(InputDict["tileHeight"]);
        int gridWidth = Convert.ToInt32(InputDict["gridWidth"]);
        int gridHeight = Convert.ToInt32(InputDict["gridHeight"]);
        GridDict = new(); //Dictionary to contain string values for the map, and a key for mapInfo to use
                          //in exporting.
        TileList = new();

        GridDict["mapInfo"] = new Dictionary<string, string> {
            { "tileWidth", tileWidth.ToString()},
            { "tileHeight", tileHeight.ToString()},
            { "gridWidth", gridWidth.ToString()},
            { "gridHeight", gridHeight.ToString()},
            { "mapName", InputDict["mapName"] },
        };
        for (int i = 0; i < gridHeight; i++) {
            for (int j = 0; j < gridWidth; j++) {
                var tile = new Tile(TextureDict["EmptyTile"], "EmptyTile", false, j * tileWidth, i * tileHeight);
                TileList.Add(tile);
            }
        }
        _isGenerated = true; 
        BuildPalette();
    }


    
    private void BuildPalette() {
        

        // Assigning the GridDict Reference Keys/Values for FilePath manipulation upon loading into the main game
        // Omitting the "GameAssets" folder, which is only existant in the TileEditor Content folder.
        GridDict["tileProperties"] = new() {
            // "Tilenames" value should be the Key of each item in the GameTextures Dict. Separated by a space, if
            // more than one value. This will allow the level builder class to split them, and iterate them. Effectively
            // loading each texture needed. This is why I mirror the Content Folder structure for the editor, and
            // the current project. So I handle the content loading on both projects, but loading levels will load
            // textures on their own.
            { "TileNames" , "BasicTile "},
            { "BasicTilePath" , "Textures/Tiles/BasicTile25x25"},
            { "BasicTileIsCollideable" , "true"},
            { "BasicTilePaletteLocation" , "0,-50"},
        };

        //Loading my "Palette" for use in "Painting" in the Tile Editor screen.
        TilePalette = new List<Tile>() {
            new Tile(GameTextures["BasicTile"], "BasicTile", true, 0, -50),
        };
    }
}