namespace ChessBackend.Models
{
    public class ChessRoom
    {
        public string RoomCode { get; set; } = string.Empty;
        public string WhitePlayerConnectionId { get; set; } = string.Empty;
        public string? BlackPlayerConnectionId { get; set; }

        public bool IsFull => !string.IsNullOrEmpty(WhitePlayerConnectionId) &&
                              !string.IsNullOrEmpty(BlackPlayerConnectionId);
    }
}
