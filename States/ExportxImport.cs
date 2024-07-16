using System;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileEditorPrototype;
using System.IO;

public class ExportxImport : State {

     public Dictionary<string, Texture2D> TextureDict; // Textures for the program to pass around (EditorSpecific)
    public Dictionary<string, SpriteFont> FontDict; // Fonts for the program to pass around
    public Dictionary<string, Dictionary<string, string>> GridDict; // String base reference for the entire grid.
    public Dictionary<string, Texture2D> GameTextures; // Game Specfific textures
    public List<Tile> TileList; // List of all tiles on the grid
    public List<Tile> TilePalette; // List of one of each type of tile for "Painting"
    public List<Component> Components;// Buttons
    private Button _inputReference; // Used to update Button Text in real time,
    private bool _exportComplete; 
    private bool _importComplete;
    private string _exportFileName;
    private string _importFileName;
    private string _textureName;
    private bool _isCollide;

    public ExportxImport(ContentManager contentManager, GraphicsDevice graphicsDevice, TileEditor mainProgram, Dictionary<string, Texture2D> textureDict, Dictionary<string, SpriteFont> fontDict, Dictionary<string, Texture2D> gameTexture) : base(contentManager, graphicsDevice, mainProgram) {
        TextureDict = textureDict;
        FontDict = fontDict;
        GameTextures = gameTexture;

        //Button to Export Json Map File.
        var exportButton = new Button(TextureDict["button200x35"], new Vector2(600, 200), FontDict["small"]) {
            Text = "Export Json",
        };

        exportButton.Click += ExportButton_Click;
        //Button for Json Filename Input
        var exportJsonName = new Button(TextureDict["button200x35"], new Vector2(900, 200), FontDict["small"]) {
            Text = "",
            ValueID = "Export Text",
        };

        exportJsonName.Click += ExportJsonName_Click;
        //Button to Export Json Map File.
        var importButton = new Button(TextureDict["button200x35"], new Vector2(600, 400), FontDict["small"]) {
            Text = "Import Json",
        };

        importButton.Click += ImportButton_Click;
        //Button for Json Filename Input
        var importJsonName = new Button(TextureDict["button200x35"], new Vector2(900, 400), FontDict["small"]) {
            Text = "",
            ValueID = "Import Text",
        };

        importJsonName.Click += ImportJsonName_Click;
        //To Editor Button
        var toEditorButton = new Button(TextureDict["button200x35"], new Vector2(600, 600), FontDict["small"]) {
            Text = "To Grid Editor ",
        };

        toEditorButton.Click += ToEditorButton_Click;

        //Populating Button List
        Components = new() {
            exportButton,
            exportJsonName,
            importButton,
            importJsonName,
            toEditorButton,
        };
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        spriteBatch.Begin();

        //Text Draw
        spriteBatch.DrawString(FontDict["large"], "Import/Export Menu", new Vector2(550 , 25), Color.DarkOliveGreen);
        spriteBatch.DrawString(FontDict["small"], "All imports/exports are contained within the JsonLevels folder in the project, Only enter file names please, no type extensions.", new Vector2(250 , 100), Color.OrangeRed);
        spriteBatch.DrawString(FontDict["small"], "File Name To Export ", new Vector2(900 , 170), Color.DarkOliveGreen);
        spriteBatch.DrawString(FontDict["small"], "File Name To Import ", new Vector2(900 , 370), Color.DarkOliveGreen);
        if (_exportComplete) {
            spriteBatch.DrawString(FontDict["medium"], "Export Complete!", new Vector2(1150 , 195), Color.OrangeRed);
        }
        if (_importComplete) {
            spriteBatch.DrawString(FontDict["medium"], "Import Complete!", new Vector2(1150 , 395), Color.OrangeRed);
        }

        //Button Draw
        foreach (var component in Components) {
            component.Draw(gameTime, spriteBatch);
        }


        spriteBatch.End();
    }

    public override void Update(GameTime gameTime) {

         //Keyboard updates
        if (Keyboard.GetState().IsKeyDown(Keys.F1) && _mainProgram.CurrentState is ExportxImport && _mainProgram.StateSwitchCount > 1) {
            _mainProgram.StateSwitchCount = 0;
            _mainProgram.StoredState = _mainProgram.PreviousState;
            _mainProgram.PreviousState = _mainProgram.CurrentState;
            _mainProgram.StoringState = true;
            _mainProgram.NextState = new HelpMenu(_content, _graphicsDevice, _mainProgram) {
                CurrentHelp = "exportximport", //Used for a switch in HelpState, Changes what is Displayed.
                FontDict = FontDict, //Passes FontDict to Help state, for Re-use
            };
        }

        //Button Updates
        foreach (var component in Components) {
            component.Update(gameTime);
        }
        
    }

//////////Button Methods//////////////

