using AventStack.ExtentReports;
using NUnit.Framework.Interfaces;

namespace PlaywrightTests.Utilities
{
    public class BaseTest
    {
        [SetUp]
        public void Setup()
        {
            // Lấy tên của Test Case hiện tại đang chạy trong NUnit
            string testName = TestContext.CurrentContext.Test.Name;

            // Tạo một mục (node) mới trong file báo cáo HTML
            ReportManager.CreateTest(testName);
            ReportManager.CurrentTest.Log(Status.Info, "Bắt đầu khởi tạo Trình duyệt");

        }

        [TearDown]
        public void TearDown()
        {
            // 1. Lấy kết quả chạy test từ NUnit (Pass, Fail, hay Skip)
            var status = TestContext.CurrentContext.Result.Outcome.Status;
            var errorMessage = TestContext.CurrentContext.Result.Message;

            // 2. Ghi log tương ứng vào ExtentReports
            ReportManager.CurrentTest.Log(status == TestStatus.Passed ? Status.Pass : Status.Fail,
                                       status == TestStatus.Passed ? "Test Thành Công!" : $"Test Thất Bại: {errorMessage}");

            if (status == TestStatus.Failed)
            {
                try
                {
                    // Ép kiểu IWebDriver sang ITakesScreenshot để sử dụng chức năng chụp ảnh
                    ITakesScreenshot screenshotDriver = (ITakesScreenshot)driver;
                    Screenshot screenshot = screenshotDriver.GetScreenshot();

                    // Lấy ảnh dưới dạng chuỗi Base64 (Cực kỳ hữu ích để nhúng thẳng vào file HTML)
                    string base64Image = screenshot.AsBase64EncodedString;

                    // Đính kèm ảnh vào báo cáo ExtentReports
                    ReportManager.CurrentTest.AddScreenCaptureFromBase64String(base64Image, "Ảnh chụp màn hình lúc lỗi");
                }
                catch (Exception ex)
                {
                    // Bắt lỗi dự phòng trường hợp trình duyệt crash không thể chụp ảnh
                    ReportManager.CurrentTest.Log(Status.Warning, $"Không thể chụp màn hình. Lỗi: {ex.Message}");
                }
            }
            // LUÔN LUÔN dọn dẹp driver sau khi test xong để tránh treo process Chrome ngầm
            if (driver != null)
            {
                driver.Quit(); // Đóng toàn bộ cửa sổ và kết thúc session
                driver.Dispose(); // Giải phóng tài nguyên
            }
        }
    }
}
