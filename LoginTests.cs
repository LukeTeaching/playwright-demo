using System.Threading.Tasks;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightTests;

// Kế thừa PageTest: Playwright sẽ TỰ ĐỘNG khởi tạo thuộc tính 'Page' cho mỗi Test
public class LoginTests : PageTest
{
    [Test]
    public async Task Test_ValidLogin_ShouldSucceed()
    {
        // 1. Điều hướng (Awaited)
        await Page.GotoAsync("https://the-internet.herokuapp.com/login");

        // 2. Tương tác với Auto-wait (Không cần Explicit Wait)
        // Thay vì FindElement, Playwright dùng Locator với các hàm Async
        await Page.Locator("#username").FillAsync("tomsmith");
        await Page.Locator("#password").FillAsync("SuperSecretPassword!");
        await Page.Locator("button[type='submit']").ClickAsync();

        // 3. Xác minh với Web-First Assertions
        var flashMessage = Page.Locator("#flash");
        await Expect(flashMessage).ToContainTextAsync("You logged into a secure area!");
    }
}