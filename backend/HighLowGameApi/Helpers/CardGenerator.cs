using System.Security.Cryptography;
using HighLowGameApi.Core.Enums;

namespace HighLowGameApi.Helpers
{
    public class CardGenerator : ICardGenerator
    {
        public (int Value, SuitType Suit) DrawRandomCard()
        {
            int value = RandomNumberGenerator.GetInt32(1, 14);
            var suit = (SuitType)RandomNumberGenerator.GetInt32(1, 5);

            return (value, suit);
        }
    }
}
