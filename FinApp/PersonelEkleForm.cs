using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Net;
using System.Data.SqlClient;

namespace muhasebe
{


    public partial class PersonelEkleForm : Form
    {
        public PersonelEkleForm()
        {
            InitializeComponent();
            ConfigHelper.LoadConfig();
        }

    string firmaAdi;
    string firmaAdres;
    private void PersonelEkleForm_Load(object sender, EventArgs e)
    {
      try
      {
        var (firmaAdii, firmaAdress, firmaVergiNoo) = FirmaConfigApp.GetConfigData();

        firmaAdres = firmaAdress;
        firmaAdi = firmaAdii;

      
      }
      catch (Exception ex)
      {
        MessageBox.Show(
            "Firma bilgileri okunurken bir hata oluştu: " + ex.Message,
            "Hata",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error
        );
      }
    }


    string uygulamaSifre = ConfigHelper.Config.uygulamaSifre;
        string EPosta = ConfigHelper.Config.EPosta;
        string uygulamaAdi = ConfigHelper.Config.uygulamaAdi;
        string smtpHost = ConfigHelper.Config.smtpHost;
        public static string GuvenliSifreUret(int uzunluk = 12, bool buyukHarf = true, bool kucukHarf = true, bool sayi = true, bool ozelKarakter = false)
        {
            if (uzunluk < 8)
                uzunluk = 8; 

            const string buyukHarfler = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string kucukHarfler = "abcdefghijklmnopqrstuvwxyz";
            const string sayilar = "0123456789";
            const string ozelKarakterler = "!@#$%^&*()-_=+[]{}|;:,.<>?/";

            StringBuilder sifreHavuzu = new StringBuilder();
            StringBuilder sifre = new StringBuilder();

            if (buyukHarf) sifreHavuzu.Append(buyukHarfler);
            if (kucukHarf) sifreHavuzu.Append(kucukHarfler);
            if (sayi) sifreHavuzu.Append(sayilar);
            if (ozelKarakter) sifreHavuzu.Append(ozelKarakterler);

            if (sifreHavuzu.Length == 0)
            {
                sifreHavuzu.Append(buyukHarfler);
                sifreHavuzu.Append(kucukHarfler);
                sifreHavuzu.Append(sayilar);
                sifreHavuzu.Append(ozelKarakterler);
            }

            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                byte[] randomBytes = new byte[4];

                if (buyukHarf)
                {
                    rng.GetBytes(randomBytes);
                    int index = Math.Abs(BitConverter.ToInt32(randomBytes, 0)) % buyukHarfler.Length;
                    sifre.Append(buyukHarfler[index]);
                }

                if (kucukHarf)
                {
                    rng.GetBytes(randomBytes);
                    int index = Math.Abs(BitConverter.ToInt32(randomBytes, 0)) % kucukHarfler.Length;
                    sifre.Append(kucukHarfler[index]);
                }

                if (sayi)
                {
                    rng.GetBytes(randomBytes);
                    int index = Math.Abs(BitConverter.ToInt32(randomBytes, 0)) % sayilar.Length;
                    sifre.Append(sayilar[index]);
                }

                if (ozelKarakter)
                {
                    rng.GetBytes(randomBytes);
                    int index = Math.Abs(BitConverter.ToInt32(randomBytes, 0)) % ozelKarakterler.Length;
                    sifre.Append(ozelKarakterler[index]);
                }

                for (int i = sifre.Length; i < uzunluk; i++)
                {
                    rng.GetBytes(randomBytes);
                    int index = Math.Abs(BitConverter.ToInt32(randomBytes, 0)) % sifreHavuzu.Length;
                    sifre.Append(sifreHavuzu[index]);
                }

                char[] sifreArray = sifre.ToString().ToCharArray();
                for (int i = sifreArray.Length - 1; i > 0; i--)
                {
                    rng.GetBytes(randomBytes);
                    int j = Math.Abs(BitConverter.ToInt32(randomBytes, 0)) % (i + 1);

                    char temp = sifreArray[i];
                    sifreArray[i] = sifreArray[j];
                    sifreArray[j] = temp;
                }

                return new string(sifreArray);
            }
        }
        private string GenerateVerificationCode(int length = 6)
        {
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                byte[] randomNumber = new byte[4];
                rng.GetBytes(randomNumber);

                int value = Math.Abs(BitConverter.ToInt32(randomNumber, 0));

                int max = (int)Math.Pow(10, length);
                return (value % max).ToString().PadLeft(length, '0');
            }
        }

        public bool SendEmail(string to, string subject, string body)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(EPosta, firmaAdi);
                mail.To.Add(to);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true; 

                SmtpClient smtp = new SmtpClient();
                smtp.Host = smtpHost; 
                smtp.Port = 587; 
                smtp.EnableSsl = true; 
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                smtp.Credentials = new NetworkCredential(EPosta, uygulamaSifre);

                smtp.Send(mail);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("E-posta gönderme hatası: " + ex.Message);
                return false;
            }
        }



   
    public bool MailKontrol()
        {
            txtMail.BackColor = Color.White;
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (Regex.IsMatch(txtMail.Text, pattern) == false)
            {
                txtMail.BackColor = Color.FromArgb(255, 215, 0);
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
        public bool boslukKontrol()
        {
            bool dondur = false;
            cmbBirim.BackColor = Color.White;
            txtTelNo.BackColor = Color.White;
            txtTCNo.BackColor = Color.White;
            txtSoyad.BackColor = Color.White;
            txtMail.BackColor = Color.White;
            txtAd.BackColor = Color.White;
            try
            {
                if (cmbBirim.Text == "" || cmbBirim.Text == "Seçim Yapınız")
                {
                    dondur = true;
                    cmbBirim.BackColor = Color.FromArgb(255, 82, 82);
                }
                if (txtMail.Text == "")
                {
                    
                    dondur = true;
                    txtMail.BackColor = Color.FromArgb(255, 82, 82);
                    txtMail.Focus();
                }
                
                if (txtTelNo.Text == "" || txtTelNo.Text== "(   )    -")
                {
                    dondur = true;
                    txtTelNo.BackColor = Color.FromArgb(255, 82, 82);
                    txtTelNo.Focus();
                }

          
                if (txtTCNo.Text == "")
                {
                    dondur = true;
                    txtTCNo.BackColor = Color.FromArgb(255, 82, 82);
                    txtTCNo.Focus();
                }
          
                if (txtSoyad.Text == "")
                {
                    dondur = true;
                    txtSoyad.BackColor = Color.FromArgb(255, 82, 82);
                    txtSoyad.Focus();
                }
                if (txtAd.Text == "")
                {
                    dondur = true;
                    txtAd.BackColor = Color.FromArgb(255, 82, 82);
                    txtAd.Focus();
                }
            }
            catch (Exception Hata)
            {
                MessageBox.Show("Boşluk kontrolu Hatası "+Hata, "Boşluk kontrol Hata Penceresi",MessageBoxButtons.OK,MessageBoxIcon.Error);
               
            }
            return dondur;
        }
    

        public void formTemizle()
        {
            txtAd.Text = "";
            txtSoyad.Text = "";
            txtTCNo.Text = "";
            txtTelNo.Text = "";
            txtMail.Text = "";
            txtDKod.Text = "";
            cmbBirim.Text = "Seçim Yapınız";
            btnkaydet.Visible = false;
            button1.Visible = true;
            txtDKod.ReadOnly = true;
            txtDKod.BackColor = Color.Gray;
            txtDKod.Cursor = Cursors.No;
        }

        public string emailSablon()
        {
            return $@"<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Doğrulama Kodu</title>
    <style type=""text/css"">
        /* Profesyonel Renk Paleti */
        body {{
            font-family: 'Segoe UI', Arial, sans-serif;
            line-height: 1.6;
            color: #333333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f9f9f9;
        }}
        
        .container {{
            border: 1px solid #dddddd;
            border-radius: 8px;
            padding: 30px;
            background-color: #ffffff;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
        }}
        
        /* Başlık Stillemesi */
        .header {{
            text-align: center;
            padding: 20px 0;
            border-bottom: 2px solid #1a73e8;
            margin-bottom: 25px;
            background-color: #f5f8ff;
            border-radius: 6px;
        }}
        
        .logo {{
            margin: 0 auto;
            max-width: 200px;
            height: auto;
        }}
        
        /* Kod Kutusu */
        .code-container {{
            background: #f5f8ff;
            padding: 20px;
            text-align: center;
            border-radius: 8px;
            margin: 25px 0;
            border: 1px solid #e0e8ff;
        }}
        
        .verification-code {{
            font-size: 32px;
            font-weight: bold;
            letter-spacing: 8px;
            color: #1a73e8;
            padding: 15px;
            background-color: #ffffff;
            border-radius: 6px;
            margin: 10px 0;
            border: 1px dashed #1a73e8;
        }}
        
        /* İçerik Stilleri */
        .info {{
            margin-bottom: 20px;
            color: #333333;
            background-color: #f5f8ff;
            padding: 15px;
            border-radius: 6px;
            border-left: 4px solid #1a73e8;
        }}
        
        .highlight {{
            font-weight: bold;
            color: #1a73e8;
        }}
        
        /* Alt Bilgi */
        .footer {{
            font-size: 12px;
            color: #666666;
            text-align: center;
            margin-top: 30px;
            padding-top: 20px;
            border-top: 1px solid #dddddd;
        }}
        
        .company-info {{
            display: flex;
            justify-content: space-between;
            margin-top: 20px;
            flex-wrap: wrap;
        }}
        
        .info-column {{
            flex: 1;
            min-width: 200px;
            padding: 10px;
        }}
        
        .timer {{
            display: inline-block;
            background-color: #f1f3f4;
            color: #5f6368;
            padding: 5px 10px;
            border-radius: 15px;
            font-size: 14px;
            margin-top: 10px;
        }}
.timer:before {{
            content: '⏱️';
            margin-right: 5px;
        }}
        
        /* Responsive Tasarım */
        @media only screen and (max-width: 480px) {{
            .container {{
                padding: 15px;
                margin: 10px;
            }}
            
            .verification-code {{
                font-size: 24px;
                letter-spacing: 5px;
            }}
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <img src=""cid:logo"" class=""logo"" alt=""{firmaAdi} Logo"">
        </div>
        
        <p>Merhaba <span class=""highlight"">{txtAd.Text} {txtSoyad.Text}</span>,</p>
        
        <div class=""info"">
            <p>İş başvurunuz için teşekkür ederiz. Başvurunuz başarıyla sistemimize kaydedilmiştir.</p>
            
            <p><strong>Çalışılacak Birim:</strong> <span class=""highlight"">{cmbBirim.Text}</span><br/>
            <strong>Telefon Numarası:</strong> {txtTelNo.Text}<br>
            <strong>Başvuru Tarihi:</strong> {DateTime.Now.ToShortDateString()}</p>
        </div>
        
        <div class=""code-container"">
            <p>Lütfen kimliğinizi doğrulamak için aşağıdaki kodu kullanın:</p>
            <div class=""verification-code"">{dKod}</div>
            <div class=""timer"">Bu kod 10 dakika geçerlidir</div>
        </div>
        
        <p>{firmaAdi} Şirketi'nde çalışmak için başvurunuzu tamamlamak üzere lütfen yukarıdaki doğrulama kodunu sisteme giriniz.</p>
        
        <p>Herhangi bir sorunuz olursa, lütfen <a href=""mailto:{EPosta}"" style=""color: #1a73e8;"">{EPosta}</a> adresinden bizimle iletişime geçmekten çekinmeyiniz.</p>
        
        <p>Saygılarımızla,<br>
        <span class=""highlight"">{firmaAdi} Şirketi İnsan Kaynakları</span></p>
        
        <div class=""company-info"">
            <div class=""info-column"">
                <strong style=""color: #1a73e8;"">İletişim</strong>
                Email: {EPosta}<br>
            </div>
            <div class=""info-column"">
                <strong style=""color: #1a73e8;"">Adres</strong>
                <p>{firmaAdres}</p>
            </div>
        </div>
        
        <div class=""footer"">
            <p>Bu e-posta otomatiktir. Lütfen yanıtlamayınız.</p>
            <p>Eğer <span class=""highlight"">{txtAd.Text}</span> değilseniz veya böyle bir başvuru yapmadıysanız, lütfen bu e-postayı görmezden geliniz ve derhal siliniz.</p>
            <p>Kişisel verileriniz KVKK kapsamında korunmaktadır.</p>
            <p>&copy; {DateTime.Now.Year} {firmaAdi} Şirketi. Tüm hakları saklıdır.</p>
        </div>
    </div>
</body>
</html>";
        }
        string dKod;
        string girisPass = GuvenliSifreUret();
        public string emailSablonPass()
        {
            return $@"<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Doğrulama Kodu</title>
    <style type=""text/css"">
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #E0E0E0;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #1C2541;
        }}
        .container {{
            border: 1px solid #E0E0E0;
            border-radius: 5px;
            padding: 25px;
            background-color: #0A1128;
        }}
        .header {{
            text-align: center;
            padding-bottom: 15px;
            border-bottom: 1px solid #E0E0E0;
            margin-bottom: 20px;
            background-color:#283655;
        }}
        .logo {{
            position: relative;
            top: 8px;
            font-size: 24px;
            font-weight: bold;
            color: #B0C4DE;
        }}
        .code-container {{
            background-color: #1C2541;
            padding: 15px;
            text-align: center;
            border-radius: 4px;
            margin: 20px 0;
            color: #4D648D;
        }}
        .verification-code {{
            font-size: 24px;
            font-weight: bold;
            letter-spacing: 5px;
            color: #00A8E8;
        }}
        .info {{
           margin-bottom: 15px;
            color:#E0E0E0;
        }}
        .footer {{
            font-size: 12px;
            color: #777777;
            text-align: center;
            margin-top: 20px;
            padding-top: 15px;
            border-top: 1px solid #E0E0E0;
        }}
        .highlight {{
             font-weight: bold;
        }}
        .button {{
            display: inline-block;
            background-color: #2C3E50;
            color: white;
            padding: 10px 20px;
            text-decoration: none;
            border-radius: 4px;
            margin-top: 10px;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <div class=""logo"">{firmaAdi} Muhasebe Uygulaması</div>
        </div>
        
        <p>Merhaba <span class=""highlight"">{txtAd.Text} {txtSoyad.Text}</span>,</p>
        
        <div class=""info"">
            <p>İlk iş gününüzü kutlarım. Aşağıda verilen şifre ile sisteme giriş yapabilirsin.</p>
            
          <p class=""p""><strong>Çalışılacak Birim:</strong> {cmbBirim.Text}<br/>
            </ p >

        </div>
        
        <div class=""code-container"">
            <p>Giriş Şifreniz:</p>
            <div class=""verification-code"">{girisPass}</div>
        </div>
        
       
        <p>Herhangi bir sorunuz olursa, lütfen bizimle iletişime geçmekten çekinmeyiniz.</p>
        
        <p>Saygılarımızla,<br>
        {firmaAdi} Şirketi İnsan Kaynakları</p>
        
        <div class=""footer"">
            <p>Bu e-posta otomatiktir. Lütfen yanıtlamayınız.</p>
            <p>Eğer <span class=""highlight"">{txtAd.Text}</span> değilseniz veya böyle bir başvuru yapmadıysanız, lütfen bu e-postayı görmezden geliniz ve derhal siliniz.</p>
            <p>&copy; 2025 {firmaAdi} Şirketi. Tüm hakları saklıdır.</p>
        </div>
    </div>
</body>
</html>";
        }
        public bool KayitEkle()
        {
            bool sonuc = false;

            try
            {
                Form1.BaglantiAc();

                string sorgu = "INSERT INTO Personel (PersonelAd, PersonelSoyad, PersonelTCNo, PersonelTel, PersonelMail, PersonelBirim,PersonelSifre) " +
                               "VALUES (@ad, @soyad, @tcno, @tel, @mail, @birim, @sifre)";

                using (SqlCommand cmd = new SqlCommand(sorgu, Form1.Baglanti))
                {
                    // Parametreleri ekle
                    cmd.Parameters.AddWithValue("@ad", txtAd.Text);
                    cmd.Parameters.AddWithValue("@soyad", txtSoyad.Text);
                    cmd.Parameters.AddWithValue("@tcno", txtTCNo.Text);
                    cmd.Parameters.AddWithValue("@tel", txtTelNo.Text);
                    cmd.Parameters.AddWithValue("@mail", txtMail.Text);
                    cmd.Parameters.AddWithValue("@birim", cmbBirim.Text);
                    cmd.Parameters.AddWithValue("@sifre", girisPass);

                    // Komutu çalıştır
                    int etkilenenSatir = cmd.ExecuteNonQuery();

                    // Eğer etkilenen satır sayısı 0'dan büyükse işlem başarılı
                    if (etkilenenSatir > 0)
                    {
                        sonuc = true;
                      
                    }
                }
            }
            catch (Exception ex)
            {
                // Hata mesajını göster
                MessageBox.Show("Kayıt eklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                sonuc = false;
            }
            finally
            {
                // Bağlantıyı kapat
                if (Form1.Baglanti != null && Form1.Baglanti.State == ConnectionState.Open)
                {
                    Form1.Baglanti.Close();
                }
            }

            return sonuc;
        }
        private bool epostaKullanim()
        {
            try
            {
                Form1.BaglantiAc();
                string mail = txtMail.Text;
                string Sorgu = "SELECT COUNT(*) FROM Personel WHERE PersonelMail= @mail";
                using (SqlCommand command = new SqlCommand(Sorgu, Form1.Baglanti))
                {
                    command.Parameters.AddWithValue("@mail", mail);
                    int existingCount = (int)command.ExecuteScalar();
                    if (existingCount > 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message, "Hata",
                         MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                if (Form1.Baglanti.State == ConnectionState.Open)
                { Form1.Baglanti.Close(); }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (boslukKontrol())
            {              
                MessageBox.Show("Boşlukları Doldur ");
            }
            else
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
                        MessageBox.Show("Mail Adresinizi Kontrol Ediniz", "Hatalı Mail Adresi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        string aliciEmail = txtMail.Text; 
                        string konu = "FinApp Doğrulama Kodu: "+dKod;
                        dKod = GenerateVerificationCode();
                        string icerik = emailSablon();

                        if (SendEmail(aliciEmail, konu, icerik))
                        {
                            if (!epostaKullanim())
                            {
                                MessageBox.Show("Bu e-posta adresi zaten kullanılıyor!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                MessageBox.Show("E-posta başarıyla gönderildi!");
                                txtDKod.ReadOnly = false;
                                txtDKod.Cursor = Cursors.IBeam;
                                txtDKod.BackColor = Color.White;
                                button1.Visible = false;
                                btnkaydet.Visible = true;
                            }
                            
                            
                        }
                        else
                        {
                            MessageBox.Show("E-posta gönderimi başarısız!");
                        }
               
                    }
                }
            }
           
        }

        private void btnkaydet_Click(object sender, EventArgs e)
        {
            if(dKod == txtDKod.Text)
            {

                KayitEkle();
                string aliciEmail = txtMail.Text;
                string konu = "uygulama Giriş Şifreniz";
                string icerik = emailSablonPass();

                if (SendEmail(aliciEmail, konu, icerik))
                {
                        MessageBox.Show("Şifreniz Mail Adresinize Gönderildi.", "Kayıt Başarılı");
                        txtDKod.ReadOnly = false;
                        txtDKod.Cursor = Cursors.IBeam;
                        txtDKod.BackColor = Color.White;
                        button1.Visible = false;
                        btnkaydet.Visible = true;
                }
                else
                {
                    MessageBox.Show("E-posta gönderimi başarısız! kullanıcı şifresi: ");
                }
                formTemizle();
            }
            else
            {
                DialogResult rtysend = MessageBox.Show("Kod Hatalı Doğrulama Kodunu Tekrar Almak İster Misiniz?", "Doğrulama Başarısız", MessageBoxButtons.YesNo, MessageBoxIcon.Warning); ;
                if(rtysend == DialogResult.Yes)
                {
                    btnkaydet.Visible = false;
                    button1.Visible = true;
                }
            }
            
        }
  }
}
