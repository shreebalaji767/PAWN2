namespace PAWN2.Game;

public sealed class Move
{
    public int From { get; init; }
    public int To { get; init; }
    public Piece? Captured { get; set; }
    public PieceType? Promotion { get; init; }
    public bool IsCastle { get; init; }
    public bool IsEnPassant { get; init; }
    public string Notation { get; set; } = "";
    public Move Clone() => new() { From=From, To=To, Captured=Captured?.Clone(), Promotion=Promotion, IsCastle=IsCastle, IsEnPassant=IsEnPassant, Notation=Notation };
}
