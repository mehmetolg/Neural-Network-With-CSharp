using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MehmetOlgunYapaySinirAgi
{
    class Layer
    {
        public int layerTypeId = -1;
        public List<Neuron> neurons;
        public Layer(int layerTypeId)
        {
            neurons = new List<Neuron>();
            this.layerTypeId = layerTypeId;
        }
        public void AddNeuron(Neuron neuron)
        {
            neurons.Add(neuron);
        }

        public void ManyAddNeuron(int neuronCount)
        {
            for (int i = 0; i < neuronCount; i++)
            {
                neurons.Add(new Neuron()
                {
                    neuronTypeId=this.layerTypeId,
                    
                });
            }
        }
    }
}
