using PlacemarkFilterAPI.Models;
using System.Xml.Linq;

namespace PlacemarkFilterAPI.Services
{
    public class KmlService
    {
        private readonly string _filePath;

        public KmlService()
        {
            _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "DIRECIONADORES1.kml");
        }

        public List<PlacemarkModel> LoadKml()
        {
            List<PlacemarkModel> placemarks = new List<PlacemarkModel>();

            try
            {
                var document = XDocument.Load(_filePath);
                var placemarkElements = document.Descendants().Where(x => x.Name.LocalName == "Placemark");

                foreach (var element in placemarkElements)
                {
                    var cliente = GetExtendedDataValue(element, "CLIENTE");
                    var situacao = GetExtendedDataValue(element, "SITUAÇÃO");
                    var bairro = GetExtendedDataValue(element, "BAIRRO");
                    var referencia = GetElementValue(element, "REFERENCIA");
                    var ruaCruzamento = GetExtendedDataValue(element, "RUA/CRUZAMENTO");

                    Console.WriteLine($"Cliente encontrado: {cliente}");

                    placemarks.Add(new PlacemarkModel
                    {
                        Cliente = cliente,
                        Situacao = situacao,
                        Bairro = bairro,
                        Referencia = referencia,
                        RuaCruzamento = ruaCruzamento
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar o arquivo KML: {ex.Message}");
            }

            return placemarks;
        }

        private string GetExtendedDataValue(XElement placemarkElement, string dataName)
        {
            XNamespace ns = "http://www.opengis.net/kml/2.2";

            var dataElement = placemarkElement.Descendants(ns + "Data")
                .FirstOrDefault(x => string.Equals(x.Attribute("name")?.Value, dataName, StringComparison.OrdinalIgnoreCase));

            if (dataElement != null)
            {
                var valueElement = dataElement.Element(ns + "value");
                if (valueElement != null && !string.IsNullOrWhiteSpace(valueElement.Value))
                {
                    return valueElement.Value.Trim();
                }
            }

            return "N/A";
        }

        private string GetElementValue(XElement element, string localName)
        {
            return element.Descendants().FirstOrDefault(x => x.Name.LocalName == localName)?.Value ?? string.Empty;
        }

        public List<PlacemarkModel> FilterPlacemarks(List<PlacemarkModel> placemarks, FilterCriteriaModel criteria)
        {
            var filteredPlacemarks = placemarks.AsQueryable();

            if (!string.IsNullOrEmpty(criteria.Cliente))
                filteredPlacemarks = filteredPlacemarks.Where(p => p.Cliente.Equals(criteria.Cliente, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(criteria.Situacao))
                filteredPlacemarks = filteredPlacemarks.Where(p => p.Situacao.Equals(criteria.Situacao, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(criteria.Bairro))
                filteredPlacemarks = filteredPlacemarks.Where(p => p.Bairro.Equals(criteria.Bairro, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(criteria.Referencia) && criteria.Referencia.Length >= 3)
                filteredPlacemarks = filteredPlacemarks.Where(p => p.Referencia.Contains(criteria.Referencia, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(criteria.RuaCruzamento) && criteria.RuaCruzamento.Length >= 3)
                filteredPlacemarks = filteredPlacemarks.Where(p => p.RuaCruzamento.Contains(criteria.RuaCruzamento, StringComparison.OrdinalIgnoreCase));

            return filteredPlacemarks.ToList();
        }

        public List<string> GetUniqueValues(List<PlacemarkModel> placemarks, string field)
        {
            return field.ToLower() switch
            {
                "cliente" => placemarks.Select(p => p.Cliente).Distinct().ToList(),
                "situação" => placemarks.Select(p => p.Situacao).Distinct().ToList(),
                "bairro" => placemarks.Select(p => p.Bairro).Distinct().ToList(),
                _ => new List<string>()
            };
        }

        public void ExportFilteredKml(List<PlacemarkModel> filtrosPlacemarks, string outputPath)
        {
            try
            {
                XNamespace ns = "http://www.opengis.net/kml/2.2";

                var kmlDocument = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement(ns + "kml", 
                        new XElement(ns + "Document",
                            filtrosPlacemarks.Select(p => new XElement(ns + "Placemark",
                                new XElement(ns + "name", p.Cliente),
                                new XElement(ns + "description", p.Referencia),
                                new XElement(ns + "ExtendedData",
                                    new XElement(ns + "Data", new XAttribute("name", "CLIENTE"),
                                        new XElement(ns + "value", p.Cliente)),
                                    new XElement(ns + "Data", new XAttribute("name", "SITUAÇÃO"),
                                        new XElement(ns + "value", p.Situacao)),
                                    new XElement(ns + "Data", new XAttribute("name", "BAIRRO"),
                                        new XElement(ns + "value", p.Bairro)),
                                    new XElement(ns + "Data", new XAttribute("name", "RUA/CRUZAMENTO"),
                                        new XElement(ns + "value", p.RuaCruzamento))
                                )
                            ))
                        )
                    )
                );

                kmlDocument.Save(outputPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao exportar o arquivo KML: {ex.Message}");
            }
        }
    }
}