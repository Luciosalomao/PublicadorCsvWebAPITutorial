using Newtonsoft.Json;
using ConvertCsvToJson;
using System.Net;
using System.Text;
using CsvHelper;
using System.Globalization;


namespace ConversorCsvJson
{
    public partial class frmPrincipal : Form
    {
        public frmPrincipal()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        static List<Dictionary<string, string>> ReadCsv(string filePath)
        {
            List<Dictionary<string, string>> csvData = new List<Dictionary<string, string>>();

            using (StreamReader reader = new StreamReader(filePath))
            {
                string[] headers = reader.ReadLine().Split(',');

                while (!reader.EndOfStream)
                {
                    string[] values = reader.ReadLine().Split(',');
                    Dictionary<string, string> dataEntry = new Dictionary<string, string>();

                    for (int i = 0; i < headers.Length; i++)
                    {
                        dataEntry[headers[i]] = values[i];
                    }

                    csvData.Add(dataEntry);
                }
            }

            return csvData;
        }

        static string ConvertToJson(List<Dictionary<string, string>> csvData)
        {
            return JsonConvert.SerializeObject(csvData, Formatting.Indented);
        }

        static void SaveJsonToFile(string jsonData, string filePath)
        {
            File.WriteAllText(filePath, jsonData);
        }

        private void txtPathCsv_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {


        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        public class Dado
        {
            public string nome { get; set; }
            public decimal preco { get; set; }
            public string descricao { get; set; }

        }

        public class Dado2
        {
            public string nome { get; set; }
            public string preco { get; set; }
            public string descricao { get; set; }
        }

        private void button5_Click(object sender, EventArgs e)
        {


        }

        private void button6_Click(object sender, EventArgs e)
        {

            var urlBase = "http://localhost:87";
            var api = "/api/v1/produto/lista";

            var urlApiProduto = urlBase + api;

            List<Dado> ldados = new List<Dado>();

            using var produto = new HttpClient();
            using var openFileDialog = new OpenFileDialog();

            try
            {
                openFileDialog.Filter = "Arquivos JSON (*.csv)|*.csv|Todos os arquivos (*.*)|*.*";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string jsonContent = File.ReadAllText(openFileDialog.FileName);

                    using (var reader = new StreamReader(openFileDialog.FileName))

                    using (var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)))
                    {
                        var registros = csv.GetRecords<Dado2>().ToArray();

                        foreach (var dado in registros)
                        {
                            decimal result = decimal.Parse(dado.preco, CultureInfo.InvariantCulture); 

                            Dado registro = new()
                            {
                                nome = dado.nome,
                                preco = result,
                                descricao = dado.descricao
                            };

                            ldados.Add(registro);
                        }

                        string JsonObjeto = JsonConvert.SerializeObject(ldados);

                        var content = new StringContent(JsonObjeto, Encoding.UTF8, "application/json");
                        ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

                        var resultado = produto.PostAsync(urlApiProduto, content).Result;

                        if (resultado.IsSuccessStatusCode)
                        {
                            var RetornoJson = resultado.Content.ReadAsStringAsync();
                            RetornoJson.Wait();
                            var sRetorno = JsonConvert.DeserializeObject(RetornoJson.Result).ToString();
                            MessageBox.Show(sRetorno);
                        }
                        else
                        {
                            MessageBox.Show(resultado.IsSuccessStatusCode.ToString());
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocorreu um erro: {ex.Message}");
            }
        }
    }
}