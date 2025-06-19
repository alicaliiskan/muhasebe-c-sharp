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
using System.Globalization;
using System.Net;


namespace muhasebe
{
    public partial class MuhasebeForm : Form
    {
        public MuhasebeForm()
        {
            InitializeComponent();
            ConfigHelper.LoadConfig();
        }

      

        public void FaturaGuncelleveToplamHesapla(int faturaId)
        {
            try
            {
                if (Form1.Baglanti.State != ConnectionState.Open)
                {
                    Form1.BaglantiAc();
                }

                string kalemSorgu = @"SELECT birim_fiyat, kdv_orani, miktar
                               FROM Fatura_Kalemleri
                               WHERE fatura_id = @fatura_id";

                decimal toplamTutar = 0;
                decimal toplamKdv = 0;
                decimal toplamKdvOrani = 0;
                int kalemSayisi = 0;

                using (SqlCommand cmd = new SqlCommand(kalemSorgu, Form1.Baglanti))
                {
                    cmd.Parameters.AddWithValue("@fatura_id", faturaId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            decimal birimFiyat = reader.IsDBNull(0) ? 0 : reader.GetDecimal(0);
                            decimal kdvOrani = reader.IsDBNull(1) ? 0 : reader.GetDecimal(1);
                            int miktar = reader.IsDBNull(2) ? 0 : reader.GetInt32(2); 


                            decimal netTutar = birimFiyat * miktar;

                            decimal kdvTutar = netTutar * (kdvOrani / 100);
                            decimal yeniBirimFiyat = netTutar + kdvTutar;

                            toplamTutar += yeniBirimFiyat;
                            toplamKdv += kdvTutar;
                            toplamKdvOrani += kdvOrani;
                            kalemSayisi++;
                        }
                    }
                }

                if (kalemSayisi > 0)
                {
                    decimal ortalamaKdvOrani = toplamKdvOrani / kalemSayisi;

                    string faturaGuncellemeSorgusu = @"UPDATE Faturalar
                                               SET toplam_tutar = @toplam_tutar, kdv_orani = @kdv_orani
                                               WHERE fatura_id = @fatura_id";

                    using (SqlCommand cmd = new SqlCommand(faturaGuncellemeSorgusu, Form1.Baglanti))
                    {
                        cmd.Parameters.AddWithValue("@toplam_tutar", toplamTutar);
                        cmd.Parameters.AddWithValue("@kdv_orani", ortalamaKdvOrani);
                        cmd.Parameters.AddWithValue("@fatura_id", faturaId);

                        int etkilenenSatir = cmd.ExecuteNonQuery();

                        if (etkilenenSatir > 0)
                        {
                         
                        }
                        else
                        {
                            MessageBox.Show("Fatura bulunamadı veya güncellenemedi.", "this is method FaturaGuncelleveToplamHesapla");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Fatura kalemi bulunamadı.", "Uyarı");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Fatura Güncelleme Hatası");
            }
            finally
            {
                if (Form1.Baglanti.State == ConnectionState.Open)
                {
                    Form1.Baglanti.Close();
                }
            }
        }





        private int EnSonFaturaId(SqlConnection conn)
        {
            if (conn.State != ConnectionState.Open)
            {
                Form1.BaglantiAc();
            }

            string query = "SELECT TOP 1 fatura_id FROM faturalar ORDER BY fatura_id DESC";
            SqlCommand cmd = new SqlCommand(query, conn);

            object result = cmd.ExecuteScalar();

            if (result != null)
            {
                return Convert.ToInt32(result);
            }
            else
            {
                return -1; 
            }
        }
        private string FaturaNoOlustur()
        {
            if (Form1.Baglanti.State != ConnectionState.Open)
            {
                Form1.BaglantiAc();
            }

            string tarihKismi = DateTime.Now.ToString("yyyyMMdd");
            Random rnd = new Random();
            int rastgeleSayi = rnd.Next(1000, 9999);
            string faturaNo = $"FTR-{tarihKismi}-{rastgeleSayi}";

            string query = "SELECT COUNT(*) FROM Faturalar WHERE fatura_no = @faturaNo";
            using (SqlCommand cmd = new SqlCommand(query, Form1.Baglanti))
            {
                cmd.Parameters.AddWithValue("@faturaNo", faturaNo);

                int count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    return FaturaNoOlustur();
                }
            }

            return faturaNo;
        }

