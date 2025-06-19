using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace muhasebe
{
    public partial class Form1 : Form
    {

       
        public Form1()
        {
          
            ConfigHelper.LoadConfig();

            if (string.IsNullOrEmpty(ConfigHelper.Config.uygulamaSifre) || string.IsNullOrEmpty(ConfigHelper.Config.EPosta) || string.IsNullOrEmpty(ConfigHelper.Config.smtpHost) || string.IsNullOrEmpty(ConfigHelper.Config.uygulamaAdi))
            {
                MessageBox.Show("Kurulum yapmanız gerekiyor!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FormSetup setupForm = new FormSetup(); 
                setupForm.ShowDialog(); 
         
               
            }
            InitializeComponent();

        }


        public static SqlConnection Baglanti = new SqlConnection("bağlantı cümleciği");
       
        public static void BaglantiAc()
        {
            try
            {
                    if(Baglanti.State == ConnectionState.Closed)
                {
                    Baglanti.Open();
                }
            }
            catch (Exception hata)
            {
                MessageBox.Show(hata.Message, "Veritabanı Bağlantısı Kurulamadı",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (progressBar1.Value < 100)
            {
                progressBar1.Value++;
            }
            else
            {
                timer1.Enabled = false;
                login lgn = new login();
                this.Hide();
                lgn.ShowDialog();
                this.Close();

            }
        }
    }
}
