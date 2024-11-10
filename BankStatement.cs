using System;
using System.Globalization;

// Tạo struct lưu trữ dữ liệu
public struct BankStatement
{
    public DateTime Date { get; }
    public string TransactionNo { get; }
    public decimal Credit { get; }
    public decimal Debit { get; }
    public string Detail { get; }


    // Constructor
    public BankStatement(DateTime Date, string TransactionNo, decimal Credit, decimal Debit, string Detail)
    {
        this.Date = Date;
        this.TransactionNo = TransactionNo;
        this.Credit = Credit;
        this.Debit = Debit;
        this.Detail = Detail;
    }

    // Override ToString
    public override string ToString()
    {
        return $"Date: {Date.ToString("dd/MM/yyyy")}, Transaction No: {TransactionNo}, Credit: {Credit}, Debit: {Debit}, Detail: {Detail}";
    }
}