        public void SatisYap()
        {
            try
            {
                Form1.BaglantiAc();
                DateTime tarih = DateTime.Now;
                string faturaOlulstur = "INSERT INTO Faturalar (fatura_no, musteriID, tarih, toplam_tutar, kdv_orani, PersonelID) VALUES(@fatura_no, @musteriID, @tarih, @toplam_tutar, @kdv_orani,@PersonelID)";
                SqlCommand calis = new SqlCommand(faturaOlulstur, Form1.Baglanti);
               
                string cmbText = cmbMusteriAdi.Text;
                string musteriIdStr = string.Empty;
              

                if (!string.IsNullOrEmpty(cmbText))
                {
                    string[] parts = cmbText.Split(new string[] { " - " }, StringSplitOptions.None);
                    if (parts.Length > 0)
                    {
                        musteriIdStr = parts[0];
                    }
                }

                int? musteriId = string.IsNullOrEmpty(musteriIdStr) ? (int?)null : int.Parse(musteriIdStr);

                calis.Parameters.AddWithValue("@fatura_no", FaturaNoOlustur());
                calis.Parameters.AddWithValue("@musteriID", musteriId.HasValue ? musteriId.Value : (object)DBNull.Value);
                calis.Parameters.AddWithValue("@tarih", tarih);
                calis.Parameters.AddWithValue("@toplam_tutar", 0);
                calis.Parameters.AddWithValue("@kdv_orani", 0);
                calis.Parameters.AddWithValue("@PersonelID", oturum.PersonelID);


                if (calis.ExecuteNonQuery() == 1)
                {
                    foreach (var item in lstBoxUrunler.Items)
                    {
                        string[] parcalar = item.ToString().Split('-');
                        if (parcalar.Length >= 3)
                        {
                            int urunId = int.Parse(parcalar[0].Trim());
                            int miktar = int.Parse(parcalar[2].Trim());

                            string urunAd = "";
                            decimal birimFiyat = 0;
                            decimal kdvOrani = 0;

                            string urunSorgu = "SELECT urunAdi, urunBirimFiyati, urunKdvOran FROM urunler WHERE urun_id = @id";
                            using (SqlCommand urunCmd = new SqlCommand(urunSorgu, Form1.Baglanti))
                            {
                                urunCmd.Parameters.AddWithValue("@id", urunId);
                                using (SqlDataReader reader = urunCmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        urunAd = reader["urunAdi"].ToString();
                                        birimFiyat = Convert.ToDecimal(reader["urunBirimFiyati"]);
                                        kdvOrani = Convert.ToDecimal(reader["urunKdvOran"]);
                                    }
                                }
                            }

                            string faturaKalemOlustur = @"INSERT INTO Fatura_Kalemleri (fatura_id, urun_ad, miktar, birim_fiyat, kdv_orani) VALUES (@fatura_id, @urun_ad, @miktar, @birim_fiyat, @kdv_orani)";
                            int sonFaturaId = EnSonFaturaId(Form1.Baglanti);
                            using (SqlCommand cmd = new SqlCommand(faturaKalemOlustur, Form1.Baglanti))
                            {
                            
                                cmd.Parameters.AddWithValue("@fatura_id", sonFaturaId); 
                                cmd.Parameters.AddWithValue("@urun_ad", urunAd);
                                cmd.Parameters.AddWithValue("@miktar", miktar);
                                cmd.Parameters.AddWithValue("@birim_fiyat", birimFiyat);
                                cmd.Parameters.AddWithValue("@kdv_orani", kdvOrani);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    int sonFaturaIdd = EnSonFaturaId(Form1.Baglanti);
                    FaturaGuncelleveToplamHesapla(sonFaturaIdd);
                    MessageBox.Show("fatura oluştu");
                    faturaCek();
                    }
            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message + " satış yapma hatası");
            }
            finally
            {
                if (Form1.Baglanti.State == ConnectionState.Open)
                {
                    Form1.Baglanti.Close();
                }
            }
        }


        public void UrunSil()
        {
            try
            {
                Form1.BaglantiAc();
                string sorgu = "DELETE FROM urunler WHERE urun_Id = @urun_Id";
                SqlCommand silKomut = new SqlCommand(sorgu, Form1.Baglanti);
                silKomut.Parameters.AddWithValue("@urun_Id", txtUrunSilID.Text);
                if (silKomut.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("Ürün başarıyla silindi.");
                    urunListele();
                    urunListeleCmb();
                }
             

            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message + " ürün silme hatası");
            }
            finally
            {
                if (Form1.Baglanti.State == ConnectionState.Open)
                {
                    Form1.Baglanti.Close();
                }
            }
        }
        public void urunEkle()
        {
            try
            {
                Form1.BaglantiAc();
                string sorgu = "insert into urunler(urunAdi,urunBirimFiyati,urunKdvOran,urunTotalFiyat) VALUES(@urunAdi,@urunBirimFiyati,@urunKdvOran,@urunTotalFiyat)";
                SqlCommand ekleKomut = new SqlCommand(sorgu,Form1.Baglanti);
                ekleKomut.Parameters.AddWithValue("@urunAdi",txtUrunEkleAdi.Text);
                ekleKomut.Parameters.AddWithValue("@urunBirimFiyati", txtUrunEkleFiyat.Text);
                ekleKomut.Parameters.AddWithValue("@urunKdvOran", txtUrunEkleKdv.Text);
                ekleKomut.Parameters.AddWithValue("@urunTotalFiyat", Convert.ToDouble(txtUrunEkleFiyat.Text) + Convert.ToDouble(txtUrunEkleFiyat.Text) * Convert.ToDouble(txtUrunEkleKdv.Text) / 100);
                if (ekleKomut.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("ekleme başarılı");
                    urunListele();
                    urunListeleCmb();
                }
            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message + " ürün ekleme hatası");
            }
            finally
            {
                if (Form1.Baglanti.State == ConnectionState.Open)
                {
                    Form1.Baglanti.Close();
                }
            }
        }

