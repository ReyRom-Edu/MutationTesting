﻿using TransactionLib;

public class TransactionAnalyzerTests
{
    [Fact]
    public void RejectsUnknownTransactionType()
    {
        var tx = new Transaction
        {
            Amount = 100,
            Kind = TransactionKind.Unknown,
            Timestamp = DateTime.Now
        };

        Assert.StartsWith("Ошибка", TransactionAnalyzer.AnalyzeTransaction(tx));
    }

    [Fact]
    public void RejectsTooManyTransactionsForNonVip()
    {
        var tx = new Transaction
        {
            Amount = 100,
            Kind = TransactionKind.Deposit,
            DailyTransactionCount = 15,
            IsVipClient = false
        };

        Assert.Contains("превышено количество", TransactionAnalyzer.AnalyzeTransaction(tx));
    }

    [Fact]
    public void AllowsVipToTransferLargeExternal()
    {
        var tx = new Transaction
        {
            Amount = 2_000_000,
            Kind = TransactionKind.Transfer,
            IsInternal = false,
            IsVipClient = true
        };

        Assert.Equal("Транзакция допустима.", TransactionAnalyzer.AnalyzeTransaction(tx));
    }

    [Fact]
    public void AppliesCommissionForNonVipCurrentToSaving()
    {
        var tx = new Transaction
        {
            Amount = 1000,
            Kind = TransactionKind.Transfer,
            FromAccountType = "Текущий",
            ToAccountType = "Сберегательный",
            IsInternal = false,
            IsVipClient = false
        };

        Assert.StartsWith("Комиссия", TransactionAnalyzer.AnalyzeTransaction(tx));
    }

    [Fact]
    public void RejectsDailySumExceededForNonVip()
    {
        var tx = new Transaction
        {
            Amount = 300_000,
            Kind = TransactionKind.Transfer,
            DailyTransactionTotal = 1_300_000,
            IsVipClient = false
        };

        Assert.StartsWith("Ограничение: превышена", TransactionAnalyzer.AnalyzeTransaction(tx));
    }

    [Fact]
    public void RejectsLargeAtmTransfer()
    {
        var tx = new Transaction
        {
            Amount = 150_000,
            Kind = TransactionKind.Transfer,
            Channel = "ATM",
            IsInternal = true
        };

        Assert.StartsWith("Ограничение: банкоматы", TransactionAnalyzer.AnalyzeTransaction(tx));
    }

    [Fact]
    public void AllowsVipLargeExternalWithdrawal()
    {
        var tx = new Transaction
        {
            Amount = 500_000,
            Kind = TransactionKind.Withdrawal,
            IsInternal = false,
            IsVipClient = true,
            Channel = "Office"
        };

        Assert.Equal("Транзакция допустима.", TransactionAnalyzer.AnalyzeTransaction(tx));
    }

    [Fact]
    public void RejectsZeroAmount()
    {
        var tx = new Transaction
        {
            Amount = 0,
            Kind = TransactionKind.Unknown,
            IsVipClient = false,
            Channel = "Web",
            Timestamp = DateTime.Now
        };

        Assert.StartsWith("Ошибка", TransactionAnalyzer.AnalyzeTransaction(tx));
    }

}