    // Click to Export Json File
    private void ExportButton_Click(object sender, EventArgs e) {
        bool textCheck = false;
        foreach (var component in Components) {
            if (((Button)component).ValueID == "Export Text") {
                if (((Button)component).Text != "") {
                    _exportFileName = ((Button)component).Text;
                    textCheck = true;
                }
            }
        }
        if (textCheck) {
            CompileAndExport(_exportFileName);
            _exportComplete = true;
        }    
    }
    // Click to Input Name for Json Export
    private void ExportJsonName_Click(object sender, EventArgs e) {
        _mainProgram.Window.TextInput += TextInputHandler;
        _inputReference = (Button)sender;
    }
    // Click to Import Json File
    private void ImportButton_Click(object sender, EventArgs e) {
        bool textCheck = false;
        foreach (var component in Components) {
            if (((Button)component).ValueID == "Import Text") {
                if (((Button)component).Text != "") {
                    _importFileName = ((Button)component).Text;
                    textCheck = true;
                }
            }
        }
        if (textCheck) {
            try {
                var ImportJson = File.ReadAllText("JsonLevels/" + _importFileName + ".json");
                GridDict = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string,string>>>(ImportJson);
                _importComplete = true;
                ImportAndLoad();
            } catch {  }
        }    
    }
    // Click to Input Name for Json Import
    private void ImportJsonName_Click(object sender, EventArgs e) {
        _mainProgram.Window.TextInput += TextInputHandler;
        _inputReference = (Button)sender;
    }
    // Click to return to Grid Editor
    private void ToEditorButton_Click(object sender, EventArgs e) {
        if (_mainProgram.PreviousState is EditorActive) {
            _mainProgram.NextState = _mainProgram.PreviousState;
        } else if (TileList.Count > 0 && TilePalette.Count > 0) {
            _mainProgram.NextState = new EditorActive(_content, _graphicsDevice, _mainProgram, TextureDict, FontDict) {
                Tile_List = TileList,
                Tile_Palette = TilePalette,
                Grid_Dict = GridDict,
            };
        }
    }

/////////End of Button Methods////////  

    //Handles text input for buttons.
    private void TextInputHandler(object sender, TextInputEventArgs args) {
        var pressedKey = args.Key;
        var character = args.Character;
        if (pressedKey == Keys.Enter) {
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

    //Handles Exporting of Json File
    private void CompileAndExport(string name) {
        foreach (var tile in TileList) {
            if (tile.IsUsed) {
                GridDict[tile.Position.X.ToString() + "," + tile.Position.Y.ToString()] = new Dictionary<string, string> {
                     { "Texture" , tile.TextureName },
                     { "IsCollideable", tile.IsCollideable.ToString().ToLower() },
                     { "X" , tile.Position.X.ToString() },
                     { "Y" , tile.Position.Y.ToString() },
                     };
            }
        }

        var JsonExport = JsonSerializer.Serialize(GridDict);
        File.WriteAllText("JsonLevels/" + name + ".json", JsonExport);
    }

    //Loading the Specified Json File
    private void ImportAndLoad() {
        //Building the Default tile list, to the Json map specifications.
        TileList = new();
        int tileWidth = Convert.ToInt32(GridDict["mapInfo"]["tileWidth"]);
        int tileHeight = Convert.ToInt32(GridDict["mapInfo"]["tileHeight"]);
        int gridWidth = Convert.ToInt32(GridDict["mapInfo"]["gridWidth"]);
        int gridHeight = Convert.ToInt32(GridDict["mapInfo"]["gridHeight"]);
        for (int i = 0; i < gridHeight; i++) {
            for (int j = 0; j < gridWidth; j++) {
                var tile = new Tile(TextureDict["EmptyTile"], "EmptyTile", false, j * tileWidth, i * tileHeight);
                TileList.Add(tile);
            }
        }
        //Loading the Tile Palette
        TilePalette = new();
        var nameArray = GridDict["tileProperties"]["TileNames"].Split();
        foreach (var item in nameArray) {
            if (item != "") {
                var xYArray = GridDict["tileProperties"][item + "PaletteLocation"].Split(",");
                Tile tile = new(GameTextures[item], item, Convert.ToBoolean(GridDict["tileProperties"][item + "IsCollideable"]), 
                Convert.ToInt32(xYArray[0]), Convert.ToInt32(xYArray[1]));
                TilePalette.Add(tile);
            }
        }
        //Taking all the active Tiles in the Json map, and swapping the tiles on our default grid to them.
        foreach (var (key, value) in GridDict) {
            if (key == "mapInfo" || key == "tileProperties") {
                continue;
            }
            var xYArray = key.Split(",");
            foreach (var (key02, val02) in value) {
                switch (key02) {
                    case "Texture":
                        _textureName = val02;
                        break;
                    case "IsCollideable":
                        _isCollide = Convert.ToBoolean(val02);
                        break;
                    default:
                        break;
                }
            }
            foreach (var tile in TileList){
                if (tile.Position.X == Convert.ToInt32(xYArray[0]) && tile.Position.Y == Convert.ToInt32(xYArray[1])) {
                    tile.ChangeTexture(GameTextures[_textureName], _isCollide, true, _textureName);
                }
            }

        }
    }
}