namespace ChessBackend.Models
{
    public record ChessMoveDto(
     string RoomCode,
     string From,       
     string To,        
     string? Promotion 
 );
}
