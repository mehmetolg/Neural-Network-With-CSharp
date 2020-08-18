using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MehmetOlgunYapaySinirAgi
{
    class Neuron
    {
       public List<double> weights;
        public double signal;
        public int neuronTypeId;
        public double hata;
        public double sigmoidHata;
        public List<double> weightChangeQuantity;
        public Neuron()
        {
            weightChangeQuantity = new List<double>();
            weights = new List<double>();          
        }
        public static readonly Random rnd = new Random();
        public static readonly Object kilit = new Object();
        public void AddNewManyWeight(int weightCount,bool inputNeuron)
        {
            
            for (int i = 0; i < weightCount; i++)
            {
                          
                if (inputNeuron)
                {
                    weights.Add(1);
                    weightChangeQuantity.Add(0);
                }
                else
                {                   
                    lock(kilit)
                    {
                        double newRandomWeight = Math.Round(rnd.NextDouble(), 8);
                        weights.Add(newRandomWeight);
                        weightChangeQuantity.Add(0);
                    }
                   
                }
                
            }
        }
        public void SetNeuronOutputSignal(List<double> signals)
        {
            double netSignal = 0;
            if (neuronTypeId == (int)Sabitler.LayerTypes.GirdiKatmanı)
                this.signal = signals[0];
            else
            {
                for (int i = 0; i < signals.Count; i++)
                {
                    netSignal += signals[i] * weights[i];
                }               
                this.signal = Math.Round(1 / (1 + (Math.Pow(Math.E, (-1 * netSignal)))),8);
            }
           
        }


    }
}
