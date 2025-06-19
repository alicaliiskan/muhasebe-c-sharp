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
    public partial class PersonelSil : Form
    {
        public PersonelSil()
        {
            InitializeComponent();
        }

        public void KayitSil()
        {
            try
            {
                Form1.BaglantiAc();

                string Sorgu = "DELETE FROM Personel WHERE PersonelID = @PersonelID";

                using (SqlCommand SilKomut = new SqlCommand(Sorgu, Form1.Baglanti))
                {
                    SilKomut.Parameters.AddWithValue("@PersonelID", Convert.ToInt32(PerID.Text));

                    int etkilenenSatirSayisi = SilKomut.ExecuteNonQuery();

                    if (etkilenenSatirSayisi > 0)
                    {
                        MessageBox.Show(PerID.Text + " numaralı Kayıt Silindi", "Kayıt Sil Form",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Kayıt silinemedi. Böyle bir kayıt bulunamadı.", "Uyarı",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message, "Kayıt sil hata penceresi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (Form1.Baglanti.State == ConnectionState.Open)
                {
                    Form1.Baglanti.Close();
                }
            }
    }
        private void btnkaydet_Click(object sender, EventArgs e)
        {
          DialogResult ynt = MessageBox.Show(txtAd.Text + " " + txtSoyad.Text + " işten çıkarılsın mı?", "Kayıt Siliniyor!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if(ynt == DialogResult.Yes)
            {
                KayitSil();
            }
        }
    }
}