        public void urunGuncelle()
        {
            try
            {
                Form1.BaglantiAc();
                string sorgu = "UPDATE urunler SET urunAdi = @urunAdi, urunBirimFiyati = @urunBirimFiyati, urunKdvOran = @urunKdvOran, urunTotalFiyat = @urunTotalFiyat WHERE urun_Id = @urun_Id";
                SqlCommand guncellekomut = new SqlCommand(sorgu, Form1.Baglanti);

                CultureInfo kultur = new CultureInfo("tr-TR");

                string ad = txtUrunGuncelleAd.Text;
                decimal birimFiyat = decimal.Parse(txtUrunGuncelleFiyat.Text, kultur);
                decimal kdvOrani = decimal.Parse(txtUrunGuncelleKdv.Text, kultur);
                decimal totalFiyat = birimFiyat + (birimFiyat * kdvOrani / 100);

                guncellekomut.Parameters.AddWithValue("@urunAdi", ad);
                guncellekomut.Parameters.AddWithValue("@urunBirimFiyati", birimFiyat);
                guncellekomut.Parameters.AddWithValue("@urunKdvOran", kdvOrani);
                guncellekomut.Parameters.AddWithValue("@urunTotalFiyat", totalFiyat);
                guncellekomut.Parameters.AddWithValue("@urun_Id", txtUrunGuncelleID.Text);

                if (guncellekomut.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("Ürün güncellendi");
                    urunListele();
                    urunListeleCmb();
                }
            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message + " - ürün güncelleme hatası");
            }
            finally
            {
                if (Form1.Baglanti.State == ConnectionState.Open)
                {
                    Form1.Baglanti.Close();
                }
            }
        }
        public void musteriGuncelle()
        {
            try
            {
                if(txtmusteriGuncelleFirma.Text !=""|| txtmusteriGuncelleSoyad.Text != "" || txtmusteriGuncelleAd.Text != "")
                {
                Form1.BaglantiAc();
                string sorgu = "UPDATE musteriler SET musteriAdi = @ad, musteriSoyadi = @soyad,musteriAdres=@musteriAdres, musteriFirma = @firma, vergiDairesi = @vergiDairesi,vergiNo=@vergiNo,telefon=@telefon,ePosta=@ePosta WHERE musteriID = @id";
                SqlCommand guncelleKomut = new SqlCommand(sorgu, Form1.Baglanti);
                guncelleKomut.Parameters.AddWithValue("@ad", txtmusteriGuncelleAd.Text);
                guncelleKomut.Parameters.AddWithValue("@soyad", txtmusteriGuncelleSoyad.Text);
                guncelleKomut.Parameters.AddWithValue("@musteriAdres", txtMusteriAdres.Text);
                guncelleKomut.Parameters.AddWithValue("@firma", txtmusteriGuncelleFirma.Text);
                guncelleKomut.Parameters.AddWithValue("@vergiDairesi", txtmusteriVergidaire.Text);
                guncelleKomut.Parameters.AddWithValue("@vergiNo", txtMusteriVergiNo.Text);
                guncelleKomut.Parameters.AddWithValue("@telefon", txtMusteriTel.Text);
                guncelleKomut.Parameters.AddWithValue("@ePosta", txtMusteriMail.Text);
                guncelleKomut.Parameters.AddWithValue("@id", txtmusteriGuncelleID.Text);
                if (guncelleKomut.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("güncelleme başarılı");
                        musteriCek();
                        musterileriListeleCmb();
                }

                }
                else
                {
                    MessageBox.Show("Lütfen Bir Alanı Doldurun");
                }
            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message + " müşteri güncelle paneli hatası");
            }
            finally
            {
                if(Form1.Baglanti.State == ConnectionState.Open)
                {
                    Form1.Baglanti.Close();
                }
            }
        }
        public void MusteriSil()
        {
            try
            {
                Form1.BaglantiAc();
                string sorgu = "DELETE FROM musteriler where musteriID=@musteriID";
                SqlCommand musteriSil = new SqlCommand(sorgu, Form1.Baglanti);
                musteriSil.Parameters.AddWithValue("@musteriID", txtMusteriSilID.Text);
                if (musteriSil.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("Müsteriyi Sildim");
                    musteriCek();
                    musterileriListeleCmb();
                }
            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message + " Müşteri Sil Hata Penceresi");
            }
            finally
            {
                if( Form1.Baglanti.State == ConnectionState.Open)
                {
                    Form1.Baglanti.Close();
                }
            }
        }

        public void MusteriEkle()
        {
            try
            {
                if (txtmusteriEkleAd.Text != "" || txtmusteriEkleSoyad.Text != "" || txtmusteriEkleFirma.Text != "")
                {
                    Form1.BaglantiAc();
                    string sorgu = "insert into musteriler(musteriAdi,musteriSoyadi,musteriAdres,musteriFirma,vergiDairesi,vergiNo,telefon,ePosta) VALUES (@musteriAd,@musteriSoyad,@musteriAdres,@musteriFirma,@vergiDairesi,@vergiNo,@telefon,@ePosta)";
                    SqlCommand musteriEkle = new SqlCommand(sorgu, Form1.Baglanti);
                    musteriEkle.Parameters.AddWithValue("@musteriAd", txtmusteriEkleAd.Text);
                    musteriEkle.Parameters.AddWithValue("@musteriSoyad", txtmusteriEkleSoyad.Text);
                    musteriEkle.Parameters.AddWithValue("@musteriAdres", txtMusteriEkleAdres.Text);
                    musteriEkle.Parameters.AddWithValue("@musteriFirma", txtmusteriEkleFirma.Text);
                    musteriEkle.Parameters.AddWithValue("@vergiDairesi", txtMusteriEkleVergiDaire.Text);
                    musteriEkle.Parameters.AddWithValue("@vergiNo", txtMusteriEkleVergiNo.Text);
                    musteriEkle.Parameters.AddWithValue("@telefon", txtMusteriEkleTelefon.Text);
                    musteriEkle.Parameters.AddWithValue("@ePosta", txtMusteriEkleMail.Text);
                    if (musteriEkle.ExecuteNonQuery() == 1)
                    {
                        MessageBox.Show("Müşteriyi kaydettim");
                        musteriCek();
                        musterileriListeleCmb();
                        txtmusteriEkleAd.Text = "";
                        txtmusteriEkleSoyad.Text = "";
                        txtMusteriEkleAdres.Text = "";
                        txtmusteriEkleFirma.Text = "";
                        txtMusteriEkleVergiDaire.Text = "";
                        txtMusteriEkleVergiNo.Text = "";
                        txtMusteriEkleTelefon.Text = "";
                        txtMusteriEkleMail.Text = "";
                    }
                }
                else
                {
                    MessageBox.Show("Lütfen veri girin");
                }
            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message + " müşteri Ekleme Hata Penceresi");
            }
            finally
            {
                if (Form1.Baglanti.State == ConnectionState.Open)
                    Form1.Baglanti.Close(); 
            }
        }
        public void giderEkle()
        {
            try
            {
                Form1.BaglantiAc();
                string sorgu = "insert into Giderler(tarih,aciklama,tutar,kategori) VALUES (@tarih,@aciklama,@tutar,@kategori)";
                SqlCommand ekleKomutu = new SqlCommand(sorgu, Form1.Baglanti);
                ekleKomutu.Parameters.AddWithValue("@tarih", txtGiderEkleTarih.Value);
                ekleKomutu.Parameters.AddWithValue("@aciklama", txtGiderEkleAciklama.Text);
                ekleKomutu.Parameters.AddWithValue("tutar", txtGiderEkleTutar.Text);
                ekleKomutu.Parameters.AddWithValue("@kategori", txtGiderEkleKategori.Text);
                if(ekleKomutu.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("Gider Kaydedildi");
                    giderListele();
                }
            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message + " gider ekle paneli hatası");

            }
            finally
            {
                Form1.Baglanti.Close();
            }
        }
        public void giderSil()
        {
            try
            {
                Form1.BaglantiAc();
                string sorgu = "delete from Giderler where gider_id = @gider";
                SqlCommand silKomut = new SqlCommand(sorgu, Form1.Baglanti);
                silKomut.Parameters.AddWithValue("@gider", txtGiderIDSil.Text);
                if(silKomut.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("gider silindi.");
                    giderListele();
                }
            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message + " gider sil pencere hatası");

            }
            finally
            {
                Form1.Baglanti.Close();
            }
        }

        public void giderGuncelle()
        {
            try
            {
                Form1.BaglantiAc();
                string sorgu = "update giderler set tarih=@tarih,aciklama=@aciklama,tutar=@tutar,kategori=@kategori where gider_id = @gider_id";
                SqlCommand guncelleKomut = new SqlCommand(sorgu, Form1.Baglanti);
                guncelleKomut.Parameters.AddWithValue("@tarih", txtGiderGuncelleTarih.Value);
                guncelleKomut.Parameters.AddWithValue("@aciklama", txtGiderGuncelleAciklama.Text);
                guncelleKomut.Parameters.Add("@tutar", SqlDbType.Decimal).Value = Convert.ToDecimal(txtGiderGuncelleTutar.Text);
                guncelleKomut.Parameters.AddWithValue("@kategori", txtGiderGuncelleKategori.Text);
                guncelleKomut.Parameters.AddWithValue("@gider_id", txtGiderGuncelleID.Text);
                if (guncelleKomut.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("Veriler Güncellendi");
                    giderListele();
                }

            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message + " gider güncelle panel hatası");

            }
            finally
            {
                Form1.Baglanti.Close();
            }
        }
        public void faturaCek()
        {
            try
            {
                Form1.BaglantiAc();
                DataSet ds = new DataSet();
                string sorgu = "select * from Faturalar";
                SqlCommand musteriListeleKomut = new SqlCommand(sorgu, Form1.Baglanti);
                SqlDataAdapter da = new SqlDataAdapter(sorgu, Form1.Baglanti);
                da.Fill(ds, "faturalar");
               dgvFaturalar.DataSource = ds.Tables["faturalar"];
                dgvFaturalar.Columns["fatura_id"].HeaderText = "Fatura ID";
                dgvFaturalar.Columns["fatura_no"].HeaderText = "Fatura No";
                dgvFaturalar.Columns["musteriID"].HeaderText = "Müşteri ID";
                dgvFaturalar.Columns["tarih"].HeaderText = "Tarih";
                dgvFaturalar.Columns["toplam_tutar"].HeaderText = "Toplam Tutar";
                dgvFaturalar.Columns["kdv_orani"].HeaderText = "KDV Oranı";
                dgvFaturalar.Columns["PersonelID"].HeaderText = "Personel ID";

            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message + " fatura veri çekme hatası");

            }
            finally
            {
                Form1.Baglanti.Close();
            }

        }
        public void musteriCek()
        {
            try
            {
                Form1.BaglantiAc();
                DataSet ds = new DataSet();
                string sorgu = "select * from musteriler";
                SqlCommand musteriListeleKomut = new SqlCommand(sorgu,Form1.Baglanti);
                SqlDataAdapter da = new SqlDataAdapter(sorgu, Form1.Baglanti);
                da.Fill(ds, "musteriler");
                dgvMusteriler.DataSource = ds.Tables["musteriler"];
                dgvMusteriler.Columns["musteriID"].HeaderText = "Müşteri ID";
                dgvMusteriler.Columns["musteriAdi"].HeaderText = "Müşteri Adı";
                dgvMusteriler.Columns["musteriSoyadi"].HeaderText = "Müşteri Soyadı";
                dgvMusteriler.Columns["musteriAdres"].HeaderText = "Müşteri Adresi";
                dgvMusteriler.Columns["musteriFirma"].HeaderText = "Firma Adı";
                dgvMusteriler.Columns["vergiDairesi"].HeaderText = "Vergi Dairesi";
                dgvMusteriler.Columns["vergiNo"].HeaderText = "Vergi No";
                dgvMusteriler.Columns["telefon"].HeaderText = "Telefon";
                dgvMusteriler.Columns["ePosta"].HeaderText = "Mail";

            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message + " müşteri veri çekme hatası");

            }
            finally
            {
                Form1.Baglanti.Close();
            }
            
        }



     
        private void button7_Click(object sender, EventArgs e)
        {
            if (pnlMenu.Visible == false)
            {
                pnlMenu.Visible = true;

            }
            else
            {
                pnlMenu.Visible = false;
            }
        }


        public void frmAyar()
        {
            pnlUrunler.Dock = DockStyle.Fill;
            pnlSatis.Dock = DockStyle.Fill;
            pnlGider.Dock = DockStyle.Fill;
            pnlMusteriler.Dock = DockStyle.Fill;
            pnlFaturalar.Dock = DockStyle.Fill;
            pnlTicariMusteriSatis.Dock = DockStyle.Fill;
      
            pnlGiderEkle.Width = 484;
            pnlGiderEkle.Height = 405;
            pnlGiderSil.Width = 484;
            pnlGiderSil.Height = 405;
            pnlGiderGuncelle.Width = 484;
            pnlGiderGuncelle.Height = 405;

            pnlUrunEkle.Width = 484;
            pnlUrunEkle.Height = 405;
            pnlUrunSil.Width = 484;
            pnlUrunSil.Height = 405;
            pnlUrunGuncelle.Width = 484;
            pnlUrunGuncelle.Height = 405;


            pnlMusteriEkle.Width = 915;
            pnlMusteriEkle.Height = 437; 
            pnlMusteriSil.Width = 484;
            pnlMusteriSil.Height = 405;
            pnlMusteriGuncelle.Width = 915;
            pnlMusteriGuncelle.Height = 437;

            pnlGiderEkle.Left = (pnlGider.Width - pnlGiderEkle.Width) / 2;
            pnlGiderEkle.Top = (pnlGider.Height - pnlGiderEkle.Height) / 2;
            pnlGiderSil.Left = (pnlGider.Width - pnlGiderSil.Width) / 2;
            pnlGiderSil.Top = (pnlGider.Height - pnlGiderSil.Height) / 2;
            pnlGiderGuncelle.Left = (pnlGider.Width - pnlGiderGuncelle.Width) / 2;
            pnlGiderGuncelle.Top = (pnlGider.Height - pnlGiderGuncelle.Height) / 2;

            pnlUrunEkle.Left = (pnlUrunler.Width - pnlUrunEkle.Width) / 2;
            pnlUrunEkle.Top = (pnlUrunler.Height - pnlUrunEkle.Height) / 2;
            pnlUrunSil.Left = (pnlUrunler.Width - pnlUrunSil.Width) / 2;
            pnlUrunSil.Top = (pnlUrunler.Height - pnlUrunSil.Height) / 2;
            pnlUrunGuncelle.Left = (pnlUrunler.Width - pnlUrunGuncelle.Width) / 2;
            pnlUrunGuncelle.Top = (pnlUrunler.Height - pnlUrunGuncelle.Height) / 2;

            pnlMusteriEkle.Left = (pnlMusteriler.Width - pnlMusteriEkle.Width) / 2;
            pnlMusteriEkle.Top = (pnlMusteriler.Height - pnlMusteriEkle.Height) / 2;
            pnlMusteriSil.Left = (pnlMusteriler.Width - pnlMusteriSil.Width) / 2;
            pnlMusteriSil.Top = (pnlMusteriler.Height - pnlMusteriSil.Height) / 2;
            pnlMusteriGuncelle.Left = (pnlMusteriler.Width - pnlMusteriGuncelle.Width) / 2;
            pnlMusteriGuncelle.Top = (pnlMusteriler.Height - pnlMusteriGuncelle.Height) / 2;

            dgvGider.Visible = true;
            dgvFaturalar.Visible = true;
            dgvGider.Dock = DockStyle.Fill;
            dgvMusteriler.Dock = DockStyle.Fill;
            dgvUrunler.Dock = DockStyle.Fill;
            dgvFaturalar.Dock = DockStyle.Fill;
            pnlGiderEkle.Visible = false;
            pnlGiderSil.Visible = false;
            pnlGiderGuncelle.Visible = false;
            dgvMusteriler.Visible = true;
            pnlMusteriEkle.Visible = false;
            pnlMusteriSil.Visible = false;
            pnlMusteriGuncelle.Visible = false;
            dgvUrunler.Visible = true;
            pnlUrunEkle.Visible = false;
            pnlUrunGuncelle.Visible = false;
            pnlFaturalar.Visible = false;
            pnlUrunSil.Visible = false;
            dgvGider.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMusteriler.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUrunler.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvFaturalar.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            pnlGiderMenu.Visible = false;
            pnlMusteriMenu.Visible = false;
            pnlUrunMenu.Visible = false;
            pnlFaturaMenu.Visible = false;
        }

        public void urunListeleCmb()
        {
            try
            {
                Form1.BaglantiAc();
                string sorgu = "select * from urunler";
                SqlCommand listele = new SqlCommand(sorgu, Form1.Baglanti);
                SqlDataReader oku = listele.ExecuteReader();
                cmburunler.Items.Clear();

                while (oku.Read())
                {
                    int id = oku.GetInt32(0);
                    string urunAd = oku.GetString(1);
                    decimal BirimFiyat = oku.GetDecimal(2);
                    decimal urunKdbOran = oku.GetDecimal(3);
                    decimal urunbirimTotal = oku.GetDecimal(4);
                    cmburunler.Items.Add($"{id} - {urunAd}");
                }

                oku.Close();

            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message + " cmb ürünler listeleme hatası");
            }
            finally
            {
                if (Form1.Baglanti.State == ConnectionState.Open)
                {
                    Form1.Baglanti.Close();
                }
            }
        }

        public void musterileriListeleCmb()
        {
            try
            {
                Form1.BaglantiAc();
                string sorgu = "select musteriID,musteriAdi,musteriSoyadi,musteriFirma from musteriler";
                SqlCommand listele = new SqlCommand(sorgu, Form1.Baglanti);
                SqlDataReader oku = listele.ExecuteReader();
                cmbMusteriAdi.Items.Clear();
                while (oku.Read())
                {
                    int id = oku.GetInt32(0); 
                    string ad = oku.GetString(1); 
                    string soyad = oku.GetString(2); 
                    string firma = oku.GetString(3); 

                    cmbMusteriAdi.Items.Add($"{id} - {ad} {soyad} ({firma})");
                }

                oku.Close();

            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message + " cmb ürünler listeleme hatası");
            }
            finally
            {
                if(Form1.Baglanti.State == ConnectionState.Open)
                {
                    Form1.Baglanti.Close();
                }
            }
        }

        public void urunListele()
        {
            try
            {
                Form1.BaglantiAc();

                string sorgu = "SELECT * FROM urunler";
                SqlDataAdapter da = new SqlDataAdapter(sorgu, Form1.Baglanti);
                DataSet ds = new DataSet();
                da.Fill(ds, "urunler");

                dgvUrunler.DataSource = ds.Tables["urunler"];
                dgvUrunler.Columns["urun_Id"].HeaderText = "Ürün ID";
                dgvUrunler.Columns["urunAdi"].HeaderText = "Ürün Adı";
                dgvUrunler.Columns["urunBirimFiyati"].HeaderText = "Ürün Birim Fiyatı";
                dgvUrunler.Columns["urunKdvOran"].HeaderText = "Ürün KDV Oranı";
                dgvUrunler.Columns["urunTotalFiyat"].HeaderText = "Ürün Satış Fiyatı";
            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message + " ürün listeleme hatası");
            }
            finally
            {
                if (Form1.Baglanti.State == ConnectionState.Open)
                {
                    Form1.Baglanti.Close();
                }
            }
        }
        public void giderListele()
        {
            try
            {
                Form1.BaglantiAc();
                DataSet dsGider = new DataSet();
                String SorguGider = "select * FROM giderler";
                SqlDataAdapter daGider = new SqlDataAdapter(SorguGider, Form1.Baglanti);
                daGider.Fill(dsGider, "Giderler");
                dgvGider.DataSource = dsGider.Tables["Giderler"];
                dgvGider.Columns["gider_id"].HeaderText = "Gider ID";
                dgvGider.Columns["tarih"].HeaderText = "Tarih";
                dgvGider.Columns["aciklama"].HeaderText = "Gider Açıklaması";
                dgvGider.Columns["tutar"].HeaderText = "Gider Tutarı";
                dgvGider.Columns["kategori"].HeaderText = "kategori";
            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message, "gider listeleme hatası.");
            }
            finally
            {
                if(Form1.Baglanti.State == ConnectionState.Open)
                {
                    Form1.Baglanti.Close();
                }
            }
        }
        private void MuhasebeForm_Load(object sender, EventArgs e)
        {

            giderListele();
            urunListele();
            frmAyar();
            musteriCek();
            musterileriListeleCmb();
            urunListeleCmb();
            faturaCek();
        }

 

        private void button1_Click(object sender, EventArgs e)
        {
            giderEkle();
        }

        private void btnGiderSil_Click(object sender, EventArgs e)
        {
            giderSil();
        }

        private void btnGiderGuncelle_Click(object sender, EventArgs e)
        {
            giderGuncelle();
        }



    



        private void button1_Click_1(object sender, EventArgs e)
        {
            MusteriEkle();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MusteriSil();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            musteriGuncelle();
        }

        private void btnFaturaCreate_Click(object sender, EventArgs e)
        {
            pnlTicariMusteriSatis.Visible = true;
            pnlSatis.Visible = true;
            pnlMusteriler.Visible = false;
            pnlUrunler.Visible = false;
            pnlGider.Visible = false;
            pnlFaturalar.Visible = false;
        }

        private void btnUrunler_Click(object sender, EventArgs e)
        {

            pnlFaturaMenu.Visible = false;
            pnlGiderMenu.Visible = false;
            pnlMusteriMenu.Visible = false;
            if (pnlUrunMenu.Visible == true)
            {
                pnlUrunMenu.Visible = false;
            }
            else
            {
                pnlUrunMenu.Visible = true;
            }
            pnlSatis.Visible = false;
            pnlMusteriler.Visible = false;
            pnlUrunler.Visible = true;
            pnlGider.Visible = false;
            pnlFaturalar.Visible = false;
        }

        private void btnGider_Click(object sender, EventArgs e)
        {
            pnlFaturaMenu.Visible = false;
            pnlMusteriMenu.Visible = false;
            pnlUrunMenu.Visible = false;
            pnlSatis.Visible = false;
            pnlMusteriler.Visible = false;
            pnlUrunler.Visible = false;
            pnlGider.Visible = true;
            pnlFaturalar.Visible = false;
            if (pnlGiderMenu.Visible == true)
            {
                pnlGiderMenu.Visible = false;
            }
            else
            {
                pnlGiderMenu.Visible = true;
            }
 
        }

        private void btnMusteriler_Click(object sender, EventArgs e)
        {

            pnlFaturaMenu.Visible = false;
            pnlGiderMenu.Visible = false;
            pnlUrunMenu.Visible = false;
            if (pnlMusteriMenu.Visible == true)
            {
                pnlMusteriMenu.Visible = false;
            }
            else
            {
                pnlMusteriMenu.Visible = true;
            }
            pnlSatis.Visible = false;
            pnlMusteriler.Visible = true;
            pnlUrunler.Visible = false;
            pnlGider.Visible = false;
            pnlFaturalar.Visible = false;
        }

        private void btnUrunEkle_Click(object sender, EventArgs e)
        {
            urunEkle();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            urunGuncelle();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            UrunSil();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (cmburunler.SelectedItem != null)
            {
                lstBoxUrunler.Items.Add(cmburunler.SelectedItem.ToString() + " - " + txtUrunAdet.Text);
            }
            else
            {
                MessageBox.Show("Lütfen Bir ürün seçiniz", "Seçim Yapın", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
     
        }

        private void button11_Click(object sender, EventArgs e)
        {
      if(cmbMusteriAdi.SelectedItem == null)
      {
        MessageBox.Show("Lütfen Bir Müşteri Seçin");
      }else if (lstBoxUrunler.Items.Count == 0)
      {
        MessageBox.Show("Sepet boş");
      }
      else
      {
        SatisYap();
      }
            
        }

  

        private void button12_Click(object sender, EventArgs e)
        {
            if (lstBoxUrunler.SelectedItem != null)
            {
                lstBoxUrunler.Items.Remove(lstBoxUrunler.SelectedItem);
            }
            else
            {
                MessageBox.Show("Silmek için bir öğe seçmelisiniz.", "Uyarı");
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (lstBoxUrunler.Items.Count > 0)
            {
                lstBoxUrunler.Items.Clear();
            }
            else
            {
                MessageBox.Show("Listede silinecek öğe bulunmuyor.", "Bilgi");
            }
        }

        private void toolStripStatusLabel14_Click(object sender, EventArgs e)
        {
            dgvFaturalar.Visible = false;
  
        }



        private void button5_Click(object sender, EventArgs e)
        {
            pnlGiderMenu.Visible = false;
            pnlMusteriMenu.Visible = false;
            pnlUrunMenu.Visible = false;
            if(pnlFaturaMenu.Visible == true)
            {
                pnlFaturaMenu.Visible = false;
            }
            else
            {
                pnlFaturaMenu.Visible = true;
            }
           
            pnlSatis.Visible = false;
            pnlMusteriler.Visible = false;
            pnlUrunler.Visible = false;
            pnlGider.Visible = false;
            pnlFaturalar.Visible = true;
        }



        private void button8_Click(object sender, EventArgs e)
        {
            int fatura_id = Convert.ToInt32(dgvFaturalar.CurrentRow.Cells["fatura_id"].Value);
            string fatura_no = dgvFaturalar.CurrentRow.Cells["fatura_no"].Value?.ToString();
            int musteriID = Convert.ToInt32(dgvFaturalar.CurrentRow.Cells["musteriID"].Value);
            string tarih = dgvFaturalar.CurrentRow.Cells["tarih"].Value?.ToString();
            string toplam_tutar = dgvFaturalar.CurrentRow.Cells["toplam_tutar"].Value?.ToString();
            string kdv_orani = dgvFaturalar.CurrentRow.Cells["kdv_orani"].Value?.ToString();



            faturaCreate faturaForm = new faturaCreate(fatura_id, fatura_no, musteriID, tarih, toplam_tutar, kdv_orani);
            faturaForm.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            dgvGider.Visible = true;
            pnlGiderEkle.Visible = false;
            pnlGiderSil.Visible = false;
            pnlGiderGuncelle.Visible = false;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            dgvGider.Visible = false;
            pnlGiderEkle.Visible = true;
            pnlGiderSil.Visible = false;
            pnlGiderGuncelle.Visible = false;
        }

        private void button15_Click(object sender, EventArgs e)
        {
      if (dgvGider.Rows.Count > 0)
      {
        dgvGider.Visible = false;
        pnlGiderEkle.Visible = false;
        pnlGiderSil.Visible = false;
        pnlGiderGuncelle.Visible = true;
        txtGiderGuncelleID.Text = dgvGider.CurrentRow.Cells[0].Value.ToString();
        txtGiderGuncelleTarih.Text = dgvGider.CurrentRow.Cells[1].Value.ToString();
        txtGiderGuncelleAciklama.Text = dgvGider.CurrentRow.Cells[2].Value.ToString();
        txtGiderGuncelleTutar.Text = dgvGider.CurrentRow.Cells[3].Value.ToString();
        txtGiderGuncelleKategori.Text = dgvGider.CurrentRow.Cells[4].Value.ToString();
      }
      else
      {
        MessageBox.Show("veri seçilmedi");
      }
  
        }

        private void button16_Click(object sender, EventArgs e)
        {
      if (dgvGider.Rows.Count > 0)
      {
        dgvGider.Visible = false;
        pnlGiderEkle.Visible = false;
        pnlGiderSil.Visible = true;
        pnlGiderGuncelle.Visible = false;

        txtGiderIDSil.Text = dgvGider.CurrentRow.Cells[0].Value.ToString();
        txtGiderEkleTarih.Text = dgvGider.CurrentRow.Cells[1].Value.ToString();
        txtGiderAciklamaSil.Text = dgvGider.CurrentRow.Cells[2].Value.ToString();
        txtGiderTutarSil.Text = dgvGider.CurrentRow.Cells[3].Value.ToString();
        txtGiderKategoriSil.Text = dgvGider.CurrentRow.Cells[4].Value.ToString();
      }
      else {
        MessageBox.Show("Veri Seçilmedi");
          }
       
        }

        private void button17_Click(object sender, EventArgs e)
        {
            dgvMusteriler.Visible = true;
            pnlMusteriEkle.Visible = false;
            pnlMusteriSil.Visible = false;
            pnlMusteriGuncelle.Visible = false;
        }

        private void button20_Click(object sender, EventArgs e)
        {
      if (dgvMusteriler.Rows.Count > 0)
      {
        txtMusteriSilID.Text = dgvMusteriler.CurrentRow.Cells[0].Value.ToString();
        txtMusteriSilAd.Text = dgvMusteriler.CurrentRow.Cells[1].Value.ToString();
        txtMusteriSilSoyad.Text = dgvMusteriler.CurrentRow.Cells[2].Value.ToString();
        txtMusteriSilFirma.Text = dgvMusteriler.CurrentRow.Cells[3].Value.ToString();
        dgvMusteriler.Visible = false;
        pnlMusteriEkle.Visible = false;
        pnlMusteriSil.Visible = true;
        pnlMusteriGuncelle.Visible = false;
      }
      else
      {
        MessageBox.Show("veri yok");
      }
   
        }

        private void button18_Click(object sender, EventArgs e)
        {
            dgvMusteriler.Visible = false;
            pnlMusteriEkle.Visible = true;
            pnlMusteriSil.Visible = false;
            pnlMusteriGuncelle.Visible = false;
        }

        private void button19_Click(object sender, EventArgs e)
        {
      if(dgvMusteriler.Rows.Count > 0)
      {  
        txtmusteriGuncelleID.Text = dgvMusteriler.CurrentRow.Cells[0].Value.ToString();
        txtmusteriGuncelleAd.Text = dgvMusteriler.CurrentRow.Cells[1].Value.ToString();
        txtmusteriGuncelleSoyad.Text = dgvMusteriler.CurrentRow.Cells[2].Value.ToString();
        txtmusteriGuncelleFirma.Text = dgvMusteriler.CurrentRow.Cells[3].Value.ToString();

        dgvMusteriler.Visible = false;
        pnlMusteriEkle.Visible = false;
        pnlMusteriSil.Visible = false;
        pnlMusteriGuncelle.Visible = true;
      }
      else
      {
        MessageBox.Show("veri yok");
      }
           
        }

        private void button24_Click(object sender, EventArgs e)
        {
            dgvUrunler.Visible = true;
            pnlUrunEkle.Visible = false;
            pnlUrunGuncelle.Visible = false;
            pnlUrunSil.Visible = false;
        }

        private void button23_Click(object sender, EventArgs e)
        {
            dgvUrunler.Visible = false;
            pnlUrunEkle.Visible = true;
            pnlUrunGuncelle.Visible = false;
            pnlUrunSil.Visible = false;
        }

        private void button22_Click(object sender, EventArgs e)
        {
            txtUrunGuncelleID.Text = dgvUrunler.CurrentRow.Cells[0].Value.ToString();
            txtUrunGuncelleAd.Text = dgvUrunler.CurrentRow.Cells[1].Value.ToString();
            txtUrunGuncelleFiyat.Text = dgvUrunler.CurrentRow.Cells[2].Value.ToString();
            txtUrunGuncelleKdv.Text = dgvUrunler.CurrentRow.Cells[3].Value.ToString();
            dgvUrunler.Visible = false;
            pnlUrunEkle.Visible = false;
            pnlUrunGuncelle.Visible = true;
            pnlUrunSil.Visible = false;
        }

        private void button21_Click(object sender, EventArgs e)
        {
            txtUrunSilID.Text = dgvUrunler.CurrentRow.Cells[0].Value.ToString();
            txtUrunSilAd.Text = dgvUrunler.CurrentRow.Cells[1].Value.ToString();
            txtUrunSilFiyat.Text = dgvUrunler.CurrentRow.Cells[2].Value.ToString();
            txtUrunSilKdv.Text = dgvUrunler.CurrentRow.Cells[3].Value.ToString();
            dgvUrunler.Visible = false;
            pnlUrunEkle.Visible = false;
            pnlUrunGuncelle.Visible = false;
            pnlUrunSil.Visible = true;
        }
    }
}
