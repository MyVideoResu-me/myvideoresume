using System.Text.Json;
namespace MyVideoResume.Client.Pages.Shared.Security.Recaptcha;

public class RecaptchaService
{
    private readonly ILogger<RecaptchaService> _logger;
    private readonly IConfiguration _configuration;
    public RecaptchaService(ILogger<RecaptchaService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public virtual async Task<RecaptchaResponse?> Verify(string token)
    {
        RecaptchaResponse? reCaptchaResponse;
        using (var httpClient = new HttpClient())
        {
            var content = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("secret", _configuration.GetValue<string>("Security:Captcha_SecretKey")),
                new KeyValuePair<string, string>("response", token)
            });
            try
            {
                var response = await httpClient.PostAsync($"https://www.google.com/recaptcha/api/siteverify", content);
                var jsonString = await response.Content.ReadAsStringAsync();
                reCaptchaResponse = JsonSerializer.Deserialize<RecaptchaResponse>(jsonString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }

            return reCaptchaResponse;
        }
    }
}