using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

namespace muhasebe
{
  public class FirmaConfigApp
    {
        private static readonly string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FirmaData.config");

        public static void CreateConfigFile()
        {
            if (File.Exists(configFilePath))
            {
                return;
            }

            try
            {
                XDocument doc = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement("FirmaData",
                        new XElement("FirmaAdi", ""),
                        new XElement("FirmaAdres", ""),
                        new XElement("FirmaVergiNo", "")
                    )
                );

                doc.Save(configFilePath);
            }
            catch (Exception ex)
            {
                throw new Exception("Config dosyası oluşturulurken hata oluştu: " + ex.Message);
            }
        }



        public static void UpdateConfigFile(string firmaAdi, string firmaAdres, string firmaVergiNo)
        {
            if (!File.Exists(configFilePath))
            {
                CreateConfigFile();
            }

            try
            {
                XDocument doc = XDocument.Load(configFilePath);

                XElement firmaData = doc.Element("FirmaData");
                if (firmaData == null)
                {
                    throw new Exception("Geçersiz XML formatı: FirmaData elementi bulunamadı.");
                }

                XElement firmaAdiElement = firmaData.Element("FirmaAdi");
                XElement firmaAdresElement = firmaData.Element("FirmaAdres");
                XElement firmaVergiNoElement = firmaData.Element("FirmaVergiNo");

                if (firmaAdiElement == null || firmaAdresElement == null || firmaVergiNoElement == null)
                {
                    throw new Exception("XML dosyasında gerekli elementlerden biri eksik.");
                }

                firmaAdiElement.Value = firmaAdi;
                firmaAdresElement.Value = firmaAdres;
                firmaVergiNoElement.Value = firmaVergiNo;

                doc.Save(configFilePath);
            }
            catch (Exception ex)
            {
                throw new Exception("Config dosyası güncellenirken hata oluştu: " + ex.Message);
            }
        }

        public static (string FirmaAdi, string FirmaAdres, string FirmaVergiNo) GetConfigData()
        {
            try
            {
                if (!File.Exists(configFilePath))
                {
                    CreateConfigFile();
                }

                XDocument doc = XDocument.Load(configFilePath);

                XElement firmaData = doc.Element("FirmaData");
                if (firmaData == null)
                {
                    throw new Exception("Geçersiz XML formatı: FirmaData elementi bulunamadı.");
                }

                XElement firmaAdiElement = firmaData.Element("FirmaAdi");
                XElement firmaAdresElement = firmaData.Element("FirmaAdres");
                XElement firmaVergiNoElement = firmaData.Element("FirmaVergiNo");

                if (firmaAdiElement == null || firmaAdresElement == null || firmaVergiNoElement == null)
                {
                    throw new Exception("XML dosyasında gerekli elementlerden biri eksik.");
                }

                return (
                    firmaAdiElement.Value,
                    firmaAdresElement.Value,
                    firmaVergiNoElement.Value
                );
            }
            catch (Exception ex)
            {
                throw new Exception("Config dosyası okunurken hata oluştu: " + ex.Message);
            }
        }

    }
}
