using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Xml.Linq;

using GeonBit.UI.Entities;
using GeonBit.UI.Entities.TextValidators;
using GeonBit.UI.DataTypes;
using System.Collections.Generic;
using GeonBit.UI;
using System.Linq;
using System;
using System.Text.RegularExpressions;

namespace GAME
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static void split(string tosplit)
        {
            string [] array  = tosplit.Split(',');
           
        }
        Tile[,] tileset;
       

        private Texture2D Textureback;
        private Vector2 Positionback;

        List<Panel> panels = new List<Panel>();

        // buttons to rotate examples
        Button nextExampleButton;
        Button nextExampleButton1;
        Button previousExampleButton;

        int currExample = 0;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            tileset = getTileset();
            // create and init the UI manager
            UserInterface.Initialize(Content, BuiltinThemes.hd);
            UserInterface.Active.UseRenderTarget = true;

            // draw cursor outside the render target
            UserInterface.Active.IncludeCursorInRenderTarget = false;
            // TODO: Add your initialization logic here

            base.Initialize();

            int _ScreenWidth = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            int _ScreenHeight = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            graphics.PreferredBackBufferWidth = (int)_ScreenWidth;
            graphics.PreferredBackBufferHeight = (int)_ScreenHeight;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            // init ui and examples
            InitExamplesAndUI();
        
    }

     
        public Tile[,] getTileset()
        {

            XDocument xDoc = XDocument.Load("Content/maps/map.tmx");
            int mapWidth = int.Parse(xDoc.Root.Attribute("width").Value);
            int mapHeight = int.Parse(xDoc.Root.Attribute("height").Value);
            int tilecount = int.Parse(xDoc.Root.Element("tileset").Attribute("tilecount").Value);
            int columns = int.Parse(xDoc.Root.Element("tileset").Attribute("columns").Value);



            var data = (from item in xDoc.Descendants("map")
                        select new
                        {
                            layerlist = item.Elements("layer")
                        }).Single();

            var bq = (from c in data.layerlist
                      select new
                      {
                          c.Element("data").Value,
                      }).ToArray();
            //foreach (var item in bq)
            //{
            //    string[] split = bq.ToString().Split(',');
            //}    
            
    
           
              string s0 = bq[0].Value.ToString();
              split(s0);  
              string[] splitArray = s0.Split(',');


            int[,] intIDs = new int[mapWidth, mapHeight];

            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    intIDs[x, y] = int.Parse(splitArray[x + y * mapWidth]);
                }
            }

            int key = 0;
            Vector2[] sourcePos = new Vector2[tilecount];
            for (int x = 0; x < tilecount / columns; x++)
            {
                for (int y = 0; y < columns; y++)
                {
                    sourcePos[key] = new Vector2(y * 16, x * 16);
                    key++;
                }
            }

            Texture2D sourceTex = Content.Load<Texture2D>("Images/Tileset");

            Tile[,] tiles = new Tile[mapWidth, mapHeight];
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    tiles[x, y] = new Tile
                        (
                        new Vector2(x * 16, y * 16),
                        sourceTex,
                        new Rectangle((int)sourcePos[intIDs[x, y]].X, (int)sourcePos[intIDs[x, y]].Y, 16, 16)
                        );
                }
            }
            return tiles;
        }
        protected void InitExamplesAndUI()
        {
            bool initExamples = true;
            
            // add previous example button
            previousExampleButton = new Button("Back", ButtonSkin.Default, Anchor.BottomCenter, new Vector2(300, 50));
            previousExampleButton.OnClick = (Entity btn) => { this.PreviousExample(); };
            UserInterface.Active.AddEntity(previousExampleButton);

            // add new scenario button
            nextExampleButton = new Button("New Scenario", ButtonSkin.Default, Anchor.TopCenter, new Vector2(300, 50));
            nextExampleButton.OnClick = (Entity btn) => { this.NextExample(); };

            // add new hero button
            nextExampleButton1 = new Button("Embark", ButtonSkin.Default, Anchor.BottomCenter, new Vector2(300, 50));
            nextExampleButton1.OnClick = (Entity btn) => { this.NextExample(); };
            spriteBatch.Begin();
            foreach (Tile t in tileset)
            {
                t.Draw(spriteBatch);
            }
            spriteBatch.End();

            // add exit button
            Button exitBtn = new Button("Exit", anchor: Anchor.BottomCenter, size: new Vector2(300, 50));
            exitBtn.OnClick = (Entity entity) => { Exit(); };
            

            if (initExamples)
            {
                { 
                    int PanelHeight = 400;
                    Panel Panel = new Panel(new Vector2(500, PanelHeight + 2), PanelSkin.Simple, Anchor.Center);
                    panels.Add(Panel);
                    UserInterface.Active.AddEntity(Panel);
                  
                    Panel.AddChild(nextExampleButton);
                    Panel.AddChild(exitBtn);
                    {
                    var btn = new Button("Credits", ButtonSkin.Default, Anchor.Center, new Vector2(300, 50));
                    btn.OnClick += (GeonBit.UI.Entities.Entity entity) =>
                        {
                            GeonBit.UI.Utils.MessageBox.ShowMsgBox("Hello World!", "This is a simple message box. It doesn't say much, really.");
                        };
                    Panel.AddChild(btn);
                    }
                }

                {
                    int panelWidth = 730;

                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(panelWidth, 550));
                    panels.Add(panel);

                    UserInterface.Active.AddEntity(panel);

                    // add title and text
                    panel.AddChild(new Header("Create New Character"));
                    panel.AddChild(new HorizontalLine());

                    // create an internal panel to align components better - a row that covers the entire width split into 3 columns (left, center, right)
                    // first the container panel
                    Panel entitiesGroup = new Panel(new Vector2(0, 240), PanelSkin.None, Anchor.AutoCenter);
                    entitiesGroup.Padding = Vector2.Zero;
                    panel.AddChild(entitiesGroup);

                    // now left side
                    Panel leftPanel = new Panel(new Vector2(0.33f, 0), PanelSkin.None, Anchor.TopLeft);
                    leftPanel.Padding = Vector2.Zero;
                    entitiesGroup.AddChild(leftPanel);

                    // right side
                    Panel rightPanel = new Panel(new Vector2(0.33f, 0), PanelSkin.None, Anchor.TopRight);
                    rightPanel.Padding = Vector2.Zero;
                    entitiesGroup.AddChild(rightPanel);

                    // center
                    Panel centerPanel = new Panel(new Vector2(0.33f, 0), PanelSkin.None, Anchor.TopCenter);
                    centerPanel.Padding = Vector2.Zero;
                    entitiesGroup.AddChild(centerPanel);

                    // create a character preview panel
                    centerPanel.AddChild(new Label(@"Preview", Anchor.AutoCenter));
                    Panel charPreviewPanel = new Panel(new Vector2(180, 180), PanelSkin.Simple, Anchor.AutoCenter);
                    charPreviewPanel.Padding = Vector2.Zero;
                    centerPanel.AddChild(charPreviewPanel);

                    // create preview pics of character
                    Image previewImage = new Image(Content.Load<Texture2D>("example/warrior"), Vector2.Zero, anchor: Anchor.Center);
                    Image previewImageColor = new Image(Content.Load<Texture2D>("example/warrior_color"), Vector2.Zero, anchor: Anchor.Center);
                    Image previewImageSkin = new Image(Content.Load<Texture2D>("example/warrior_skin"), Vector2.Zero, anchor: Anchor.Center);
                    charPreviewPanel.AddChild(previewImage);
                    charPreviewPanel.AddChild(previewImageColor);
                    charPreviewPanel.AddChild(previewImageSkin);

                    // add skin tone slider
                    Slider skin = new Slider(0, 10, new Vector2(0, -1), SliderSkin.Default, Anchor.Auto);
                    skin.OnValueChange = (Entity entity) =>
                    {
                        Slider slider = (Slider)entity;
                        int alpha = (int)(slider.GetValueAsPercent() * 255);
                        previewImageSkin.FillColor = new Color(60, 32, 25, alpha);
                    };
                    skin.Value = 5;
                    charPreviewPanel.AddChild(skin);

                    // create the class selection list
                    leftPanel.AddChild(new Label(@"Class", Anchor.AutoCenter));
                    SelectList classTypes = new SelectList(new Vector2(0, 208), Anchor.Auto);
                    classTypes.AddItem("Warrior");
                    classTypes.AddItem("Mage");
                    classTypes.AddItem("Ranger");
                    classTypes.AddItem("Monk");
                    classTypes.SelectedIndex = 0;
                    leftPanel.AddChild(classTypes);
                    classTypes.OnValueChange = (Entity entity) =>
                    {
                        string texture = ((SelectList)(entity)).SelectedValue.ToLower();
                        previewImage.Texture = Content.Load<Texture2D>("example/" + texture);
                        previewImageColor.Texture = Content.Load<Texture2D>("example/" + texture + "_color");
                        previewImageSkin.Texture = Content.Load<Texture2D>("example/" + texture + "_skin");
                    };

                    // create color selection buttons
                    rightPanel.AddChild(new Label(@"Color", Anchor.AutoCenter));
                    Color[] colors = { Color.White, Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Purple, Color.Cyan, Color.Brown };
                    int colorPickSize = 24;
                    foreach (Color baseColor in colors)
                    {
                        rightPanel.AddChild(new LineSpace(0));
                        for (int i = 0; i < 8; ++i)
                        {
                            Color color = baseColor * (1.0f - (i * 2 / 16.0f)); color.A = 255;
                            ColoredRectangle currColorButton = new ColoredRectangle(color, Vector2.One * colorPickSize, Anchor.AutoInline);
                            currColorButton.Padding = currColorButton.SpaceAfter = currColorButton.SpaceBefore = Vector2.Zero;
                            currColorButton.OnClick = (Entity entity) =>
                            {
                                previewImageColor.FillColor = entity.FillColor;
                            };
                            rightPanel.AddChild(currColorButton);
                        }
                    }
                    panel.AddChild(nextExampleButton1);

                    // add character name, last name, and age
                    // first add the labels
                    entitiesGroup.AddChild(new Label(@"Name: ", Anchor.AutoInline, size: new Vector2(0.4f, -1)));
                    // now add the text inputs

                    // first name
                    TextInput firstName = new TextInput(false, new Vector2(0.4f, -1), anchor: Anchor.Auto);
                    firstName.PlaceholderText = "Name";
                    firstName.Validators.Add(new TextValidatorEnglishCharsOnly(true));
                    firstName.Validators.Add(new OnlySingleSpaces());
                    firstName.Validators.Add(new TextValidatorMakeTitle());
                    entitiesGroup.AddChild(firstName);
                }
                {
                    int PanelHeight = 400;
                    Panel Panel = new Panel(new Vector2(500, PanelHeight + 2), PanelSkin.Simple, Anchor.BottomRight);
                    panels.Add(Panel);
                    UserInterface.Active.AddEntity(Panel);

                }
                // init panels and buttons
                UpdateAfterExapmleChange();

                }
                // call base initialize
                base.Initialize();
            }
        public void NextExample()
        {
            currExample++;
            UpdateAfterExapmleChange();
        }

        /// <summary>
        /// Show previous UI example.
        /// </summary>
        public void PreviousExample()
        {
            currExample--;
            UpdateAfterExapmleChange();
        }
        protected void UpdateAfterExapmleChange()
        {
            // hide all panels and show current example panel
            foreach (Panel panel in panels)
            {
                panel.Visible = false;
            }
            panels[currExample].Visible = true;

            // disable / enable next and previous buttons
            nextExampleButton.Disabled = currExample == panels.Count - 1;
            previousExampleButton.Disabled = currExample == 0;
        }
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Song song = Content.Load<Song>("Sound/menumusic"); 
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;

            Textureback = Content.Load<Texture2D>("Images/background");
            Positionback = new Vector2(0,-200);


            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // make sure window is focused
            if (!IsActive)
                return;

            // exit on escape
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // update UI
            UserInterface.Active.Update(gameTime);

            // call base update
            base.Update(gameTime);
        }

 

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            UserInterface.Active.Draw(spriteBatch);

            // clear buffer
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            spriteBatch.Draw(Textureback, Positionback, Color.White);

            spriteBatch.End();

            if (currExample == 2)
            {
                spriteBatch.Begin();
                foreach (Tile t in tileset)
                {
                    GraphicsDevice.Clear(Color.CornflowerBlue);   
                    t.Draw(spriteBatch);
                }
                spriteBatch.End();
            }


            // finalize ui rendering
            UserInterface.Active.DrawMainRenderTarget(spriteBatch);

      

            base.Draw(gameTime);
        }
    }
}
