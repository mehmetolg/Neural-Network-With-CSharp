using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MehmetOlgunYapaySinirAgi
{
    //**Mehmet Olgun  - 2019-10
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int girdiKatmaniNeuronCount = 0;
        List<int> araKatmanNeuronCount = new List<int>();
        int cikisKatmaniNeuronCount = 0;
        int epochSayisi = 0;
        public List<double> totalErrors = new List<double>();
        NeuralNetwork yapaySinirAgi = new NeuralNetwork();
        List<List<double>> egitimData = new List<List<double>>();
        List<List<double>> ciktiData = new List<List<double>>();
        private void Button1_Click(object sender, EventArgs e)
        {
            #region Önemsiz Kısım
            List<string> temp = new List<string>();
            temp.AddRange(textBox2.Text.Split(','));
            temp.ForEach(x => araKatmanNeuronCount.Add(Convert.ToInt32(x)));
            girdiKatmaniNeuronCount = Convert.ToInt32(textBox1.Text);
            cikisKatmaniNeuronCount = Convert.ToInt32(textBox3.Text);
            epochSayisi = Convert.ToInt32(textBox4.Text);
            #endregion


            yapaySinirAgi.epochCount = epochSayisi;
            yapaySinirAgi.AddNewLayer(new Layer((int)Sabitler.LayerTypes.GirdiKatmanı));
            for (int i = 0; i < araKatmanNeuronCount.Count; i++)
            {
                yapaySinirAgi.AddNewLayer(new Layer((int)Sabitler.LayerTypes.AraKatman));
            }
            yapaySinirAgi.AddNewLayer(new Layer((int)Sabitler.LayerTypes.CiktiKatmanı));

            yapaySinirAgi.layers.Where(x => x.layerTypeId == (int)Sabitler.LayerTypes.GirdiKatmanı).First().ManyAddNeuron(girdiKatmaniNeuronCount);
            List<Layer> arakatmanlar = new List<Layer>();
            arakatmanlar = yapaySinirAgi.layers.Where(x => x.layerTypeId == (int)Sabitler.LayerTypes.AraKatman).ToList();
            for (int i = 0; i < araKatmanNeuronCount.Count; i++)
            {
                arakatmanlar[i].ManyAddNeuron(araKatmanNeuronCount[i]);
            }
            yapaySinirAgi.layers.Where(x => x.layerTypeId == (int)Sabitler.LayerTypes.CiktiKatmanı).First().ManyAddNeuron(cikisKatmaniNeuronCount);


            var girdiNeurons = yapaySinirAgi.layers.Where(x => x.layerTypeId == (int)Sabitler.LayerTypes.GirdiKatmanı).First().neurons;
            foreach (var girdiNeuron in girdiNeurons)
            {
                girdiNeuron.AddNewManyWeight(1, true);
            }


            for (int i = 0; i < yapaySinirAgi.layers.Count; i++)
            {
                Layer tempLayer = yapaySinirAgi.layers[i];
                if (tempLayer.layerTypeId == (int)Sabitler.LayerTypes.AraKatman)
                {
                    List<Neuron> tempNeuronList = new List<Neuron>();
                    tempNeuronList = tempLayer.neurons;
                    foreach (var item in tempNeuronList)
                    {
                        item.AddNewManyWeight(yapaySinirAgi.layers[i - 1].neurons.Count, false);
                    }

                }
            }


            var ciktiNeurons = yapaySinirAgi.layers.Where(x => x.layerTypeId == (int)Sabitler.LayerTypes.CiktiKatmanı).First().neurons;
            foreach (var ciktiNeuron in ciktiNeurons)
            {
                ciktiNeuron.AddNewManyWeight(araKatmanNeuronCount[araKatmanNeuronCount.Count - 1], false);
            }


            yapaySinirAgi.YSAHataBul();
            yapaySinirAgi.totalErrors.ForEach(x => listBox1.Items.Add(x));

        }

        private void Button2_Click(object sender, EventArgs e)
        {
            double girdi1 = Convert.ToInt32(textBox5.Text);
            double girdi2 = Convert.ToInt32(textBox6.Text);
            double girdi3 = Convert.ToInt32(textBox7.Text);
            double girdi4 = Convert.ToInt32(textBox8.Text);
            List<double> testData = new List<double>() { girdi1, girdi2, girdi3, girdi4 };
            yapaySinirAgi.Test(testData);
            textBox9.Text = yapaySinirAgi.totalTestErrors[0].ToString();
            textBox10.Text = yapaySinirAgi.totalTestErrors[1].ToString();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            yapaySinirAgi = new NeuralNetwork();
            totalErrors = new List<double>();
            listBox1.Items.Clear();
            MessageBox.Show("Yapay Sinir Ağı Temizlendi", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Application.Restart();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            girdiKatmaniNeuronCount = Convert.ToInt32(textBox1.Text);
            cikisKatmaniNeuronCount = Convert.ToInt32(textBox3.Text);            
            OpenFileDialog file = new OpenFileDialog();
            file.ShowDialog();
            string dosyayolu = file.FileName;
            string[] allLines = File.ReadAllLines(dosyayolu);
            
            for (int i = 1; i < allLines.Length; i++)
            {
                List<double> temp = new List<double>();
                allLines[i].Split(';').ToList().ForEach(x => temp.Add(Convert.ToDouble(x)));
                egitimData.Add(new List<double>());
                ciktiData.Add(new List<double>());
                for (int j = 0; j < temp.Count; j++)
                {
                    if(j<girdiKatmaniNeuronCount)
                    {
                        egitimData[egitimData.Count - 1].Add(temp[j]);
                    }
                    else
                    {
                        ciktiData[ciktiData.Count - 1].Add(temp[j]);
                    }
                    
                }

            }
            yapaySinirAgi.egitimData = egitimData;
            yapaySinirAgi.ciktiData = ciktiData;
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
