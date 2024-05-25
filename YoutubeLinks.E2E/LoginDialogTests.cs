namespace YoutubeLinks.E2E
{
    [TestFixture]
    public class LoginDialogTests : PageTestBase
    {
        [Test]
        public async Task LoginDialogCanLogInUser()
        {
            await LoginAsUser();
        }

        [Test]
        public async Task LoginDialogCanLogInAdmin()
        {
            await LoginAsAdmin();
        }

        [Test]
        public async Task TestLogout()
        {
            await LoginAsAdmin();
            await Logout();
        }

        [Test]
        public async Task ResendConfirmationEmail()
        {

        }

        [Test]
        public async Task ForgotPassword()
        {

        }
    }
}