namespace TED.API.Exceptions;

public class ValidationException : Exception
{
    public IEnumerable<string> Errors { get; private set; }

    public ValidationException(IEnumerable<string> errors)
        : base("Erro na Validação")
    {
        Errors = errors;
    }
}