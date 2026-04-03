using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

namespace PlaywrightTests.Utilities
{
    public static class ReportManager
    {
        private static ExtentReports _extent;

        // Thuộc tính để các lớp Test gọi đến và ghi log
        public static ExtentTest CurrentTest { get; set; }

        public static void InitializeReport()
        {
            // 1. Xác định đường dẫn lưu file HTML (Lưu vào thư mục bin/Debug/...)
            string reportPath = Path.Combine(Directory.GetCurrentDirectory(), "ExtentReport.html");

            // 2. Khởi tạo SparkReporter (Giao diện HTML hiện đại của Extent)
            var sparkReporter = new ExtentSparkReporter(reportPath);
            sparkReporter.Config.DocumentTitle = "Báo Cáo Kiểm Thử Tự Động - Playwright";
            sparkReporter.Config.ReportName = "Hệ thống Demo Herokuapp";
            sparkReporter.Config.Theme = AventStack.ExtentReports.Reporter.Config.Theme.Dark;

            // 3. Đính kèm giao diện vào bộ máy Report
            _extent = new ExtentReports();
            _extent.AttachReporter(sparkReporter);

            // Thêm thông tin môi trường (Tùy chọn)
            _extent.AddSystemInfo("Người thực hiện", "Luke Nguyen");
            _extent.AddSystemInfo("Hệ điều hành", System.Environment.OSVersion.ToString());
        }

        public static ExtentTest CreateTest(string testName)
        {
            CurrentTest = _extent.CreateTest(testName);
            return CurrentTest;
        }

        public static void FlushReport()
        {
            // Lệnh bắt buộc để xuất/lưu file HTML vào ổ cứng
            _extent?.Flush();
        }
    }
}
