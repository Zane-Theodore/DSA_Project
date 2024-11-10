using System;
using System.Collections.Generic;

class Run
{
    static void Main(string[] args)
    {
        // Đường dẫn đến file CSV
        string filePath = @"...\Project\DSA_Project\data\test.csv";

        // Gọi hàm để đọc file từ FileFunctions
        List<BankStatement> statements = Functions.ReadStatementsFromFile(filePath);

        // Gọi hàm để xuất dữ liệu từ FileFunctions
        Functions.DisplayStatements(statements);
    }
}
