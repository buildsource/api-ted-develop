using Castle.DynamicProxy;
using System.Diagnostics;

namespace TED.API.Interceptors
{
    public class SuccessLoggingInterceptor : IInterceptor
    {
        private readonly ILogger<SuccessLoggingInterceptor> _logger;

        public SuccessLoggingInterceptor(ILogger<SuccessLoggingInterceptor> logger)
        {
            _logger = logger;
        }

        public void Intercept(IInvocation invocation)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                invocation.Proceed();
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Método {MethodName} executado com sucesso em {ExecutionTime} ms",
                                       invocation.Method.Name,
                                       stopwatch.ElapsedMilliseconds);
            }
        }
    }
}