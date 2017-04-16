using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeCalculator
{
    public class Currency
    {
        public string id { get; set; }
        public string currency { get; set; }
        public double rate { get; set; }

        public override string ToString()
        {
            return $"{currency}\t{rate}";
        }
    }
    public class Rates
    {
        public static IEnumerable<Currency> GetBnrExchangeRates()
        {
            var result = new List<Currency>();
            const string url = "http://www.bnro.ro/nbrfxrates.xml";
            var request = (HttpWebRequest)WebRequest.Create(url);
            var body = string.Empty;
            try
            {
                var dataSet = new DataSet();
                if (!File.Exists("nbrfxrates.xsd"))
                    Wget("http://www.bnr.ro/xsd/nbrfxrates.xsd", "nbrfxrates.xsd");
                dataSet.ReadXmlSchema("nbrfxrates.xsd");
                var response = request.GetResponseAsync().Result;
                using (var receiveStream = response.GetResponseStream())
                using (var readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    dataSet.ReadXml(readStream);
                var data = dataSet.Tables["Rate"].AsEnumerable().ToList();
                foreach (var i in data)
                {
                    var currency = i.Field<string>("Currency");
                    var rate = (double)i.Field<decimal>("Rate_text");
                    var multiplier = (i.Field<long?>("multiplier")) ?? 1;
                    result.Add(new Currency()
                    {
                        id = Guid.NewGuid().ToString(),
                        currency = currency,
                        rate = rate / (multiplier != 0 ? multiplier : 1)
                    });
                };

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return result;
        }

        public static async Task<bool> Wget(string url, string destinationFileName)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            var body = string.Empty;
            try
            {
                var response = await request.GetResponseAsync();
                using (var input = response.GetResponseStream())
                using (var output = File.OpenWrite(destinationFileName))
                {
                    input?.CopyTo(output);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }
    }
}
