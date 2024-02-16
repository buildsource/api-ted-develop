using TED.API.DTOs;

namespace TED.API.Interfaces
{
    public interface ISinacorTedService
    {
        Task<string> ClienteEnviaTedParaSinacorAsync(SinacorTedRequestDto sinacorTedRequestDto);
    }
}
