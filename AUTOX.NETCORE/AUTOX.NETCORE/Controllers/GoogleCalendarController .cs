using AUTOX.BLL.Interfaces;
using AUTOX.DAL;
using AUTOX.DOMAIN;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AUTOX.WEB.Controllers
{
    public class GoogleCalendarController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IDatabaseService _databaseService;

        public GoogleCalendarController(
            IConfiguration configuration,
            IDatabaseService databaseService)
        {
            _configuration = configuration;
            _databaseService = databaseService;
        }

        [HttpGet]
        public IActionResult Connect()
        {
            var clientId = _configuration["Google:ClientId"];
            var redirectUri = _configuration["Google:RedirectUri"];

            var scope = Uri.EscapeDataString(Google.Apis.Calendar.v3.CalendarService.Scope.Calendar);
            var finalRedirectUri = Uri.EscapeDataString(redirectUri);

            var authUrl =
                "https://accounts.google.com/o/oauth2/v2/auth" +
                "?client_id=" + Uri.EscapeDataString(clientId) +
                "&redirect_uri=" + finalRedirectUri +
                "&response_type=code" +
                "&scope=" + scope +
                "&access_type=offline" +
                "&prompt=consent";

            return Redirect(authUrl);
        }

        [HttpGet]
        public async Task<IActionResult> Callback(string code, string error)
        {
            if (!string.IsNullOrWhiteSpace(error))
            {
                return Content("Google authorization failed: " + error);
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                return Content("Authorization code not received.");
            }

            var clientId = _configuration["Google:ClientId"];
            var clientSecret = _configuration["Google:ClientSecret"];
            var redirectUri = _configuration["Google:RedirectUri"];

            var flow = new GoogleAuthorizationCodeFlow(
                new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new Google.Apis.Auth.OAuth2.ClientSecrets
                    {
                        ClientId = clientId,
                        ClientSecret = clientSecret
                    },
                    Scopes = new[]
                    {
                        Google.Apis.Calendar.v3.CalendarService.Scope.Calendar
                    }
                });

            TokenResponse tokenResponse = await flow.ExchangeCodeForTokenAsync(
                userId: "autox-google-calendar",
                code: code,
                redirectUri: redirectUri,
                taskCancellationToken: CancellationToken.None);

            if (tokenResponse == null)
            {
                return Content("Token exchange failed.");
            }

            using var dataTransactionManager = new DataTransactionManager(_databaseService.GetConnectionString());

            var existingList = await dataTransactionManager
                .GoogleCalendarTokenDataManager
                .RetrieveData("GetGoogleCalendarToken");

            var existing = existingList.FirstOrDefault();

            var model = new GoogleCalendarToken
            {
                GoogleCalendarTokenId = existing?.GoogleCalendarTokenId ?? 0,
                GoogleEmail = existing?.GoogleEmail ?? "your-connected-google-account@gmail.com",
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = !string.IsNullOrWhiteSpace(tokenResponse.RefreshToken) ? tokenResponse.RefreshToken: existing?.RefreshToken ?? string.Empty,
                TokenExpiryUtc = tokenResponse.IssuedUtc.AddSeconds(tokenResponse.ExpiresInSeconds ?? 0).ToUniversalTime(),
                CreatedAt = existing?.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            string json = JsonConvert.SerializeObject(model);

            var (status, id) = await dataTransactionManager
                .GoogleCalendarTokenDataManager
                .UpdateDataReturnPrimaryKey("UpdateGoogleCalendarToken", json);

            if (!status)
            {
                return Content("Token received, but saving to database failed.");
            }

            return Content("Google Calendar connected successfully.");
        }
    }
}