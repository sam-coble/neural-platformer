using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Drawing;
using System.Windows;
namespace BrainGame
{
    public class ActiveSim
    {
        private string levelPath;
        public JObject level;
        public Player player;
        public Dictionary<string, double> constants;
        private bool onGround;
        public Brain brain;
        public Dictionary<string, bool> outputs;
        public ActiveSim(string levelPath, Dictionary<string, double> constants,
            ArrayList brainWeightids)
        {
            this.levelPath = levelPath;
            this.level = this.loadLevel();

            this.player = new Player((JObject)this.level.GetValue("player"));

            this.constants = constants;
            if (!this.constants.ContainsKey("xVel"))
                this.constants["xVel"] = 5;
            if (!this.constants.ContainsKey("grav"))
                this.constants["grav"] = 0.3;
            if (!this.constants.ContainsKey("jumpVel"))
                this.constants["jumpVel"] = 10;

            this.onGround = false;

            string[] inputs = new string[] {"xpos", "ypos", "bias"};
            string[] outputs = new string[] { "left", "right", "jump" };
            this.brain = new Brain(
                new ArrayList(inputs),
                new ArrayList(outputs));
            this.brain.addHiddenNode(0.5, 0, "");


            foreach(ArrayList weight in brainWeightids)
            {
                
                this.brain.addWeightFromIds(
                    (string)(weight[0]),(string)(weight[1]),(int)(weight[2]));
            }
        }
        private JObject loadLevel()
        {
            StreamReader r = new StreamReader(this.levelPath + ".json");
            string text = r.ReadToEnd();
            return JObject.Parse(text);
        }
        private bool collision(MyRect rect1, MyRect rect2)
        {
            return rect1.X < rect2.X + rect2.Width &&
                rect1.X + rect1.Width > rect2.X &&
                rect1.Y < rect2.Y + rect2.Height &&
                rect1.Y + rect1.Height > rect2.Y;
        }
        public void update()
        {
            Dictionary<string, double> inputs = new Dictionary<string, double>();
            inputs.Add("xpos", this.player.pos.X);
            inputs.Add("ypos", this.player.pos.Y);
            inputs.Add("bias", 1);

            this.outputs = this.brain.getOutputs(inputs);

            // apply horizontal vel
            if(this.onGround)
            {
                if(this.outputs["right"] && !this.outputs["left"])
                {
                    this.player.vel.X = (float)this.constants["xVel"];
                }
                else if(this.outputs["left"] && !this.outputs["right"])
                {
                    this.player.vel.X = (float)(-1 * this.constants["xVel"]);
                }
                else
                {
                    this.player.vel.X = 0;
                }
            }

            // apply horizontal pos
            this.player.pos.X += this.player.vel.X;

            // fix horizontal pos
            
            ArrayList solids = new ArrayList();
            foreach(KeyValuePair<string, JToken> piece in ((JObject)this.level
                .GetValue("solids")))
            {
                if(((string)((JObject)piece.Value).GetValue("type"))
                    .Equals("solid"))
                {
                    solids.Add(piece.Value);
                }
            }
            for(int i = 0; i < solids.Count; i++)
            {
                if (this.collision(
                    new MyRect(
                        this.player.pos.X,
                        this.player.pos.Y,
                        this.player.scale.X,
                        this.player.scale.Y
                    ),
                    new MyRect(
                        (double)((JObject)solids[i]).GetValue("x"),
                        (double)((JObject)solids[i]).GetValue("y"),
                        (double)((JObject)solids[i]).GetValue("w"),
                        (double)((JObject)solids[i]).GetValue("h")
                    )
                ))
                {
                    if(this.player.vel.X > 0)
                    {
                        this.player.pos.X =
                            ((float)(((JObject)solids[i]).GetValue("x")) -
                            this.player.scale.X);
                    }
                    else
                    {
                        this.player.pos.X =
                            ((float)(((JObject)solids[i]).GetValue("x")) +
                            (float)(((JObject)solids[i]).GetValue("w")));
                    }
                }
            }

            // apply vertical vel from grav
            this.player.vel.Y += (float)this.constants["grav"];

            //apply vertical pos
            this.player.pos.Y += this.player.vel.Y;

            // fix vertical pos
            this.onGround = false;
            for (int i = 0; i < solids.Count; i++)
            {
                if (this.collision(
                    new MyRect(
                        this.player.pos.X,
                        this.player.pos.Y,
                        this.player.scale.X,
                        this.player.scale.Y
                    ),
                    new MyRect(
                        (double)((JObject)solids[i]).GetValue("x"),
                        (double)((JObject)solids[i]).GetValue("y"),
                        (double)((JObject)solids[i]).GetValue("w"),
                        (double)((JObject)solids[i]).GetValue("h")
                    )
                ))
                {
                    if (this.player.vel.Y >= 0)
                    {
                        this.onGround = true;
                        this.player.pos.Y =
                            (float)(((JObject)solids[i]).GetValue("y")) - this.player.scale.Y;
                        this.player.vel.Y = 0;
                    }
                    else
                    {
                        this.player.pos.Y =
                            ((float)((JObject)solids[i]).GetValue("y")) +
                            ((float)((JObject)solids[i]).GetValue("h"));
                        this.player.vel.Y = 0;
                    }
                }
            }

            // apply vertical vel from jump if on ground
            if (this.outputs["jump"] && this.onGround)
            {
                this.player.vel.Y = -1 * (float)this.constants["jumpVel"];
            }
            
        }
    }
}
