namespace YoutubeLinks.E2E
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class LoginTests : PageTestBase
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
    }
}