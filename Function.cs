using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

public class Functions
{
    // Hàm đọc file và lưu trữ dữ liệu
    public static List<BankStatement> ReadStatementsFromFile(string filePath)
    {
        List<BankStatement> statements = new List<BankStatement>();

        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                // Đọc và bỏ qua dòng đầu tiên (header)
                string? headerLine = reader.ReadLine();

                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    // Tách các trường bằng ký tự tab ('\t')
                    string[] fields = line.Split('\t');

                    // Kiểm tra số lượng trường để đảm bảo rằng dữ liệu hợp lệ
                    if (fields.Length == 5)
                    {
                        // Tạo đối tượng BankStatement từ dữ liệu trong dòng CSV
                        BankStatement statement = new BankStatement(
                            DateTime.ParseExact(fields[0], "d/M/yyyy", CultureInfo.InvariantCulture),
                            fields[1],
                            decimal.Parse(fields[2]),
                            decimal.Parse(fields[3]),
                            fields[4]
                        );

                        // Thêm đối tượng vào danh sách
                        statements.Add(statement);
                    }
                    else
                    {
                        Console.WriteLine("Lỗi: Dòng không hợp lệ hoặc thiếu dữ liệu.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Đã xảy ra lỗi khi đọc file CSV: " + ex.Message);
        }

        return statements;
    }

    // Hàm in ra menu chính của chương trình
    public static void DisplayMenu(List<BankStatement> statements)
    {
        bool running = true;

        while (running)
        {
            Console.Clear();
            // Hiển thị các lựa chọn menu
            Console.WriteLine("====== Menu ======");
            Console.WriteLine("1. Xem dữ liệu sao kê");
            Console.WriteLine("2. Tìm kiếm dữ liệu");
            Console.WriteLine("3. Sắp xếp dữ liệu");
            Console.WriteLine("4. Thống kê dữ liệu");
            Console.WriteLine("0. Thoát chương trình");
            Console.WriteLine("==================");
            Console.Write("Chọn chức năng: ");
            string choice = Console.ReadLine(); // Nhập lựa chọn từ người dùng

            // Xử lý lựa chọn
            switch (choice)
            {
                case "1":
                    DisplayStatements(statements); // Hiển thị dữ liệu sao kê
                    break;
                case "2":
                    DisplaySearchMenu(statements); // Gọi menu tìm kiếm
                    break;
                case "3":
                    DisplaySortMenu(statements); // Gọi menu sắp xếp
                    break;
                case "4":
                    DisplayStatisticsMenu(statements); // Gọi menu thống kê
                    break;
                case "0":
                    running = false; // Thoát chương trình
                    break;
                default:
                    Console.WriteLine("Lựa chọn không hợp lệ.");
                    break;
            }
        }
    }

    private static int itemsPerPage = 50;  // số lượng dữ liệu 1 trang
    private static int currentPage = 1;

