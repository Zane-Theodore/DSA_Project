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

    // Hàm xuất toàn bộ dữ liệu
    public static void DisplayStatements(List<BankStatement> statements)
    {
        Console.WriteLine("Danh sách sao kê ngân hàng đã đọc từ file CSV:");
        foreach (var statement in statements)
        {
            Console.WriteLine(statement.ToString());
        }
    }
}
