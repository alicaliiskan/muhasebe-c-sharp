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
    public partial class yonetici : Form
    {
        public yonetici()
        {
            InitializeComponent();
        }

        public void gelirHesap()
        {
            try
            {
                if(Form1.Baglanti.State == ConnectionState.Closed)
                {
                    Form1.Baglanti.Open();
                }

                DateTime baslangicTarihi = dateTimePicker1.Value;            
                DateTime bitisTarihi = dateTimePicker2.Value.Date.AddDays(1); 

                string sorgu = "SELECT toplam_tutar, kdv_orani FROM Faturalar WHERE tarih >= @baslangic AND tarih < @bitis";
                SqlCommand yukleKomut = new SqlCommand(sorgu, Form1.Baglanti);
                yukleKomut.Parameters.AddWithValue("@baslangic",baslangicTarihi);
                yukleKomut.Parameters.AddWithValue("@bitis", bitisTarihi);
                SqlDataReader dr = yukleKomut.ExecuteReader();
                double totalGelir = 0;
                int satisSayi = 0;
                double kdvTutar = 0;
                double kdv = 0;
                while (dr.Read())
                {
                    totalGelir += Convert.ToDouble(dr["toplam_tutar"]);
                    satisSayi++;
                    kdv = Convert.ToDouble(dr["toplam_tutar"]) * Convert.ToDouble(dr["kdv_orani"]) / (100 + Convert.ToDouble(dr["kdv_orani"]));
                    kdvTutar += kdv;
                }
                lblGelir.Text = totalGelir.ToString("N2");
                lblSatisSayisi.Text = satisSayi.ToString("N0");
                lblKDV.Text = kdvTutar.ToString("N2");
            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message, "gelirHesap adlı Method hatası");
            }
            finally {
                if (Form1.Baglanti.State == ConnectionState.Open)
                {
                    Form1.Baglanti.Close();
                }
            }
        }
        public void totalGider()
        {
            try
            {
                if (Form1.Baglanti.State == ConnectionState.Closed)
                {
                    Form1.Baglanti.Open();
                }

                DateTime baslangicTarihi = dateTimePicker1.Value;
                DateTime bitisTarihi = dateTimePicker2.Value.Date.AddDays(1);

                string sorgu = "SELECT tutar FROM Giderler WHERE tarih >= @baslangic AND tarih < @bitis";
                SqlCommand yukleKomut = new SqlCommand(sorgu, Form1.Baglanti);
                yukleKomut.Parameters.AddWithValue("@baslangic", baslangicTarihi);
                yukleKomut.Parameters.AddWithValue("@bitis", bitisTarihi);
                SqlDataReader dr = yukleKomut.ExecuteReader();
                double totalGider = 0;
                int GiderSayi = 0;
                while (dr.Read())
                {
                    totalGider += Convert.ToDouble(dr["tutar"]);
                    GiderSayi++;
                }
                lblGider.Text = totalGider.ToString("N2");
                lblGiderSayisi.Text = GiderSayi.ToString("N0");
            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message, "totalGider Method Hatası");
            }
            finally
            {
                if (Form1.Baglanti.State == ConnectionState.Open)
                {
                    Form1.Baglanti.Close();
                }
            }
        }
        private void yonetici_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Value = dateTimePicker1.Value.AddMonths(-1);
            gelirHesap();
            totalGider();
            double gelir = Convert.ToDouble(lblGelir.Text);
            double gider = Convert.ToDouble(lblGider.Text);
            double kdv = Convert.ToDouble(lblKDV.Text);
            lblKar.Text = (gelir - (gider+kdv)).ToString("N2");
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            lbldate.Text = DateTime.Now.ToLongDateString();
            lbldatetime.Text = DateTime.Now.ToLongTimeString();
        }
        private void btnPersonel_Click(object sender, EventArgs e)
        {
            PersonelForm PFrm = new PersonelForm();
            PFrm.ShowDialog();
        }
        private void btnMuhasebe_Click(object sender, EventArgs e)
        {
            MuhasebeForm mFrm = new MuhasebeForm();
            mFrm.Show();
        }
        private void btnFirmaData_Click(object sender, EventArgs e)
        {
            FirmaVerileri frmDAta = new FirmaVerileri();
            frmDAta.ShowDialog();
        }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            gelirHesap();
            totalGider();
            double gelir = Convert.ToDouble(lblGelir.Text);
            double gider = Convert.ToDouble(lblGider.Text);
            double kdv = Convert.ToDouble(lblKDV.Text);
            lblKar.Text = (gelir - (gider + kdv)).ToString("N2");
        }
        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            gelirHesap();
            totalGider();
            double gelir = Convert.ToDouble(lblGelir.Text);
            double gider = Convert.ToDouble(lblGider.Text);
            double kdv = Convert.ToDouble(lblKDV.Text);
            lblKar.Text = (gelir - (gider + kdv)).ToString("N2");
        }
    }
}
