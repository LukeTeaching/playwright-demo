using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace PlaywrightTests;

public class PlaywrightBaseTest : PageTest
{
    [SetUp]
    public async Task Setup()
    {
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
        // 2. Lấy trạng thái của bài test vừa chạy xong từ NUnit
        bool isFailed = TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed;

        if (isFailed)
        {
            // TẠO THƯ MỤC VÀ LƯU TRACE KHI LỖI
            string testName = TestContext.CurrentContext.Test.Name;

            // Đặt tên file zip dễ nhận diện (VD: Test_Login_trace.zip)
            string traceFileName = $"{testName}_trace.zip";
            string traceFilePath = Path.Combine(Directory.GetCurrentDirectory(), "traces", traceFileName);

            // Dừng máy quay và LƯU file xuống ổ cứng
            await Context.Tracing.StopAsync(new TracingStopOptions
            {
                Path = traceFilePath
            });

            // Ghi log ra Terminal để sinh viên biết file được lưu ở đâu
            TestContext.Progress.WriteLine($"[CẢNH BÁO] Test Fail! Đã lưu Trace tại: {traceFilePath}");
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