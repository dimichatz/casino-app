using HighLowGameApi.Core.Enums;

namespace HighLowGameApi.DTO
{
    public class StartSessionResultDTO
    {
        public Guid SessionId { get; set; }
        public int StartCardValue { get; set; }
        public SuitType StartCardSuit { get; set; }
    }
}
