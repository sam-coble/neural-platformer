using System;
using System.Numerics;
using Newtonsoft.Json.Linq;
namespace BrainGame
{
    public class Player
    {
        public Vector2 pos;
        public Vector2 vel;
        public Vector2 scale;
        public Player(JObject player)
        {
            this.pos = new Vector2(
                (float)((JObject)player.GetValue("pos")).GetValue("x"),
                (float)((JObject)player.GetValue("pos")).GetValue("y")
            );
            this.vel = new Vector2(
                (float)((JObject)player.GetValue("vel")).GetValue("x"),
                (float)((JObject)player.GetValue("vel")).GetValue("y")
            );
            this.scale = new Vector2(
                (float)((JObject)player.GetValue("scale")).GetValue("x"),
                (float)((JObject)player.GetValue("scale")).GetValue("y")
            );
        }
    }
}
