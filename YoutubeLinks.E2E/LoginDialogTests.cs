namespace YoutubeLinks.E2E
{
    [Parallelizable(ParallelScope.Self)]
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
        public async Task Logout()
        {

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