using MauiAppTempoAgora.Models;
using System.Text.Json.Nodes;

namespace MauiAppTempoAgora.Services
{
    public class DataService
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<Tempo?> GetPrevisao(string cidade)
        {
            Tempo? t = null;

            string chave = "7046260d0783435cfd8890bd1d75a99d";

            string url = $"https://api.openweathermap.org/data/2.5/weather?" +
                         $"q={Uri.EscapeDataString(cidade)}&units=metric&appid={chave}";

            try
            {
                HttpResponseMessage resp = await client.GetAsync(url);

                if (resp.IsSuccessStatusCode)
                {
                    string json = await resp.Content.ReadAsStringAsync();
                    var rascunho = JsonNode.Parse(json);

                    if (rascunho != null)
                    {
                        // Unix Epoch correto para conversão dos timestamps
                        long sunriseUnix = (long)(rascunho["sys"]?["sunrise"] ?? 0);
                        long sunsetUnix = (long)(rascunho["sys"]?["sunset"] ?? 0);

                        DateTime sunrise = DateTime.UnixEpoch.AddSeconds(sunriseUnix).ToLocalTime();
                        DateTime sunset = DateTime.UnixEpoch.AddSeconds(sunsetUnix).ToLocalTime();

                        t = new()
                        {
                            lat = (double)(rascunho["coord"]?["lat"] ?? 0.0),
                            lon = (double)(rascunho["coord"]?["lon"] ?? 0.0),
                            description = (string)(rascunho["weather"]?[0]?["description"] ?? ""),
                            main = (string)(rascunho["weather"]?[0]?["main"] ?? ""),
                            temp_min = (double)(rascunho["main"]?["temp_min"] ?? 0.0),
                            temp_max = (double)(rascunho["main"]?["temp_max"] ?? 0.0),
                            speed = (double)(rascunho["wind"]?["speed"] ?? 0.0),
                            visibility = (int)(rascunho["visibility"] ?? 0),
                            sunrise = sunrise.ToShortTimeString(),
                            sunset = sunset.ToShortTimeString()
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                // Trata falhas de rede ou parsing sem travar o app
                System.Diagnostics.Debug.WriteLine($"Erro ao buscar clima: {ex.Message}");
            }

            return t;
        }
    }
}