using Microsoft.EntityFrameworkCore.Storage;

namespace OperationIntelligence.DB;

public interface IFinanceUnitOfWork
{
    IChartOfAccountRepository ChartOfAccounts { get; }
    IFiscalYearRepository FiscalYears { get; }
    IFiscalPeriodRepository FiscalPeriods { get; }
    ICostCenterRepository CostCenters { get; }
    IJournalEntryRepository JournalEntries { get; }
    IJournalLineRepository JournalLines { get; }
    IGeneralLedgerEntryRepository GeneralLedgerEntries { get; }
    IInvoiceRepository Invoices { get; }
    IInvoiceLineRepository InvoiceLines { get; }
    IPaymentRepository Payments { get; }
    IPaymentAllocationRepository PaymentAllocations { get; }
    IAccountReceivableRepository AccountsReceivable { get; }
    IVendorBillRepository VendorBills { get; }
    IVendorBillLineRepository VendorBillLines { get; }
    IAccountPayableRepository AccountsPayable { get; }
    IExpenseRepository Expenses { get; }
    IBudgetRepository Budgets { get; }
    IBudgetLineRepository BudgetLines { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
