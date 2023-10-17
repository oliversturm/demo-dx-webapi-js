using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace XAFApp.WebApi.API.Security;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase {
  private readonly IConfiguration configuration;
  private readonly IStandardAuthenticationService securityAuthenticationService;

  public AuthenticationController(IStandardAuthenticationService securityAuthenticationService,
    IConfiguration configuration) {
    this.securityAuthenticationService = securityAuthenticationService;
    this.configuration = configuration;
  }

  [HttpPost("AuthenticateJwt")]
  public IActionResult AuthenticateJwt(
    [FromBody] [SwaggerRequestBody(@"For example: <br /> { ""userName"": ""Admin"", ""password"": """" }")]
    AuthenticationStandardLogonParameters logonParameters
  ) {
    ClaimsPrincipal user = securityAuthenticationService.Authenticate(logonParameters);

    if (user != null) {
      SymmetricSecurityKey issuerSigningKey =
        new(Encoding.UTF8.GetBytes(configuration["Authentication:Jwt:IssuerSigningKey"]));
      JwtSecurityToken token = new(
        configuration["Authentication:Jwt:Issuer"],
        configuration["Authentication:Jwt:Audience"],
        user.Claims,
        expires: DateTime.Now.AddHours(2),
        signingCredentials: new SigningCredentials(issuerSigningKey, SecurityAlgorithms.HmacSha256)
      );
      return Ok(new JwtSecurityTokenHandler().WriteToken(token));
    }

    return Unauthorized("User name or password is incorrect.");
  }

  [HttpPost("LogIn")]
  public IActionResult LogIn(
    [FromBody] [SwaggerRequestBody(@"For example: <br /> { ""userName"": ""Admin"", ""password"": """" }")]
    AuthenticationStandardLogonParameters logonParameters
  ) {
    ClaimsPrincipal user = securityAuthenticationService.Authenticate(logonParameters);

    if (user != null) {
      AuthenticationProperties authProperties = new() {
        AllowRefresh = true, ExpiresUtc = DateTimeOffset.Now.AddDays(1), IsPersistent = true
      };
      return new SignInResult(CookieAuthenticationDefaults.AuthenticationScheme, user, authProperties);
    }

    return Unauthorized("User name or password is incorrect.");
  }

  [HttpPost("LogOut")]
  public IActionResult LogOut() {
    return new SignOutResult(CookieAuthenticationDefaults.AuthenticationScheme);
  }
}