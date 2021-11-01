using System;
namespace BrainGame
{
    public class Node
    {
        public string label;
        public double x;
        public double y;
        public double val;
        public string id;

        public Node(string label, double x, double y, double val, string id)
        {
            this.label = label;
            this.x = x;
            this.y = y;
            this.val = val;
            this.id = id;
        }
    }
}
