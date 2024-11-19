namespace YoutubeLinks.IntegrationTests.Features.Users.Commands;

public class LoginFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Login_ShouldSucceed_WhenAdminCredentialsAreValid()
    {
        await LoginAsAdmin();
    }
    
    [Fact]
    public async Task Login_ShouldSucceed_WhenUserCredentialsAreValid()
    {
        await LoginAsUser();
    }
}