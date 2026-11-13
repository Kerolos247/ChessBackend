namespace ChessBackend.Modules.Multiplayer.DTOs;

public record ChessMoveDto(
    string RoomCode,
    string From,
    string To,
    string? Promotion
);
