using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MehmetOlgunYapaySinirAgi
{
    class NeuralNetwork
    {
        public List<List<double>> egitimData;
        public List<List<double>> ciktiData;
        public List<double> totalErrors;
        public List<double> totalTestErrors;
        public List<Layer> layers;
        public double ogrenmeKatsayisi;
        public double momentum;
        public int epochCount;


        public NeuralNetwork()
        {
            epochCount = 0;
            ogrenmeKatsayisi = 0.1;
            momentum = 0.8;
            layers = new List<Layer>();
            totalErrors = new List<double>();
            totalTestErrors = new List<double>();
            egitimData = new List<List<double>>();
            ciktiData = new List<List<double>>();

        }



        public void AddNewLayer(Layer layer)
        {
            layers.Add(layer);
        }


        public void YSAHataBul()
        {
            for (int ep = 0; ep < epochCount; ep++)
            {
                for (int e = 0; e < egitimData.Count; e++)
                {
                    List<double> veri = egitimData[e];
                    for (int i = 0; i < layers.Count; i++)
                    {
                        for (int j = 0; j < layers[i].neurons.Count; j++)
                        {
                            Neuron tempNeuron = layers[i].neurons[j];
                            if (tempNeuron.neuronTypeId == (int)Sabitler.LayerTypes.GirdiKatmanı)
                            {
                                tempNeuron.SetNeuronOutputSignal(new List<double>() { veri[j] });
                            }
                            else
                            {
                                List<double> beforeLayerSignals = new List<double>();
                                beforeLayerSignals.AddRange(layers[i - 1].neurons.Select(x => x.signal).ToList());
                                tempNeuron.SetNeuronOutputSignal(beforeLayerSignals);
                            }
                        }
                    }
                    for (int i = layers.Count - 1; i > 0; i--)
                    {
                        List<double> ciktiVeri = ciktiData[e];
                        for (int j = 0; j < layers[i].neurons.Count; j++)
                        {

                            Neuron tempNeuron = layers[i].neurons[j];
                            if (tempNeuron.neuronTypeId == (int)Sabitler.LayerTypes.CiktiKatmanı)
                            {
                                tempNeuron.hata = Math.Round((ciktiVeri[j] - tempNeuron.signal), 8);
                                tempNeuron.sigmoidHata = Math.Round((tempNeuron.signal * (1 - tempNeuron.signal) * tempNeuron.hata), 8);
                            }
                            else if (tempNeuron.neuronTypeId == (int)Sabitler.LayerTypes.AraKatman)
                            {
                                List<double> afterLayerNeuronsSigmoidHata = new List<double>();
                                afterLayerNeuronsSigmoidHata.AddRange(layers[i + 1].neurons.Select(x => x.sigmoidHata).ToList());
                                List<double> afterLayerNeuronsWeights = new List<double>();
                                afterLayerNeuronsWeights.AddRange(layers[i + 1].neurons.Select(x => x.weights[j]).ToList());
                                double subProcess = 0;
                                for (int m = 0; m < afterLayerNeuronsSigmoidHata.Count; m++)
                                {
                                    subProcess += Math.Round((afterLayerNeuronsSigmoidHata[m] * afterLayerNeuronsWeights[m]), 8);
                                }

                                tempNeuron.sigmoidHata = Math.Round((tempNeuron.signal * (1 - tempNeuron.signal) * Math.Round(subProcess, 8)), 8);
                            }

                        }

                    }
                    for (int i = layers.Count - 1; i > 0; i--)
                    {
                        List<double> beforeLayerNeuronsSignals = new List<double>();
                        beforeLayerNeuronsSignals.AddRange(layers[i - 1].neurons.Select(x => x.signal).ToList());
                        for (int j = 0; j < layers[i].neurons.Count; j++)
                        {
                            Neuron tempNeuron = layers[i].neurons[j];
                            for (int w = 0; w < tempNeuron.weights.Count; w++)
                            {
                                tempNeuron.weightChangeQuantity[w] = Math.Round((ogrenmeKatsayisi * tempNeuron.sigmoidHata * beforeLayerNeuronsSignals[w] + (momentum * tempNeuron.weightChangeQuantity[w])), 8);
                                tempNeuron.weights[w] += tempNeuron.weightChangeQuantity[w];
                            }

                        }
                    }

                    double totalError = layers.Where(x => x.layerTypeId == (int)Sabitler.LayerTypes.CiktiKatmanı).First().neurons.Sum(x => Math.Round(Math.Pow(x.hata, 2), 8));
                    totalErrors.Add(totalError/2);


                }

            }

        }

        public void Test(List<double> testData)
        {
            totalTestErrors.Clear();
            for (int i = 0; i < layers.Count; i++)
            {
                for (int j = 0; j < layers[i].neurons.Count; j++)
                {
                    Neuron tempNeuron = layers[i].neurons[j];
                    if (tempNeuron.neuronTypeId == (int)Sabitler.LayerTypes.GirdiKatmanı)
                    {
                        tempNeuron.SetNeuronOutputSignal(new List<double>() { testData[j] });
                    }
                    else
                    {
                        List<double> beforeLayerSignals = new List<double>();
                        beforeLayerSignals.AddRange(layers[i - 1].neurons.Select(x => x.signal).ToList());
                        tempNeuron.SetNeuronOutputSignal(beforeLayerSignals);
                        if (tempNeuron.neuronTypeId == (int)Sabitler.LayerTypes.CiktiKatmanı)
                            totalTestErrors.Add(tempNeuron.signal);
                    }
                }
            }
            

        }


    }
}