    // Hàm xuất toàn bộ dữ liệu với chức năng phân trang và điều hướng
    // Hàm xuất toàn bộ dữ liệu với phân trang và điều hướng
    private static void DisplayStatements(List<BankStatement> statements)
    {
        bool running = true;

        while (running)
        {
            Console.Clear();
            ShowPage(statements);
            Console.WriteLine("\nCài đặt điều khiển:");
            Console.WriteLine("n: trang tiếp theo    p: trang trước    f: đến trang đầu    l: đến trang cuối");
            Console.WriteLine("s: đổi số lượng hiển thị trên trang    q: quay lại menu");
            Console.Write("Lựa chọn của bạn: ");
            string choice = Console.ReadLine().ToLower();
            switch (choice)
            {
                case "n":
                    Console.Clear();
                    NextPage(statements);
                    break;
                case "p":
                    Console.Clear();
                    PreviousPage();
                    break;
                case "f":
                    Console.Clear();
                    FirstPage();
                    break;
                case "l":
                    Console.Clear();
                    LastPage(statements);
                    break;
                case "s":
                    Console.Clear();
                    ChangeItemsPerPage();
                    break;
                case "q":
                    Console.Clear();
                    currentPage = 1;
                    running = false;
                    break;
                default:
                    Console.WriteLine("Lựa chọn không hợp lệ. Vui lòng thử lại.");
                    break;
            }
        }
    }
    // Hàm xuất ra bảng của trang
    private static void ShowPage(List<BankStatement> statements)
    {
        Console.Clear();
        int totalItems = statements.Count; // Tổng số bản ghi
        int totalPages = (int)Math.Ceiling(totalItems / (double)itemsPerPage); // Tính tổng số trang
        int startItem = (currentPage - 1) * itemsPerPage; // Bản ghi bắt đầu của trang hiện tại
        int endItem = Math.Min(startItem + itemsPerPage, totalItems); // Bản ghi kết thúc của trang hiện tại
                                                                      // Hiển thị thông tin tổng quát
        Console.WriteLine($"Hiển thị: [{startItem + 1} - {endItem}] / {totalItems} dữ liệu");
        Console.WriteLine($"Trang: {currentPage} / {totalPages}\n");

        // Định nghĩa chiều rộng cố định cho từng cột
        const int col1Width = 4;  // Chiều rộng cột STT
        const int col2Width = 15; // Chiều rộng cột Ngày giao dịch
        const int col3Width = 13; // Chiều rộng cột Số tiền (đủ chứa tối đa 1 tỷ với định dạng 1,000,000,000)
        const int col4Width = 100; // Chiều rộng cột Nội dung

        // In tiêu đề bảng
        Console.WriteLine(
            $"{"STT".PadRight(col1Width)}| {"Ngày giao dịch".PadRight(col2Width)}| {"Số tiền".PadRight(col3Width)} | {"Nội dung".PadRight(col4Width)}");

        // In đường kẻ ngang dưới tiêu đề
        Console.WriteLine(new string('-', col1Width + col2Width + col3Width + col4Width + 6));

        // In từng dòng dữ liệu
        for (int i = startItem; i < endItem; i++)
        {
            var statement = statements[i];
            // Định dạng từng dòng theo chiều rộng cột
            string formattedRow = $"{(i + 1).ToString().PadRight(col1Width)}| " +
                                  $"{statement.Date:dd/MM/yyyy}".PadRight(col2Width) + "| " +
                                  $"{(statement.Credit - statement.Debit):N0}".PadLeft(col3Width) + " | " +
                                  $"{statement.Detail.PadRight(col4Width)}";
            Console.WriteLine(formattedRow); // In dòng ra màn hình
        }

        // In đường kẻ ngang dưới cùng
        Console.WriteLine(new string('-', col1Width + col2Width + col3Width + col4Width + 6));
    }
    //Hàm chuyển trang
    private static void NextPage(List<BankStatement> statements)
    {
        Console.Clear();
        // Tính tổng số trang dựa trên số lượng bản ghi và số mục trên mỗi trang
        int totalPages = (int)Math.Ceiling(statements.Count / (double)itemsPerPage);
        // Kiểm tra nếu trang hiện tại nhỏ hơn tổng số trang, thì tăng trang hiện tại lên 1
        if (currentPage < totalPages)
        {
            currentPage++;
        }
    }
    private static void PreviousPage()
    {
        Console.Clear(); // Xóa màn hình trước khi hiển thị trang mới
        if (currentPage > 1)
        {
            currentPage--;
        }
        // Hiển thị trang mới
    }
    //Hàm chuyển đến trang đầu tiên
    private static void FirstPage()
    {
        Console.Clear(); // Xóa màn hình trước khi hiển thị trang mới
        currentPage = 1;

    }
    // Hàm chuyển đến trang cuối
    private static void LastPage(List<BankStatement> statements)
    {
        Console.Clear();
        // Tính tổng số trang và chuyển đến trang cuối cùng
        currentPage = (int)Math.Ceiling(statements.Count / (double)itemsPerPage);
    }
    // Hàm thay đổi số dữ liệu của 1 trang
    private static void ChangeItemsPerPage()
    {
        Console.Clear();
        // Yêu cầu người dùng nhập số lượng mục muốn hiển thị trên một trang
        Console.Write("Nhập số lượng mục hiển thị trên một trang: ");
        // Kiểm tra và gán giá trị mới cho itemsPerPage nếu người dùng nhập số nguyên dương hợp lệ
        if (int.TryParse(Console.ReadLine(), out int newItemsPerPage) && newItemsPerPage > 0)
        {
            itemsPerPage = newItemsPerPage;
            currentPage = 1;  // Đặt lại trang hiện tại về trang đầu tiên sau khi thay đổi số mục trên mỗi trang
        }
        else
        {
            Console.WriteLine("Số lượng không hợp lệ. Vui lòng nhập số nguyên dương.");
        }
    }

