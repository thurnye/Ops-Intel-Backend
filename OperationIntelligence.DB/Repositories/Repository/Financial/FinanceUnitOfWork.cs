using Microsoft.EntityFrameworkCore.Storage;

namespace OperationIntelligence.DB;

public class FinanceUnitOfWork : IFinanceUnitOfWork
{
    private readonly OperationIntelligenceDbContext _context;

    public FinanceUnitOfWork(
        OperationIntelligenceDbContext context,
        IChartOfAccountRepository chartOfAccounts,
        IFiscalYearRepository fiscalYears,
        IFiscalPeriodRepository fiscalPeriods,
        ICostCenterRepository costCenters,
        IJournalEntryRepository journalEntries,
        IJournalLineRepository journalLines,
        IGeneralLedgerEntryRepository generalLedgerEntries,
        IInvoiceRepository invoices,
        IInvoiceLineRepository invoiceLines,
        IPaymentRepository payments,
        IPaymentAllocationRepository paymentAllocations,
        IAccountReceivableRepository accountsReceivable,
        IVendorBillRepository vendorBills,
        IVendorBillLineRepository vendorBillLines,
        IAccountPayableRepository accountsPayable,
        IExpenseRepository expenses,
        IBudgetRepository budgets,
        IBudgetLineRepository budgetLines)
    {
        _context = context;
        ChartOfAccounts = chartOfAccounts;
        FiscalYears = fiscalYears;
        FiscalPeriods = fiscalPeriods;
        CostCenters = costCenters;
        JournalEntries = journalEntries;
        JournalLines = journalLines;
        GeneralLedgerEntries = generalLedgerEntries;
        Invoices = invoices;
        InvoiceLines = invoiceLines;
        Payments = payments;
        PaymentAllocations = paymentAllocations;
        AccountsReceivable = accountsReceivable;
        VendorBills = vendorBills;
        VendorBillLines = vendorBillLines;
        AccountsPayable = accountsPayable;
        Expenses = expenses;
        Budgets = budgets;
        BudgetLines = budgetLines;
    }

    public IChartOfAccountRepository ChartOfAccounts { get; }
    public IFiscalYearRepository FiscalYears { get; }
    public IFiscalPeriodRepository FiscalPeriods { get; }
    public ICostCenterRepository CostCenters { get; }
    public IJournalEntryRepository JournalEntries { get; }
    public IJournalLineRepository JournalLines { get; }
    public IGeneralLedgerEntryRepository GeneralLedgerEntries { get; }
    public IInvoiceRepository Invoices { get; }
    public IInvoiceLineRepository InvoiceLines { get; }
    public IPaymentRepository Payments { get; }
    public IPaymentAllocationRepository PaymentAllocations { get; }
    public IAccountReceivableRepository AccountsReceivable { get; }
    public IVendorBillRepository VendorBills { get; }
    public IVendorBillLineRepository VendorBillLines { get; }
    public IAccountPayableRepository AccountsPayable { get; }
    public IExpenseRepository Expenses { get; }
    public IBudgetRepository Budgets { get; }
    public IBudgetLineRepository BudgetLines { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Database.BeginTransactionAsync(cancellationToken);
    }
}
