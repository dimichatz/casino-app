using HighLowGameApi.Core.Enums;

namespace HighLowGameApi.Helpers
{
    public interface ICardGenerator
    {
        (int Value, SuitType Suit) DrawRandomCard();
    }
}
