using Amazon.SecretsManager.Model;
using Amazon.SecretsManager;
using Amazon;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace TED.API.Helpers;


[ExcludeFromCodeCoverage]
public static class SecretsManagerHelper
{
    public static async Task<string?> ConnectionString(string secretId)
    {
        if (string.IsNullOrWhiteSpace(secretId))
            throw new ArgumentNullException("secretId não pode ser nulo.");

        return await SecretsManager.GetSecretValueAsync(secretId);
    }
}


[ExcludeFromCodeCoverage]
public static class SecretsManager
{
    public static async Task<string?> GetSecretValueAsync(string secretId)
    {
        if (string.IsNullOrEmpty(secretId))
            throw new ArgumentNullException(nameof(secretId));

        try
        {
            var request = new GetSecretValueRequest
            {
                SecretId = secretId
            };

            using var client = new AmazonSecretsManagerClient(RegionEndpoint.SAEast1);

            var response = await client.GetSecretValueAsync(request);


            if (response == null)
                throw new ArgumentNullException("GetSecretValueResponse não pode ser nulo.");

            if (response?.SecretString != null)
                return response.SecretString;

            using var reader = new StreamReader(response.SecretBinary);

            return Encoding.UTF8.GetString(Convert.FromBase64String(await reader.ReadToEndAsync()));
        }
        catch (Exception _)
        {
            throw;
        }
    }
}