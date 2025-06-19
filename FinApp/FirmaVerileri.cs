using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace muhasebe
{
    public partial class FirmaVerileri : Form
    {
        public FirmaVerileri()
        {
            InitializeComponent();
        }



    private void FirmaVerileri_Load(object sender, EventArgs e)
        {
            try
            {
                var (firmaAdi, firmaAdres, firmaVergiNo) = FirmaConfigApp.GetConfigData();
                txtFirmaAdi.Text = firmaAdi;
                txtFirmaAdres.Text = firmaAdres;
                txtFirmaVergiNo.Text = firmaVergiNo;

      }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FirmaConfigApp.UpdateConfigFile(txtFirmaAdi.Text, txtFirmaAdres.Text, txtFirmaVergiNo.Text);
            try
            {
                var (firmaAdi, firmaAdres, firmaVergiNo) = FirmaConfigApp.GetConfigData();
                txtFirmaAdi.Text = firmaAdi;
                txtFirmaAdres.Text = firmaAdres;
                txtFirmaVergiNo.Text = firmaVergiNo;

      }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
