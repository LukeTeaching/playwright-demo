using AventStack.ExtentReports;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using PlaywrightTests.Utilities;
using System.IO;
using System.Threading.Tasks;

namespace PlaywrightTests;

public class PlaywrightBaseTest : PageTest
{
    [SetUp]
    public async Task Setup()
    {
        // Lấy tên của Test Case hiện tại đang chạy trong NUnit
        string testName = TestContext.CurrentContext.Test.Name;

        // Tạo một mục (node) mới trong file báo cáo HTML
        ReportManager.CreateTest(testName);
        ReportManager.CurrentTest.Log(Status.Info, "Bắt đầu khởi tạo Trình duyệt");
        // 1. Luôn luôn BẬT máy quay phim ngay khi test bắt đầu
        // Vì ta không thể biết trước test sẽ Pass hay Fail
        await Context.Tracing.StartAsync(new TracingStartOptions
        {
            Screenshots = true, // Chụp ảnh liên tục
            Snapshots = true,   // Lưu lại cấu trúc DOM HTML
            Sources = true      // Lưu lại mã nguồn C# đang chạy
        });
    }

    [TearDown]
    public async Task TearDown()
    {
        // 1. Lấy kết quả chạy test từ NUnit (Pass, Fail, hay Skip)
        var status = TestContext.CurrentContext.Result.Outcome.Status;
        var errorMessage = TestContext.CurrentContext.Result.Message;

        // 2. Ghi log tương ứng vào ExtentReports
        ReportManager.CurrentTest.Log(status == TestStatus.Passed ? Status.Pass : Status.Fail,
                                   status == TestStatus.Passed ? "Test Thành Công!" : $"Test Thất Bại: {errorMessage}");

        if (status == TestStatus.Failed)
        {
            // Chụp ảnh màn hình bằng Playwright cực nhàn
            // Không cần ép kiểu (ITakesScreenshot) rườm rà như Selenium
            byte[] screenshotBytes = await Page.ScreenshotAsync(new()
            {
                FullPage = true // Chụp toàn bộ trang 
            });
            string base64Image = Convert.ToBase64String(screenshotBytes);
            ReportManager.CurrentTest.AddScreenCaptureFromBase64String(base64Image, "Ảnh lỗi toàn màn hình");

            // Lưu file Trace 
            string traceFileName = $"{TestContext.CurrentContext.Test.Name}_trace.zip";
            string traceDir = Path.Combine(Directory.GetCurrentDirectory(), "Traces");
            Directory.CreateDirectory(traceDir); // Đảm bảo thư mục tồn tại

            string traceFilePath = Path.Combine(traceDir, traceFileName);
            await Context.Tracing.StopAsync(new() { Path = traceFilePath });

            // Gắn link tải file Trace thẳng vào báo cáo HTML
            // Chỉ cần bấm vào link này trong report là tải được file zip về debug
            ReportManager.CurrentTest.Log(Status.Info, $"<a href='./Traces/{traceFileName}' target='_blank'>📦 Tải file Trace Viewer để Debug chi tiết</a>");
        }
        else
        {
            // TEST PASS: DỪNG MÁY QUAY NHƯNG KHÔNG LƯU
            // Quan trọng: Phải gọi StopAsync nhưng không truyền Path vào.
            // Playwright sẽ tự động vứt bỏ cuộn băng, giải phóng RAM và không tốn ổ cứng.
            await Context.Tracing.StopAsync();
        }
    }
}