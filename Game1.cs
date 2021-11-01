using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;

namespace BrainGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Texture2D blackRect;
        Texture2D whiteRect;
        Texture2D blackNode;
        Texture2D whiteNode;

        ActiveSim game;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 600;
            _graphics.PreferredBackBufferHeight = 400;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            Debug.WriteLine("Initialized");

            // init
            ArrayList brainWeightIds = new ArrayList();
            ArrayList brainWeightId0 = new ArrayList();
            brainWeightId0.Add("a0");
            brainWeightId0.Add("b0");
            brainWeightId0.Add(1);
            brainWeightIds.Add(brainWeightId0);
            ArrayList brainWeightId1 = new ArrayList();
            brainWeightId1.Add("a2");
            brainWeightId1.Add("b0");
            brainWeightId1.Add(-250);
            brainWeightIds.Add(brainWeightId1);
            ArrayList brainWeightId2 = new ArrayList();
            brainWeightId2.Add("b0");
            brainWeightId2.Add("c0");
            brainWeightId2.Add(1);
            brainWeightIds.Add(brainWeightId2);
            ArrayList brainWeightId3 = new ArrayList();
            brainWeightId3.Add("b0");
            brainWeightId3.Add("c1");
            brainWeightId3.Add(-1);
            brainWeightIds.Add(brainWeightId3);
            ArrayList brainWeightId4 = new ArrayList();
            brainWeightId4.Add("a2");
            brainWeightId4.Add("c2");
            brainWeightId4.Add(1);
            brainWeightIds.Add(brainWeightId4);


            this.game = new ActiveSim(
                "assets/levels/test",
                new Dictionary<string, double>(),
                brainWeightIds
            );
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            blackRect = Content.Load<Texture2D>("BlackRect");
            whiteRect = Content.Load<Texture2D>("WhiteRect");
            blackNode = Content.Load<Texture2D>("BlackNode");
            whiteNode = Content.Load<Texture2D>("WhiteNode");
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            this.game.update();


            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            DrawActiveSim(
                JObject.FromObject(game.player),
                JObject.FromObject(game.level),
                JObject.FromObject(game.brain)
            );
            _spriteBatch.End();
            base.Draw(gameTime);
        }
        private void DrawActiveSim(JObject playerData,
            JObject levelData, JObject networkData)
        {
            DrawPlayer(playerData);
            DrawLevel(levelData);
            DrawNetwork(networkData, 10, 10, 100, 100);
        }
        private void DrawPlayer(JObject player)
        {
            
            Rectangle playerCoords = new Rectangle(
                (int)((JObject)player.GetValue("pos")).GetValue("X"),
                (int)((JObject)player.GetValue("pos")).GetValue("Y"),
                (int)((JObject)player.GetValue("scale")).GetValue("X"),
                (int)((JObject)player.GetValue("scale")).GetValue("Y")
            );
            _spriteBatch.Draw(blackRect, playerCoords, Color.Chocolate);
        }
        private void DrawLevel(JObject level)
        {
            foreach(KeyValuePair<string,JToken> levelPiece in ((JObject)level.GetValue("solids")))
            {
                Rectangle pieceCoords = new Rectangle(
                    (int)((JObject)levelPiece.Value).
                        GetValue("x"),
                    (int)((JObject)levelPiece.Value).
                        GetValue("y"),
                    (int)((JObject)levelPiece.Value).
                        GetValue("w"),
                    (int)((JObject)levelPiece.Value).
                        GetValue("h")
                );
                _spriteBatch.Draw(blackRect, pieceCoords, Color.Chocolate);
            }
        }
        private void DrawNetwork(JObject brain, int x, int y, int w, int h)
        {
            foreach(JObject weight in
                (JArray)(brain.GetValue("weights")))
            {
                int strokeWidth = (int)Math.Floor(Math.Pow(Math.Abs(
                    (double)(weight.GetValue("weight"))),0.5));
                Color color = (double)(weight.GetValue("weight")) > 0 ?
                    Color.White : Color.Black;
                DrawLine(
                    new Vector2(
                        ((float)(((JObject)(((JArray)brain.GetValue("nodes"))
                            [(int)weight.GetValue("i1")]))
                            .GetValue("x")) * w) + x,
                        ((float)(((JObject)(((JArray)brain.GetValue("nodes"))
                            [(int)weight.GetValue("i1")]))
                            .GetValue("y")) * h) + y
                    ),
                    new Vector2(
                        ((float)(((JObject)(((JArray)brain.GetValue("nodes"))
                            [(int)weight.GetValue("i2")]))
                            .GetValue("x")) * w) + x,
                        ((float)(((JObject)(((JArray)brain.GetValue("nodes"))
                            [(int)weight.GetValue("i2")]))
                            .GetValue("y")) * h) + y
                    ),
                    strokeWidth,
                    color,
                    (double)(weight.GetValue("weight")) > 0
                );
            }
            foreach(JObject node in (JArray)(brain.GetValue("nodes")))
            {
                // -9 => radius of node
                // 18 => diameter => width/height
                Rectangle nodeCoords = new Rectangle(
                    (int)((((double)(node.GetValue("x"))) * w) + x - 9),
                    (int)((((double)(node.GetValue("y"))) * h) + y - 9),
                    18,
                    18
                );
                _spriteBatch.Draw(
                    (double)(node.GetValue("val")) > 0 ? whiteNode : blackNode,
                    nodeCoords,
                    Color.White
                );
            }
        }
        public void DrawLine(Vector2 start, Vector2 end, int width,
            Color color, bool isWhite)
        {
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);

            _spriteBatch.Draw(isWhite ? whiteRect : blackRect,
                new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), width),
                null,
                color,
                angle,
                new Vector2(0, 0),
                SpriteEffects.None,
                0);
        }
    }
}
