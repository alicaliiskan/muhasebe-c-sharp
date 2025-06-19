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
using System.Security.Cryptography;
using System.Text.RegularExpressions;
namespace muhasebe
{
    public partial class PersonelGuncelleForm : Form
    {
        public PersonelGuncelleForm()
        {
            InitializeComponent();
        }
        public bool MailKontrol()
        {
            txtMail.BackColor = Color.White;
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (Regex.IsMatch(txtMail.Text, pattern) == false)
            {
                txtMail.BackColor = Color.Yellow;
                return true;
            }
            return false;
        }
        public bool TCKimlikNoDogrula(string tcNo)
        {
            if (string.IsNullOrEmpty(tcNo) || tcNo.Length != 11)
            {
                return false;
            }
            foreach (char c in tcNo)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            if (tcNo[0] == '0')
            {
                return false;
            }
            int[] digits = new int[11];
            for (int i = 0; i < 11; i++)
            {
                digits[i] = int.Parse(tcNo[i].ToString());
            }
            int teklerToplami = digits[0] + digits[2] + digits[4] + digits[6] + digits[8];
            int ciftlerToplami = digits[1] + digits[3] + digits[5] + digits[7];

            int onuncuHane = (teklerToplami * 7 - ciftlerToplami) % 10;
            if (onuncuHane != digits[9])
            {
                return false;
            }
            int ilkOnToplam = 0;
            for (int i = 0; i < 10; i++)
            {
                ilkOnToplam += digits[i];
            }

            if (ilkOnToplam % 10 != digits[10])
            {
                return false;
            }
            return true;
        }
        public bool PersonelGuncelle()
        {
            PersonelGuncelleForm form = new PersonelGuncelleForm();
            try
            {
                Form1.BaglantiAc();

                string query = "UPDATE Personel SET " +
                               "PersonelAd = '" + txtAd.Text + "', " +
                               "PersonelSoyad = '" + txtSoyad.Text + "', " +
                               "PersonelTCNo = '" + txtTCNo.Text + "', " +
                               "PersonelTel = '" + txtTelNo.Text + "', " +
                               "PersonelMail = '" + txtMail.Text + "', " +
                               "PersonelBirim = '" + cmbBirim.Text + "' " +
                               "WHERE PersonelID = " + txtPerID.Text;

                SqlCommand cmd = new SqlCommand(query, Form1.Baglanti);

                int etkilenenSatirSayisi = cmd.ExecuteNonQuery();

                if (etkilenenSatirSayisi > 0)
                {
                    MessageBox.Show("Personel bilgileri güncellendi.", "Bilgi",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }
                else
                {
                    MessageBox.Show("Güncelleme yapılamadı.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                Form1.Baglanti.Close();
            }
        }
    
        private void btnkaydet_Click(object sender, EventArgs e)
        {
            string tcKimlik = txtTCNo.Text.Trim();
            if (!TCKimlikNoDogrula(tcKimlik))
            {
                txtTCNo.BackColor = Color.FromArgb(255, 215, 0);
                txtTCNo.Focus();
                MessageBox.Show("TC Kimlik Numarası geçerli değildir!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtTCNo.SelectAll();
            }
            else
            {
                if (MailKontrol())
                {
                    MessageBox.Show("Lütfen Geçerli Bir Mail Adresi Girin", "geçersiz Mail", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else
                {
                    PersonelGuncelle();
                }
            }
        }
    }
}
