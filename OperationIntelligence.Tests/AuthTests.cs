using System.Net;
using System.Net.Http.Json;
using OperationIntelligence.Core;
using OperationIntelligence.Api;
using Microsoft.AspNetCore.Mvc.Testing;

[CollectionDefinition("Api collection")]
public class ApiCollection : ICollectionFixture<DatabaseFixture> {}

[Collection("Api collection")]
public class AuthTests
{
    private readonly HttpClient _client;
    private readonly DatabaseFixture _db;

    public AuthTests(DatabaseFixture db)
{
    _db = db;
    _client = db.Factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (UnitTestClient)");
    _client.DefaultRequestHeaders.Add("Referer", "https://localhost/test");
}
    [Fact]
    public async Task Register_Then_Login_Then_Profile_Succeeds()
    {
        await _db.ResetAsync();

        var register = new RegisterRequest {
            FirstName = "John", LastName = "Doe",
            Email = "john.doe@test.com", Password = "Passw0rd!",
            // Avatar = "img"
        };

        var regRes = await _client.PostAsJsonAsync("/api/auth/register", register);
        regRes.StatusCode.Should().Be(HttpStatusCode.OK);
        regRes.Headers.TryGetValues("X-Access-Token", out var registerTokenValues).Should().BeTrue();
        registerTokenValues!.First().Should().NotBeNullOrWhiteSpace();

        var login = new LoginRequest { EmailOrUserName = "john.doe@test.com", Password = "Passw0rd!" };
        var loginRes = await _client.PostAsJsonAsync("/api/auth/login", login);
        loginRes.StatusCode.Should().Be(HttpStatusCode.OK);
        loginRes.Headers.TryGetValues("X-Access-Token", out var loginTokenValues).Should().BeTrue();
        var token = loginTokenValues!.First();
        token.Should().NotBeNullOrWhiteSpace();

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/auth/profile");
        req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var profRes = await _client.SendAsync(req);
        profRes.StatusCode.Should().Be(HttpStatusCode.OK);

        var profBody = await profRes.Content.ReadFromJsonAsync<ApiResponse<object>>();
        profBody!.Data.Should().NotBeNull();
    }
}