    // In menu cho chức năng tìm kiếm
    private static void DisplaySearchMenu(List<BankStatement> statements)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Chọn chức năng:");
            Console.WriteLine("1. Tìm kiếm sao kê theo ngày");
            Console.WriteLine("2. Tìm kiếm sao kê theo khoảng thời gian");
            Console.WriteLine("3. Tìm kiếm sao kê theo mô tả");
            Console.WriteLine("0. Thoát");
            Console.Write("Nhập lựa chọn của bạn: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    DateTime searchDate = InputDate();
                    List<BankStatement> dateResults = SearchStatementsByDate(statements, searchDate);
                    DisplayStatements(dateResults);
                    break;

                case "2":
                    Console.WriteLine("Nhập ngày bắt đầu:");
                    DateTime startDate = InputDate();
                    Console.WriteLine("Nhập ngày kết thúc:");
                    DateTime endDate = InputDate();
                    List<BankStatement> rangeResults = SearchStatementsByDateRange(statements, startDate, endDate);
                    DisplayStatements(rangeResults);
                    break;

                case "3":
                    Console.Write("Nhập từ khóa tìm kiếm: ");
                    string searchTerm = Console.ReadLine();
                    List<BankStatement> descriptionResults = SearchStatementsByDetail(statements, searchTerm);
                    DisplayStatements(descriptionResults);
                    break;

                case "0":
                    Console.WriteLine("Thoát chương trình.");
                    return;

                default:
                    Console.WriteLine("Lựa chọn không hợp lệ, vui lòng thử lại.");
                    break;
            }
        }
    }

    // Hàm nhập ngày tháng năm từ người dùng
    private static DateTime InputDate()
    {
        DateTime date;

        while (true)
        {
            Console.Write("Nhập ngày tháng năm (dd/MM/yyyy): ");
            string input = Console.ReadLine();

            // Kiểm tra và chuyển đổi chuỗi thành DateTime
            if (DateTime.TryParseExact(input, "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                break;
            }
            else
            {
                Console.WriteLine("Định dạng ngày không hợp lệ, vui lòng thử lại.");
            }
        }

        return date;
    }

    // Hàm tìm kiếm sao kê theo ngày
    private static List<BankStatement> SearchStatementsByDate(List<BankStatement> statements, DateTime date)
    {
        List<BankStatement> results = new List<BankStatement>();

        foreach (var statement in statements)
        {
            if (statement.Date.Date == date.Date)
            {
                results.Add(statement);
            }
        }

        return results;
    }

    //tìm kiếm sao kê theo khoảng thời gian
    private static List<BankStatement> SearchStatementsByDateRange(List<BankStatement> statements, DateTime startDate, DateTime endDate)
    {
        List<BankStatement> results = new List<BankStatement>();

        foreach (var statement in statements)
        {
            if (statement.Date.Date >= startDate.Date && statement.Date.Date <= endDate.Date)
            {
                results.Add(statement);
            }
        }

        return results;
    }

    // Tìm kiếm nội dung sao kê theo thông tin mô tả
    private static List<BankStatement> SearchStatementsByDetail(List<BankStatement> statements, string searchTerm)
    {
        List<BankStatement> results = new List<BankStatement>();

        foreach (var statement in statements)
        {
            if (statement.Detail.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            {
                results.Add(statement);
            }
        }

        return results;
    }

    // Hiển thi menu của chức năng xắp xếp
    private static void DisplaySortMenu(List<BankStatement> statements)
    {
        Console.Clear();
        Console.WriteLine("Sắp xếp dữ liệu:");
        Console.WriteLine("1. Theo ngày (tăng dần)");
        Console.WriteLine("2. Theo ngày (giảm dần)");
        Console.WriteLine("3. Theo số tiền (tăng dần)");
        Console.WriteLine("4. Theo số tiền (giảm dần)");
        Console.WriteLine("0. Quay lại menu chính");
        Console.Write("Chọn chức năng: ");
        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                SortByDateAscending(statements);
                break;
            case "2":
                SortByDateDescending(statements);
                break;
            case "3":
                SortByAmountAscending(statements);
                break;
            case "4":
                SortByAmountDescending(statements);
                break;
            case "0":
                return;
            default:
                Console.WriteLine("Lựa chọn không hợp lệ.");
                return;
        }
        DisplayStatements(statements);
    }

    // Gọi hàm xắp xếp theo ngày tăng dần
    private static void SortByDateAscending(List<BankStatement> statements)
    {
        MergeSort(statements, (a, b) => a.Date < b.Date ? -1 : (a.Date > b.Date ? 1 : 0));
    }

    // Gọi hàm xắp xếp theo ngày giảm dần
    private static void SortByDateDescending(List<BankStatement> statements)
    {
        MergeSort(statements, (a, b) => a.Date > b.Date ? -1 : (a.Date < b.Date ? 1 : 0));
    }

    // Gọi hàm xắp xếp theo số tiền tăng dần
    private static void SortByAmountAscending(List<BankStatement> statements)
    {
        MergeSort(statements, (a, b) =>
            (a.Credit - a.Debit) < (b.Credit - b.Debit) ? -1 :
            ((a.Credit - a.Debit) > (b.Credit - b.Debit) ? 1 : 0));
    }

    // Gọi hàm xắp xếp theo số tiền giảm dần
    private static void SortByAmountDescending(List<BankStatement> statements)
    {
        MergeSort(statements, (a, b) =>
            (a.Credit - a.Debit) > (b.Credit - b.Debit) ? -1 :
            ((a.Credit - a.Debit) < (b.Credit - b.Debit) ? 1 : 0));
    }

    // Hàm xắp xếp ứng dụng thuật toán merger sort
    private static void MergeSort(List<BankStatement> list, Comparison<BankStatement> comparison)
    {
        if (list.Count <= 1)
            return; // Không cần sắp xếp nếu danh sách có 0 hoặc 1 phần tử.

        int mid = list.Count / 2;

        // Chia danh sách thành hai nửa
        var left = new List<BankStatement>(list.GetRange(0, mid));
        var right = new List<BankStatement>(list.GetRange(mid, list.Count - mid));

        // Đệ quy sắp xếp từng nửa
        MergeSort(left, comparison);
        MergeSort(right, comparison);

        // Gộp hai nửa đã sắp xếp vào danh sách chính
        Merge(list, left, right, comparison);
    }

    // Hàm merge
    private static void Merge(List<BankStatement> list, List<BankStatement> left, List<BankStatement> right, Comparison<BankStatement> comparison)
    {
        int i = 0, j = 0, k = 0;

        // So sánh và gộp dữ liệu từ `left` và `right` vào `list`
        while (i < left.Count && j < right.Count)
        {
            if (comparison(left[i], right[j]) <= 0)
            {
                list[k++] = left[i++];
            }
            else
            {
                list[k++] = right[j++];
            }
        }

        // Sao chép các phần tử còn lại của `left` (nếu có)
        while (i < left.Count)
        {
            list[k++] = left[i++];
        }

        // Sao chép các phần tử còn lại của `right` (nếu có)
        while (j < right.Count)
        {
            list[k++] = right[j++];
        }
    }

    // Chức năng thống kê dữ liệu
    // Hiển thị menu thống kê dữ liệu
    private static void DisplayStatisticsMenu(List<BankStatement> statements)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("====== Thống kê dữ liệu ======");
            Console.WriteLine("1. Thống kê số tiền được chuyển trong từng ngày");
            Console.WriteLine("2. Thống kê khoảng tiền được gửi");
            Console.WriteLine("0. Quay lại menu chính");
            Console.WriteLine("=============================");
            Console.Write("Chọn chức năng: ");
            string choice = Console.ReadLine(); // Nhập lựa chọn từ người dùng

            switch (choice)
            {
                case "1":
                    DisplayDailyTransferStatistics(statements); // Gọi chức năng thống kê theo ngày
                    break;
                case "2":
                    DisplayAmountRangeStatistics(statements); // Gọi chức năng thống kê theo khoảng tiền
                    break;
                case "0":
                    return; // Quay lại menu chính
                default:
                    Console.WriteLine("Lựa chọn không hợp lệ. Nhấn Enter để tiếp tục.");
                    Console.ReadLine();
                    break;
            }
        }
    }

    // 1. Thống kê số tiền được chuyển trong từng ngày
    private static void DisplayDailyTransferStatistics(List<BankStatement> statements)
    {
        // Tự nhóm dữ liệu và tính tổng tiền
        Dictionary<DateTime, decimal> dailyTransfers = new Dictionary<DateTime, decimal>();
        foreach (var statement in statements)
        {
            if (dailyTransfers.ContainsKey(statement.Date.Date))
            {
                dailyTransfers[statement.Date.Date] += statement.Credit; // Cộng dồn tiền nếu ngày đã tồn tại
            }
            else
            {
                dailyTransfers[statement.Date.Date] = statement.Credit; // Thêm ngày mới
            }
        }

        // Tính tổng số tiền
        decimal totalAmount = 0;
        foreach (var total in dailyTransfers.Values)
        {
            totalAmount += total;
        }

        // Tìm độ dài lớn nhất của ngày
        int maxDateLength = 0;
        foreach (var date in dailyTransfers.Keys)
        {
            int dateLength = date.ToString("dd/MM/yyyy").Length;
            if (dateLength > maxDateLength)
            {
                maxDateLength = dateLength;
            }
        }

        // In dữ liệu
        Console.Clear();
        Console.WriteLine("=== Thống kê số tiền được chuyển trong từng ngày ===");
        foreach (var entry in dailyTransfers)
        {
            decimal percentage = totalAmount > 0 ? (entry.Value / totalAmount) * 100 : 0;
            string date = entry.Key.ToString("dd/MM/yyyy").PadRight(maxDateLength);
            Console.WriteLine($"{date} : {entry.Value:C} ({percentage:F2}%)");
        }
        Console.WriteLine($"Tổng cộng: {totalAmount:C}");
        Console.WriteLine("Nhấn Enter để quay lại.");
        Console.ReadLine();
    }

    // 2. Thống kê khoảng tiền được gửi
    private static void DisplayAmountRangeStatistics(List<BankStatement> statements)
    {
        // Định nghĩa các khoảng tiền
        var ranges = new List<(decimal Min, decimal Max)>
        {
            (0, 500_000),
            (500_000, 2_000_000),
            (2_000_000, 5_000_000),
            (5_000_000, 10_000_000),
            (10_000_000, 50_000_000),
            (50_000_000, 100_000_000),
            (100_000_000, decimal.MaxValue)
        };

        // Tự tính số lượng và tổng tiền cho mỗi khoảng
        List<(string Range, int Count, decimal Total)> rangeStats = new List<(string, int, decimal)>();
        foreach (var range in ranges)
        {
            int count = 0;
            decimal total = 0;

            foreach (var statement in statements)
            {
                if (statement.Credit >= range.Min && statement.Credit < range.Max)
                {
                    count++;
                    total += statement.Credit;
                }
            }

            string rangeLabel = $"{range.Min:C} - {(range.Max == decimal.MaxValue ? "∞" : range.Max.ToString("C"))}";
            rangeStats.Add((rangeLabel, count, total));
        }

        // Tính tổng số giao dịch và tổng số tiền
        int totalTransactions = 0;
        decimal totalAmount = 0;
        foreach (var stat in rangeStats)
        {
            totalTransactions += stat.Count;
            totalAmount += stat.Total;
        }

        // Tìm độ dài lớn nhất của chuỗi khoảng tiền
        int maxRangeLength = 0;
        foreach (var stat in rangeStats)
        {
            if (stat.Range.Length > maxRangeLength)
            {
                maxRangeLength = stat.Range.Length;
            }
        }

        // In dữ liệu
        Console.Clear();
        Console.WriteLine("=== Thống kê khoảng tiền được gửi ===");
        foreach (var stat in rangeStats)
        {
            decimal percentage = totalTransactions > 0 ? (stat.Count / (decimal)totalTransactions) * 100 : 0;
            string range = stat.Range.PadRight(maxRangeLength);
            Console.WriteLine($"{range} : {stat.Count} giao dịch ({percentage:F2}%)");
        }
        Console.WriteLine($"\nTổng số giao dịch: {totalTransactions}");
        Console.WriteLine($"Tổng số tiền: {totalAmount:C}");
        Console.WriteLine("Nhấn Enter để quay lại.");
        Console.ReadLine();
    }
}
