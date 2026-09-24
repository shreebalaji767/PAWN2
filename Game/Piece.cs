namespace PAWN2.Game;

public enum PieceColor { White, Black }
public enum PieceType { King, Queen, Rook, Bishop, Knight, Pawn }

public sealed class Piece
{
    public PieceType Type { get; set; }
    public PieceColor Color { get; set; }
    public bool HasMoved { get; set; }
    public Piece(PieceType type, PieceColor color, bool hasMoved = false) { Type = type; Color = color; HasMoved = hasMoved; }
    public Piece Clone() => new(Type, Color, HasMoved);
}
