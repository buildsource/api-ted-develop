using System.Globalization;
using System.Text.Json.Serialization;

namespace TED.API.Configuration;

public class AppConfiguration
{

    [JsonPropertyName("API-GTW-TED-CONNECTIONSTRING")]
    public string DefaultConnection { get; set; }


    private double _valorMaximoDia;
    [JsonPropertyName("API-GTW-TED-TED-CONFIGURATION-VALOR-MAXIMO-DIA")]
    public string ValorMaximoDia
    {
        get
        {
            return _valorMaximoDia.ToString();
        }
        set
        {
            if (double.TryParse(value, out double parsedValue))
                _valorMaximoDia = parsedValue;
            else
                throw new FormatException("Invalid format. Please use a valid double number.");
        }
    }

    private int _quantidadeMaximaDia;
    [JsonPropertyName("API-GTW-TED-TED-CONFIGURATION-QUANTIDADE-MAXIMA-DIA")]
    public string QuantidadeMaximaDia
    {
        get
        {
            return _quantidadeMaximaDia.ToString();
        }
        set
        {
            if (int.TryParse(value, out int parsedValue))
                _quantidadeMaximaDia = parsedValue;
            else
                throw new FormatException("Invalid format. Please use a valid integer number.");
        }
    }

    private double _valorMaximoPorSaque;
    [JsonPropertyName("API-GTW-TED-TED-CONFIGURATION-VALOR-MAXIMO-POR-SAQUE")]
    public string ValorMaximoPorSaque
    {
        get
        {
            return _valorMaximoPorSaque.ToString();
        }
        set
        {
            if (double.TryParse(value, out double parsedValue))
                _valorMaximoPorSaque = parsedValue;
            else
                throw new FormatException("Invalid format. Please use a valid double number.");
        }
    }


    [JsonPropertyName("API-GTW-TED-SINACOR-CONFIGURATION-BASE-URL")]
    public string BaseUrl { get; set; }

    [JsonPropertyName("API-GTW-TED-SINACOR-CONFIGURATION-CLIENTE-SECRET")]
    public string ClienteSecret { get; set; }

    [JsonPropertyName("API-GTW-TED-SINACOR-CONFIGURATION-CLIENTE-ID")]
    public string ClienteId { get; set; }

    private DateTime _horarioInicio;
    [JsonPropertyName("API-GTW-TED-SINACOR-CONFIGURATION-HORARIO-INICIO")]
    public string HorarioInicio {
        get
        {
            return _horarioInicio.ToString("HH:mm:ss");
        }
        set
        {
            if (DateTime.TryParseExact(value, "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                _horarioInicio = parsedDate;
            else
                throw new FormatException("Invalid time format. Please use 'HH:mm:ss'.");
        }
    }

    private DateTime _horarioFim;
    [JsonPropertyName("API-GTW-TED-SINACOR-CONFIGURATION-HORARIO-FIM")]
    public string HorarioFim {
        get
        {
            return _horarioFim.ToString("HH:mm:ss");
        }
        set
        {
            if (DateTime.TryParseExact(value, "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                _horarioFim = parsedDate;
            else
                throw new FormatException("Invalid time format. Please use 'HH:mm:ss'.");
        }
    }
}