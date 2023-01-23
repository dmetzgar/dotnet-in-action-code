using ManningBooksApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddDbContext<CatalogContext>(options =>
{
  var connStr = config .GetConnectionString("Catalog");
  options.UseSqlite(connStr);
});

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options => {
  var scheme = new OpenApiSecurityScheme() {
    Name = "Authorization",
    Type = SecuritySchemeType.OAuth2,
    Scheme = JwtBearerDefaults.AuthenticationScheme,
    BearerFormat = "JWT",
    In = ParameterLocation.Header,
    Flows = new OpenApiOAuthFlows {
      Implicit = MsAuthHelper.GetImplicitFlow(
        config, 
        "testscope".AddScopePrefix(config))
    }
  };
  options.AddSecurityDefinition("token", scheme);
  options.AddSecurityRequirement(
    new OpenApiSecurityRequirement { {
      new OpenApiSecurityScheme {        
        Reference = new OpenApiReference {
          Type = ReferenceType.SecurityScheme,
          Id = "token",          
        }
      },
      Array.Empty<string>()
    }
  });
});

builder.Services.AddHsts(options => 
{
  options.Preload = true;
  options.IncludeSubDomains = true;
  options.MaxAge = DateTime.UtcNow.AddYears(1) - DateTime.UtcNow;
  options.ExcludedHosts.Add("test.manningcatalog.net");
});

builder.Services
  .AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options => options.ValidateMs(config));

builder.Services.AddAuthorization(options =>
{
  options.AddPolicy("AuthenticatedUsers", policyBuilder =>
  {
    policyBuilder
      .RequireAuthenticatedUser()
      .AddAuthenticationSchemes(
        JwtBearerDefaults.AuthenticationScheme);
  });
  options.AddPolicy("OnlyMe", policyBuilder => 
  {
    policyBuilder
      .RequireAuthenticatedUser()
      .AddAuthenticationSchemes(
        JwtBearerDefaults.AuthenticationScheme)
      .AddRequirements(new OnlyMeRequirement());
  });
});
builder.Services.AddSingleton<IAuthorizationHandler, 
  OnlyMeRequirementHandler>();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
  using (var dbContext = scope.ServiceProvider
    .GetRequiredService<CatalogContext>())
  {
    dbContext.Database.EnsureCreated();
  }
}

if (app.Environment.IsProduction())
{
  app.UseHsts();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
