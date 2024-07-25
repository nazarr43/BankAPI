namespace WebAPI.Infrastructure.Data
{
    public static class SqlQueries
    {
        public const string GetTransactionCountByUserId = @"
            SELECT COUNT(*)
            FROM public.""Transactions""
            WHERE ""UserId"" = @UserId";

        public const string GetVarianceByUserId = @"
            SELECT ""Variance""
            FROM ""transactions""
            WHERE ""UserId"" = @UserId";

        public const string GetTransactionsByType = @"
            SELECT *
            FROM public.""Transactions""
            WHERE ""TransactionType"" = @TransactionType
            LIMIT @PageSize OFFSET @Offset";

        public const string GetTransactionQuartiles = @"
            SELECT 
                percentile_cont(:Percentile1) WITHIN GROUP (ORDER BY ""Amount"") AS Q1,
                percentile_cont(:Percentile2) WITHIN GROUP (ORDER BY ""Amount"") AS Q2,
                percentile_cont(:Percentile3) WITHIN GROUP (ORDER BY ""Amount"") AS Q3
            FROM public.""Transactions""";

        public const string UpsertTransaction = @"
            INSERT INTO public.""Transactions"" (""UserId"", ""Amount"", ""TransactionDate"")
            VALUES (@UserId, @Amount, @TransactionDate)
            ON CONFLICT (""UserId"") DO UPDATE
            SET ""Amount"" = @Amount, ""TransactionDate"" = @TransactionDate";
    }
}
