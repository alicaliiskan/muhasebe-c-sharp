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
using System.Text.RegularExpressions;
namespace muhasebe
{
    public partial class login : Form
    {
        public login()
        {
            InitializeComponent();
        }
        public bool BoslukKontrol()
        {
            bool dondur = false;
            txtMail.BackColor = Color.FromArgb(40, 54, 85);
            txtSifre.BackColor = Color.FromArgb(40, 54, 85);
            if (txtMail.Text == "")
            {
                txtMail.BackColor = Color.Red;
               dondur =  true;
            }
            if (txtSifre.Text == "")
            {
                txtSifre.BackColor = Color.Red;
                dondur = true;
            }
            return dondur;
        }
        public bool MailKontrol()
        {
            txtMail.BackColor = Color.FromArgb(40, 54, 85);
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
           if(Regex.IsMatch(txtMail.Text, pattern)== false)
            {
                txtMail.BackColor = Color.Yellow;
                return true;
            }
            return false;
        }
        int sayac = 3;
        public void oturumAc()
        {
            try
            {
                Form1.BaglantiAc();
                string Sorgu = "SELECT PersonelID, PersonelMail, PersonelBirim,PersonelSifre, PersonelAd, PersonelSoyad FROM Personel WHERE PersonelMail = @mail";

                using (SqlCommand cmd = new SqlCommand(Sorgu, Form1.Baglanti))
                {
                    cmd.Parameters.AddWithValue("@mail", txtMail.Text);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        bool girisbasari = false;

                        if (reader.Read()) 
                        {
                            string personelID = reader["PersonelID"].ToString();
                            string personelMail = reader["PersonelMail"].ToString();
                            string personelSifre = reader["PersonelSifre"].ToString();
                            string personelBirim = reader["PersonelBirim"].ToString();
                            string personelAd = reader["PersonelAd"].ToString();
                            string personelSoyad = reader["PersonelSoyad"].ToString();
                            if (personelMail == txtMail.Text && personelSifre == txtSifre.Text)
                            {
                                girisbasari = true;
                                oturum.PersonelID = personelID;
                                oturum.Birim = personelBirim;
                                oturum.PersonelAdi= personelAd+" "+personelSoyad;

                               
                              
                                string birim = personelBirim.Trim().ToLower();
                                if (birim == "yönetici")
                                {
                                    MessageBox.Show("Giriş Başarılı", "Hoşgeldin "+ personelAd.Trim(), MessageBoxButtons.OK,MessageBoxIcon.Information);
                                    reader.Close(); 
                                    Form1.Baglanti.Close();
                                    yonetici ytFrm = new yonetici();
                                    this.Hide(); 
                                    ytFrm.ShowDialog(); 
                                    this.Close();
                                }
                                else if (birim == "muhasebe")
                                {
                                    MessageBox.Show("Muhasebe Giriş Yaptı");
                                    reader.Close();  
                                    Form1.Baglanti.Close();

                                    MuhasebeForm mFrm = new MuhasebeForm();
                                    this.Hide();
                                    mFrm.ShowDialog();
                                    this.Close();
                                }
                                else
                                {
                                    MessageBox.Show("Tüm personeller girer "+birim);
                                    reader.Close(); 
                                    Form1.Baglanti.Close();
                                }
                            }
                        }
                        else
                        {
                            girisbasari = false;
                        }

                        if (girisbasari == false)
                        {
                            sayac--; 
                            if (sayac > 0)
                            {
                                MessageBox.Show(sayac + " Deneme Kaldı", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                Application.Exit();
                            }
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message, "Giriş Kontrol Hata Penceresi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (Form1.Baglanti != null && Form1.Baglanti.State == ConnectionState.Open)
                {
                    Form1.Baglanti.Close();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(BoslukKontrol() == true)
            {
                MessageBox.Show("Lütfen Tüm Boşlukları Doldurun", "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                if (MailKontrol())
                {
                    MessageBox.Show("Lütfen Geçerli Bir Mail Adresi Girin", "geçersiz Mail", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else
                {
                    oturumAc();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
