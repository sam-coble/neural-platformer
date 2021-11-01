using System;
using System.Collections;
using System.Collections.Generic;
namespace BrainGame
{
    public class Weight
    {
        public int i1;
        public int i2;
        public double weight;
        public Weight(int i1, int i2, double weight)
        {
            this.i1 = i1;
            this.i2 = i2;
            this.weight = weight;
        }
        public Weight(Weight w)
        {
            this.i1 = w.i1;
            this.i2 = w.i2;
            this.weight = w.weight;
        }
    }
    public class WeightComparer: IComparer
    {
        private ArrayList nodes;
        public WeightComparer(ArrayList nodes)
        {
            this.nodes = nodes;
        }
        int IComparer.Compare(Object a, Object b)
        {
            return Comparer<double>.Default.Compare(
                ((Node)this.nodes[((Weight)a).i1]).x,
                ((Node)this.nodes[((Weight)b).i1]).x
            );
        }
    }
}
