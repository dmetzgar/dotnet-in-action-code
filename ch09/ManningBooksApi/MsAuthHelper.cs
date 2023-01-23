using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace ManningBooksApi;

public static class MsAuthHelper
{
  private const string UrlPrefix = 
    "https://login.microsoftonline.com/";
  private const string TenantIdConfig =
    "Authentication:Microsoft:TenantId";
  private const string ClientIdConfig =
    "Authentication:Microsoft:ClientId";

  public static OpenApiOAuthFlow GetImplicitFlow(
    IConfiguration config,
    params string[] extraScopes)
    {
      var tenantId = GetTenantId(config);
      var scopes = new Dictionary<string, string>() {
        { "openid", "" },
        { "profile", "" },
        { "email", "" }
      };
      foreach (var extraScope in extraScopes)
      {
        scopes.Add(extraScope, "");
      }

      return new OpenApiOAuthFlow {
        AuthorizationUrl = new Uri(
          $"{UrlPrefix}{tenantId}/oauth2/v2.0/authorize"),
        TokenUrl = new Uri(
          $"{UrlPrefix}{tenantId}/oauth2/v2.0/token"),
        Scopes = scopes,
      };
    }

  public static string GetTenantId(IConfiguration config)
    => config.GetValue<string>(TenantIdConfig);
  
  public static string GetClientId(IConfiguration config)
    => config.GetValue<string>(ClientIdConfig);
  
  public static string AddScopePrefix(this string scopeName, IConfiguration config)
    => $"api://{GetClientId(config)}/{scopeName}";

  public static JwtBearerOptions ValidateMs(
    this JwtBearerOptions options, 
    IConfiguration config)
  {
    options.Authority = 
      $"{UrlPrefix}{GetTenantId(config)}/v2.0";
    options.Audience = GetClientId(config);
    return options;
  }
}
