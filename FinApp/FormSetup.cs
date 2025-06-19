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
    public partial class FormSetup : Form
    {
        public FormSetup()
        {
            InitializeComponent();
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            ConfigHelper.Config.uygulamaSifre = txtUygulamaSifre.Text;
            ConfigHelper.Config.EPosta = txtEPosta.Text;
            ConfigHelper.Config.uygulamaAdi = txtUygulamaAdi.Text;
            ConfigHelper.Config.smtpHost = txtSmptHost.Text;

            ConfigHelper.SaveConfig();

            MessageBox.Show("Kurulum tamamlandı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.Close(); 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Mail gönderimi yapacak mail adresini ilgili alana ekleyiniz.","E Posta Alanı İçin Bilgi Penceresi",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Marka adınızı ilgili alana giriniz.", "Marka Alanı İçin Bilgi Penceresi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Eğer ki gmail ile gönderim istiyorsanız varsayılanı değiştirmeyiniz.", "smtp sunucu bilgi penceresi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Gmail hesabınızla Google hesap yönetimine gidin: https://myaccount.google.com/");
            MessageBox.Show("Sol menüden 'Güvenlik' seçeneğine tıklayın");
            MessageBox.Show("Google'a giriş yapma bölümünde '2 Adımlı Doğrulama' seçeneğini etkinleştirin (Etkin değilse)");
            DialogResult ynt= MessageBox.Show("2 Adımlı Doğrulama etkinleştirildikten sonra, aynı sayfada 'Uygulama Şifreleri' seçeneğini bulun Bu seçenegi görebiliyor musunuz?"," ",MessageBoxButtons.YesNo,MessageBoxIcon.Asterisk);
            if (ynt == DialogResult.No)
            {
                MessageBox.Show("Üstteki arama barına 'Uygulama Şifreleri' yazıp aratın, seçenek karşınıza çıkacaktır.");
            }
            MessageBox.Show("'Uygulama seçin' kısmından 'Diğer(Özel ad)' seçeneğini seçin ve uygulamanıza bir ad verin (ör. 'C# Email Uygulamam')");
            MessageBox.Show("'Oluştur' butonuna tıklayın");
            MessageBox.Show("Google size 16 karakterlik bir şifre verecek, bu şifreyi kopyalayın (boşluklar olmadan)");

        }
    }
}
