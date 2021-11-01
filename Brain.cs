using System;
using System.Collections;
using System.Collections.Generic;

namespace BrainGame
{
    public class Brain
    {
        public ArrayList nodes;
        private ArrayList inputIndeces;
        private int aCount;
        private ArrayList outputIndeces;
        private int cCount;
        private int bCount;
        public ArrayList weights;
        public Brain(ArrayList inputs, ArrayList outputs)
        {
            this.nodes = new ArrayList();
            this.inputIndeces = new ArrayList();
            int i = 0;
            this.aCount = 0;
            foreach(string val in inputs) {
                this.inputIndeces.Add(this.nodes.Count);
                this.nodes.Add(new Node(
                    val,
                    (double)0,
                    ((double)i) / ((double)inputs.Count),
                    (double)0,
                    "a" + this.aCount
                ));
                i++;
                this.aCount++;
            }
            this.outputIndeces = new ArrayList();
            i = 0;
            this.cCount = 0;
            foreach(string val in outputs)
            {
                this.outputIndeces.Add(this.nodes.Count);
                this.nodes.Add(new Node(
                    val,
                    (double)1,
                    ((double)i) / ((double)outputs.Count),
                    (double)0,
                    "c" + this.cCount
                ));
                i++;
                this.cCount++;
            }
            this.weights = new ArrayList();
            this.bCount = 0;
        }

        public int addInputNode(double y, string label)
        {
            return this.nodes.Add(new Node(
                label,
                (double)0,
                y,
                (double)0,
                "a" + this.aCount++
            ));
        }
        public int addOutPutNode(double y, string label)
        {
            return this.nodes.Add(new Node(
                label,
                (double)0,
                y,
                (double)0,
                "c" + this.cCount++
            )) - 1;
        }
        public int addHiddenNode(double x, double y, string label)
        {
            if (x <= 0 || x >= 1 || y < 0) throw new Exception("invalid pos");
            return this.nodes.Add(new Node(
                label,
                x,
                y,
                (double)0,
                "b" + this.bCount++
            ));
        }
        public void addWeightFromIndeces(int i, int j, double weight)
        {
            if( ((Node)this.nodes[j]).x <= ((Node)this.nodes[i]).x )
            {
                throw new Exception("invalid indeces: backwards");
            }
            if( i >= this.nodes.Count || j >= this.nodes.Count)
            {
                throw new Exception("index out of bounds");
            }
            this.weights.Add(new Weight(
                i,
                j,
                weight
            ));
        }
        public void addWeightFromIds(string id1, string id2, int weight)
        {
            int i = -1;
            for(int n = 0; n < this.nodes.Count; n++)
            {
                if(((Node)this.nodes[n]).id.Equals(id1))
                {
                    i = n;
                    break;
                }
            }
            int j = -1;
            for(int n = 0; n < this.nodes.Count; n++)
            {
                if(((Node)this.nodes[n]).id.Equals(id2))
                {
                    j = n;
                    break;
                }
            }
            if (i >= 0 && j >= 0)
            {
                this.addWeightFromIndeces(i, j, weight);
            } else {
                throw new Exception("invalid weight");
            }
        }
        public Weight removeWeightFromIndeces(int i, int j)
        {
            int index = -1;
            for(int n = 0; n < this.weights.Count; n++)
            {
                if(((Weight)this.weights[n]).i1 == i && ((Weight)this.weights[n]).i2 == j)
                {
                    index = n;
                    break;
                }
            }
            if(index >= 0)
            {
                Weight w = new Weight((Weight)this.weights[index]);
                this.weights.RemoveAt(index);
                return w;
            }
            else
            {
                throw new Exception("invalid weight");
            }
        }
        public void removeWeightFromids(int id1, int id2)
        {
            int i = -1;
            for (int n = 0; n < this.nodes.Count; n++)
            {
                if (((Node)this.nodes[n]).id.Equals(id1))
                {
                    i = n;
                    break;
                }
            }
            int j = -1;
            for (int n = 0; n < this.nodes.Count; n++)
            {
                if (((Node)this.nodes[n]).id.Equals(id2))
                {
                    j = n;
                    break;
                }
            }
            if(i >= 0 && j >= 0)
            {
                this.removeWeightFromIndeces(i, j);
            }
            else
            {
                throw new Exception("invalid ids");
            }
        }
        public Dictionary<string, bool> getOutputs(Dictionary<string, double> inputs)
        {
            foreach(Node node in nodes)
            {
                node.val = 0;
            }
            foreach(int i in inputIndeces)
            {
                ((Node)this.nodes[i]).val =
                    (double)inputs[((Node)this.nodes[i]).label];
            }
            // sort weights by starting index/pos
            this.weights.Sort(new WeightComparer(this.nodes));
            foreach(Weight weight in this.weights)
            {
                ((Node)this.nodes[weight.i2]).val +=
                    weight.weight * ((Node)this.nodes[weight.i1]).val;
            }
            Dictionary<string,bool> output = new Dictionary<string,bool>();
            foreach(int i in outputIndeces)
            {
                output[((Node)this.nodes[i]).label] =
                    ((Node)this.nodes[i]).val > 0;
            }
            return output;
        }
    }
}
