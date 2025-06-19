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
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Net.Mail;
using System.Net;

namespace muhasebe
{
    public partial class faturaCreate : Form
    {
        int fatura_id;
        int musteriID;
        string fatura_no, tarih, toplam_tutar, kdv_orani;




        public faturaCreate(int fatura_id, string fatura_no, int musteriID, string tarih, string toplam_tutar, string kdv_orani)
        {
            InitializeComponent();
            this.fatura_id = fatura_id;
            this.fatura_no = fatura_no;
            this.musteriID = musteriID;
            this.tarih = tarih;
            this.toplam_tutar = toplam_tutar;
            this.kdv_orani = kdv_orani;

        }

        string uygulamaSifre = ConfigHelper.Config.uygulamaSifre;
        string EPosta = ConfigHelper.Config.EPosta;
        string uygulamaAdi;
        string smtpHost = ConfigHelper.Config.smtpHost;



        public bool SendEmail(string to, string subject, string body, string pdfDosyaYolu)
    {
        uygulamaAdi = lblFirmaAd.Text;
      try
            {

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(EPosta, uygulamaAdi);
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

                Attachment pdfEklenti = new Attachment(pdfDosyaYolu);
                mail.Attachments.Add(pdfEklenti);

             
                smtp.Send(mail);

                return true;
            }
            catch (Exception ex)
            {
              
                Console.WriteLine("E-posta gönderme hatası: " + ex.Message);
                return false;
            }
        }

        public string musteriCekMail(string ID)
        {
            string eposta = null;

            try
            {

                if (Form1.Baglanti.State == ConnectionState.Closed)
                {
                    Form1.BaglantiAc();
                }

                string sorgu = "select * from musteriler where musteriID = @ID";
                SqlCommand yukleKomut = new SqlCommand(sorgu, Form1.Baglanti);
                yukleKomut.Parameters.AddWithValue("@ID", ID);
                SqlDataReader dr = yukleKomut.ExecuteReader();

                if (dr.Read())
                {
                    eposta = dr["ePosta"].ToString();
                }
            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message + " müşteri veri çekme hatası");
            }
            finally
            {
                if (Form1.Baglanti.State == ConnectionState.Open)
                {
                    Form1.Baglanti.Close();
                }
            }

            return eposta;
        }

        private void MusteriGetir(int musteriID)
        {
            string query = "SELECT * FROM musteriler WHERE musteriID = @id";
            SqlCommand cmd = new SqlCommand(query, Form1.Baglanti);
            cmd.Parameters.AddWithValue("@id", musteriID);

            SqlDataReader reader = null;

            try
            {
                Form1.BaglantiAc(); 

                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string musteriAdi = reader["musteriAdi"].ToString();
                    string musteriSoyadi = reader["musteriSoyadi"].ToString();
                    string musteriFirma = reader["musteriFirma"].ToString();
                    string musteriAdres = reader["musteriAdres"].ToString();
                    string vergiDairesi = reader["vergiDairesi"].ToString();
                    string vergiNo = reader["vergiNo"].ToString();
                    string telefon = reader["telefon"].ToString();
                    string ePosta = reader["ePosta"].ToString();
                    
                    // Form üzerindeki TextBox'lara yazdırma
                    lblMusteriAd.Text = musteriAdi+" "+musteriSoyadi;
                    lblMusteriFirma.Text = musteriFirma;
                    lblMusteriAdres.Text = musteriAdres;
                    lblMusteriVergiDaire.Text = vergiDairesi;
                    lblMusteriVergiNo.Text = vergiNo;
                    txtTelefon.Text = telefon;
                    txtEposta.Text = ePosta;
                    txtTelefon.Visible = false;
                    txtEposta.Visible = false;
                }
                else
                {
                    MessageBox.Show("Belirtilen müşteri bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Müşteri bilgileri alınırken hata oluştu:\n" + ex.Message);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();

                if (Form1.Baglanti != null && Form1.Baglanti.State == System.Data.ConnectionState.Open)
                    Form1.Baglanti.Close();
            }
        }

