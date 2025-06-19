# 📊 Muhasebe Takip Uygulaması

Bu uygulama, küçük ve orta ölçekli işletmelerin finansal işlemlerini kolayca takip edebileceği, C# ile geliştirilen kullanıcı dostu bir masaüstü muhasebe uygulamasıdır.

---

## 🧾 Özellikler

- 💼 Firma ve müşteri yönetimi
- 📦 Ürün tanımlama
- 🧾 Fatura oluşturma, yazdırma ve müşteriye iletme
- 💰 Gelir ve gider kayıtları
- 📈 Aylık/yıllık raporlar
- 📧 Müşteri e-posta adreslerini kaydetme ve e-posta gönderme desteği

> ℹ️ **Not:** Projeye girilen e-posta adresleri önemlidir. Uygulama bu adreslere e-posta gönderebilir.

---

## 🗃️ Veritabanı Bilgisi

- Uygulama, **`muhasebe.dacpac`** adlı veritabanı dağıtım dosyasını kullanır.
- Veritabanını projeye dahil etmek için:

### 📌 Adımlar:
1. Visual Studio'da **SQL Server Object Explorer** panelini açın.
2. Sağ tıklayıp **"Add DACPAC"** diyerek `muhasebe.dacpac` dosyasını yükleyin.
3. Yükleme tamamlandıktan sonra `muhasebe` veritabanı oluşacaktır.

---

## 🛠️ Bağlantı Ayarları

Uygulama içindeki `Form1.cs` dosyasında tanımlı olan bağlantı değişkeni güncellenmelidir:

### 📌 Örnek:
```csharp
string baglanti = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=muhasebe;Integrated Security=True;";
