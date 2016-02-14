using System;
using System.IO.Ports;  // necessário para ter acesso as portas
using System.Windows.Forms;

namespace ComunicadorSerial_Arduino
{
    public partial class USBConnectInterface : Form
    {
        public USBConnectInterface()
        {
            InitializeComponent();
            timerCOM.Enabled = true;
        }

        private void AtualizaListaCOMs()
        {
            var i = 0;

            var quantDiferente = false;

            //se a quantidade de portas mudou
            if (comboBox1.Items.Count == SerialPort.GetPortNames().Length)
            {
                for (var index = 0; index < SerialPort.GetPortNames().Length; index++)
                {
                    var s = SerialPort.GetPortNames()[index];
                    if (comboBox1.Items[i++].Equals(s) == false)
                    {
                        quantDiferente = true;
                    }
                }
            }
            else
            {
                quantDiferente = true;
            }

            //Se não foi detectado diferença
            if (quantDiferente == false)
            {
                return;                     //retorna
            }

            //limpa comboBox
            comboBox1.Items.Clear();

            //adiciona todas as COM diponíveis na lista
            foreach (string s in SerialPort.GetPortNames())
            {
                comboBox1.Items.Add(s);
                textBoxReceber.AppendText("Possivel Arduino encontrado na porta: " + s);
            }
            //seleciona a primeira posição da lista
            comboBox1.SelectedIndex = 0;
        }

        private void btConectar_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen == false)
            {
                try
                {
                    serialPort1.PortName = comboBox1.Items[comboBox1.SelectedIndex].ToString();
                    serialPort1.Open();
                }
                catch
                {
                    return;
                }
                if (!serialPort1.IsOpen) return;
                textBoxReceber.Clear();
                btConectar.Text = "Desconectar";
                comboBox1.Enabled = false;
            }
            else
            {
                try
                {
                    //serialPort1.Dispose();
                    serialPort1.Close();
                    comboBox1.Enabled = true;
                    btConectar.Text = "Conectar";
                }
                catch
                {
                    // ignored
                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (serialPort1.IsOpen)     // se porta aberta
                serialPort1.Dispose();
            serialPort1.Close();        	//fecha a porta
        }

        // Botão utilizado para enviar comandos para o Arduino
        private void btEnviar_Click(object sender, EventArgs e)
        {           
            SendDataToUsb(textBoxEnviar.Text);  //envia o texto presente no textbox
        }

        private void timerCOM_Tick_1(object sender, EventArgs e)
        {
            AtualizaListaCOMs();
        }

        public void SendDataToUsb(string massege)
        {
            if (serialPort1.IsOpen)
                serialPort1.Write(massege);
            else
            {
                serialPort1.PortName = comboBox1.Items[comboBox1.SelectedIndex].ToString();
                serialPort1.Open();
                serialPort1.Write(massege);
            }  
        }

        private void USBConnectInterface_Load(object sender, EventArgs e)
        {

        }
    }
}