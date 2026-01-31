using CasinoAppBackend.Attributes;

namespace CasinoAppBackend.Core.Filters
{
    public class TransactionFilterDTO
    {
        [RequireBothDates(nameof(DateEnd))]
        public DateTimeOffset? DateStart { get; set; }

        [RequireBothDates(nameof(DateStart))]
        public DateTimeOffset? DateEnd { get; set; }

        public bool? IncludeDeposits { get; set; }
        public bool? IncludeWithdrawals { get; set; }
        public bool? IncludeCasino { get; set; }
        public bool? IncludeOther { get; set; }

        public string? TransactionNumber { get; set; }
    }
}
