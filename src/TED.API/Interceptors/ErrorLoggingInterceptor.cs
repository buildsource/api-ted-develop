
using Castle.DynamicProxy;

namespace TED.API.Interceptors
{
    public class ErrorLoggingInterceptor : IInterceptor
    {
        private readonly ILogger<ErrorLoggingInterceptor> _logger;

        public ErrorLoggingInterceptor(ILogger<ErrorLoggingInterceptor> logger)
        {
            _logger = logger;
        }

        public void Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar o método {MethodName}", invocation.Method.Name);
                throw;
            }
        }
    }
}