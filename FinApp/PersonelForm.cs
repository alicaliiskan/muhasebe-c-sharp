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
    public partial class PersonelForm : Form
    {
        public PersonelForm()
        {
            InitializeComponent();
        }
       
        public void PerListele()
        {
            try
            {
                Form1.BaglantiAc();
                DataSet ds = new DataSet();
               
                string Sorgu = "SELECT * FROM Personel";
                SqlDataAdapter da = new SqlDataAdapter(Sorgu, Form1.Baglanti);
                da.Fill(ds, "Personel");
                dataGridView1.DataSource = ds.Tables["Personel"];
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri çekme işlemi sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (Form1.Baglanti != null && Form1.Baglanti.State == ConnectionState.Open)
                {
                    Form1.Baglanti.Close();
                }
            }
        }
        private void PersonelForm_Load(object sender, EventArgs e)
        {
            PerListele();
        }

        private void btnPerEkle_Click(object sender, EventArgs e)
        {
            PersonelEkleForm PerEkleFrm = new PersonelEkleForm();
            PerEkleFrm.ShowDialog();
        }

        private void dgvGuncelle_Click(object sender, EventArgs e)
        {
            PerListele();
        }

        private void btnPerCikar_Click(object sender, EventArgs e)
        {
            PersonelSil perSil = new PersonelSil();

            perSil.PerID.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            perSil.txtAd.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            perSil.txtSoyad.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            perSil.txtTCNo.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            perSil.txtTelNo.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            perSil.txtMail.Text = dataGridView1.CurrentRow.Cells[5].Value.ToString();
            perSil.txtBirim.Text = dataGridView1.CurrentRow.Cells[6].Value.ToString();
            perSil.ShowDialog();
        }

        private void btnPerVeriGuncelle_Click(object sender, EventArgs e)
        {
            PersonelGuncelleForm pGncl =new PersonelGuncelleForm();
            pGncl.txtPerID.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            pGncl.txtAd.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            pGncl.txtSoyad.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            pGncl.txtTCNo.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            pGncl.txtTelNo.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            pGncl.txtMail.Text = dataGridView1.CurrentRow.Cells[5].Value.ToString();
            pGncl.cmbBirim.Text = dataGridView1.CurrentRow.Cells[6].Value.ToString();
            pGncl.ShowDialog();

        }
    }
}
