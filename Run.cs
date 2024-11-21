using System;
using System.Collections.Generic;

class Run
{
    static void Main()
    {
        // Đường dẫn đến file CSV
        string filePath = @"data\data.csv";
        // Thêm bộ mã hóa UTF8
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        // Gọi hàm để đọc file từ FileFunctions
        List<BankStatement> statements = Functions.ReadStatementsFromFile(filePath);
        // Chạy chương trình
        Functions.DisplayMenu(statements);
    }
}