        private void btnConvertPdf_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "PDF Dosyası (*.pdf)|*.pdf";
                saveDialog.FileName = "fatura.pdf";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    Document doc = new Document(PageSize.A4, 25, 25, 30, 30);
                    PdfWriter.GetInstance(doc, new FileStream(saveDialog.FileName, FileMode.Create));
                    doc.Open();

                    
                    var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                    var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

                    
                    Paragraph header = new Paragraph("FATURA", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16));
                    header.Alignment = Element.ALIGN_CENTER;
                    doc.Add(header);
                    doc.Add(new Paragraph(" "));

                
                    PdfPTable infoTable = new PdfPTable(2);
                    infoTable.WidthPercentage = 100;
                    infoTable.SetWidths(new float[] { 1, 1 });

                    PdfPCell firmaCell = new PdfPCell();
                    firmaCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    firmaCell.AddElement(new Paragraph("**Firma Bilgileri**", boldFont));
                    firmaCell.AddElement(new Paragraph("Firma Adı: " + lblFirmaAd.Text, normalFont));
                    firmaCell.AddElement(new Paragraph("Adres: " + lblFirmaAdres.Text, normalFont));
                    firmaCell.AddElement(new Paragraph("Vergi No: " + lblFirmaVergiNo.Text, normalFont));

                    PdfPCell musteriCell = new PdfPCell();
                    musteriCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    musteriCell.AddElement(new Paragraph("**Müşteri Bilgileri**", boldFont));
                    musteriCell.AddElement(new Paragraph("Müşteri Adı: " + lblMusteriAd.Text, normalFont));
                    musteriCell.AddElement(new Paragraph("Adres: " + lblMusteriAdres.Text, normalFont));
                    musteriCell.AddElement(new Paragraph("Vergi No: " + lblMusteriVergiNo.Text, normalFont));

                    infoTable.AddCell(firmaCell);
                    infoTable.AddCell(musteriCell);
                    doc.Add(infoTable);

                    doc.Add(new Paragraph(" "));

                   
                    doc.Add(new Paragraph("Fatura No: " + lblFaturaNo.Text, normalFont));
                    doc.Add(new Paragraph("Fatura Tarihi: " + lblFaturaTarih.Text, normalFont));
                    doc.Add(new Paragraph(" "));

                  
                    PdfPTable productTable = new PdfPTable(dgvUrunListesi.Columns.Count);
                    productTable.WidthPercentage = 100;

                   
                    foreach (DataGridViewColumn col in dgvUrunListesi.Columns)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(col.HeaderText, boldFont));
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        productTable.AddCell(cell);
                    }

                  
                    foreach (DataGridViewRow row in dgvUrunListesi.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                productTable.AddCell(new Phrase(cell.Value?.ToString() ?? "", normalFont));
                            }
                        }
                    }

                    doc.Add(productTable);
                    doc.Add(new Paragraph(" "));

                   
                    PdfPTable totalsTable = new PdfPTable(2);
                    totalsTable.WidthPercentage = 40;
                    totalsTable.HorizontalAlignment = Element.ALIGN_RIGHT;

                    void AddTotalRow(string label, string value)
                    {
                        totalsTable.AddCell(new PdfPCell(new Phrase(label, boldFont)) { Border = iTextSharp.text.Rectangle.NO_BORDER });
                        totalsTable.AddCell(new PdfPCell(new Phrase(value, normalFont)) { Border = iTextSharp.text.Rectangle.NO_BORDER });
                    }

                    AddTotalRow("Ara Toplam:", lblAraToplam.Text);
                    AddTotalRow("KDV Tutarı:", lblKDVOran.Text);
                    AddTotalRow("Genel Toplam:", lblGenelToplam.Text);

                    doc.Add(totalsTable);

                    doc.Close();
                    MessageBox.Show("Fatura PDF olarak kaydedildi:\n" + saveDialog.FileName);

                    string aliciEmail = musteriCekMail(musteriID.ToString());

                    if (!string.IsNullOrWhiteSpace(aliciEmail)) 
                    {
                        DialogResult ynt = MessageBox.Show("Müşteriye Faturayı Göndermek İstiyor Musunuz?", "Mail Gönderim İzni", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (ynt == DialogResult.Yes)
                        {
                            string pdfDosyaYolu = saveDialog.FileName;
                            var (firmaAdi, firmaAdres, firmaVergiNo) = FirmaConfigApp.GetConfigData();
                            string konu = firmaAdi + " fatura";
                            string icerik = "Faturanız ekte PDF olarak sunulmuştur.";

                            bool sonuc = SendEmail(aliciEmail, konu, icerik, pdfDosyaYolu);

                            if (sonuc)
                            {
                                MessageBox.Show("E-posta başarıyla gönderildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("E-posta gönderilemedi. Lütfen ayarları kontrol edin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
        }

        private void FaturaKalemleriniYukle(int faturaId)
        {
            SqlConnection conn = Form1.Baglanti;
            Form1.BaglantiAc();

            string query = @" 
        SELECT 
            kalem_id, 
            urun_ad, 
            miktar, 
            birim_fiyat, 
            kdv_orani, 
            ((birim_fiyat + (birim_fiyat * kdv_orani / 100.0)) * miktar) AS toplam_tutar  
        FROM Fatura_Kalemleri 
        WHERE fatura_id = @fatura_id";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@fatura_id", faturaId);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["toplam_tutar"] != DBNull.Value)
                        {
                            row["toplam_tutar"] = Math.Round(Convert.ToDecimal(row["toplam_tutar"]), 2);
                        }
                    }

                    dgvUrunListesi.DataSource = dt;
                }
            }

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }



        private void HesaplaToplamlar()
        {
            decimal araToplam = 0;
            decimal toplamKdv = 0;
            decimal genelToplam = 0;

            foreach (DataGridViewRow row in dgvUrunListesi.Rows)
            {
                if (row.IsNewRow) continue; 

                decimal miktar = Convert.ToDecimal(row.Cells["miktar"].Value);
                decimal birimFiyat = Convert.ToDecimal(row.Cells["birim_fiyat"].Value);
                decimal kdvOrani = Convert.ToDecimal(row.Cells["kdv_orani"].Value);

                decimal satirAraToplam = miktar * birimFiyat;
                decimal satirKdv = satirAraToplam * (kdvOrani / 100);
                decimal satirGenelToplam = satirAraToplam + satirKdv;

                araToplam += satirAraToplam;
                toplamKdv += satirKdv;
                genelToplam += satirGenelToplam;
            }


            lblAraToplam.Text = araToplam.ToString("C2");    
            lblKDVOran.Text = toplamKdv.ToString("C2");
            lblGenelToplam.Text = genelToplam.ToString("C2");
        }



        private void faturaCreate_Load(object sender, EventArgs e)
        {
            FaturaKalemleriniYukle(fatura_id);
            HesaplaToplamlar();
            MusteriGetir(musteriID);

        
                var (firmaAdi, firmaAdres, firmaVergiNo) = FirmaConfigApp.GetConfigData();
                lblFirmaAd.Text = firmaAdi;
                lblFirmaAdres.Text = firmaAdres;
                lblFirmaVergiNo.Text = firmaVergiNo;
  
            lblMusteriAd.Text = lblMusteriAd.Text;
            lblMusteriAdres.Text = lblMusteriAdres.Text;
            lblMusteriVergiNo.Text = lblMusteriVergiNo.Text;
            lblFaturaTarih.Text = tarih;
            lblFaturaNo.Text = fatura_no;
   

              
            if (dgvUrunListesi.Columns.Count == 0)
            {
                dgvUrunListesi.Columns.Add("urunID", "Ürün ID");
                dgvUrunListesi.Columns.Add("urunAdi", "Ürün Adı");
                dgvUrunListesi.Columns.Add("miktar", "Miktar");
                dgvUrunListesi.Columns.Add("fiyat", "Fiyat");
                dgvUrunListesi.Columns.Add("tutar", "Tutar");
            }

            if (dgvUrunListesi.Rows.Count == 0)
            {
                dgvUrunListesi.Rows.Add("", "", "", "", "");
            }
        }
    }
}
