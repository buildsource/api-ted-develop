
using System.Text.Json.Serialization;
using TED.API.Exceptions;

namespace TED.API.Response;


/// <summary>
/// Representa a resposta padrão da API.
/// </summary>
public class ApiResponse<T>
{
    /// <summary>
    /// Indica se a operação foi bem-sucedida.
    /// </summary>
    [JsonPropertyName("isSuccess")]
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Mensagem descritiva sobre a operação.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; }

    /// <summary>
    /// Dados associados à resposta, se houver.
    /// </summary>
    [JsonPropertyName("data")]
    public T Data { get; set; }

    /// <summary>
    /// Construtor para uma resposta bem-sucedida.
    /// </summary>
    /// <param name="data">Dados a serem incluídos na resposta.</param>
    /// <param name="message">Mensagem descritiva da operação.</param>
    public ApiResponse(T data, string message)
    {
        IsSuccess = true;
        Message = message;
        Data = data;
    }

    /// <summary>
    /// Construtor para uma resposta com erro.
    /// </summary>
    /// <param name="message">Mensagem de erro.</param>
    /// <param name="err">Lista de mensagens de erro.</param>
    public ApiResponse(string message, List<string> err)
    {
        IsSuccess = false;
        Message = message ?? "Ocorreu um erro ao processar a requisição.";
        Data = (T)(object)new List<Notification>
        {
            new Notification(Guid.NewGuid().ToString(), string.Join("; ", err))
        };
    }

    /// <summary>
    /// Construtor para uma resposta com erro de validação.
    /// </summary>
    /// <param name="validationException">A exceção de validação capturada.</param>
    public ApiResponse(ValidationException validationException)
    {
        IsSuccess = false;
        Message = "Erro na validação";
        Data = (T)(object)validationException.Errors
        .Select(err => new Notification(Guid.NewGuid().ToString(), err))
        .ToList();
    }

    public ApiResponse() { }
}


/// <summary>
/// Representa uma notificação ou mensagem de erro.
/// </summary>
public class Notification
{
    /// <summary>
    /// Chave identificadora da notificação.
    /// </summary>
    [JsonPropertyName("key")]
    public string Key { get; }

    /// <summary>
    /// Mensagem da notificação.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; }

    /// <summary>
    /// Construtor para criar uma notificação.
    /// </summary>
    /// <param name="key">Chave da notificação.</param>
    /// <param name="message">Mensagem da notificação.</param>
    public Notification(string key, string message)
    {
        Key = key;
        Message = message;
    }
}
