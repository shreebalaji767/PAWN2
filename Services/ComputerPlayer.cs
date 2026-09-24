using PAWN2.Game;
using PAWN2.Models;

namespace PAWN2.Services;

public sealed class ComputerPlayer
{
    private readonly Random rng=new();
    private static readonly Dictionary<PieceType,int> Value=new(){[PieceType.Pawn]=100,[PieceType.Knight]=320,[PieceType.Bishop]=330,[PieceType.Rook]=500,[PieceType.Queen]=900,[PieceType.King]=20000};

    public Move? Choose(ChessGame game,ComputerProfile p)
    {
        var moves=game.LegalMoves(PieceColor.Black);if(moves.Count==0)return null;
        var scored=new List<(Move move,int score)>();
        foreach(var m in moves)scored.Add((m,Score(game,m,p)));
        scored.Sort((a,b)=>b.score.CompareTo(a.score));
        int pool=Math.Clamp(1+p.Chaos/30,1,6);
        if(p.Risk>75)pool=Math.Min(moves.Count,Math.Max(pool,4));
        return scored[rng.Next(Math.Min(pool,scored.Count))].move;
    }

    private int Score(ChessGame g,Move m,ComputerProfile p)
    {
        int score=0;var moving=g.Board[m.From]!;
        if(m.Captured!=null)score+=Value[m.Captured.Type]*(10+p.Greed/8);
        if(m.Promotion.HasValue)score+=Value[m.Promotion.Value];
        if(m.IsCastle)score+=25+p.Patience/8;
        var test=g.Clone();test.Apply(m);
        if(test.IsInCheck(PieceColor.White))score+=70+p.Ego/2;
        if(test.LegalMoves(PieceColor.White).Count==0&&test.IsInCheck(PieceColor.White))score+=10000;
        score+=PositionBonus(test,moving.Color);
        if(p.Risk>60)score+=rng.Next(-45,55);
        if(p.Chaos>70)score+=rng.Next(-80,81);
        if(p.Greed>80&&m.Captured!=null)score+=80;
        if(p.Ragebait>80&&!m.IsCastle)score+=rng.Next(0,35);
        return score;
    }

    private int PositionBonus(ChessGame g,PieceColor color)
    {
        int score=0;
        for(int i=0;i<64;i++)if(g.Board[i] is { } p&&p.Color==color){int r=ChessGame.Row(i),c=ChessGame.Col(i);int center=3-Math.Min(Math.Abs(3-r),Math.Abs(4-r))+3-Math.Min(Math.Abs(3-c),Math.Abs(4-c));score+=center;}
        return score;
    }
}
