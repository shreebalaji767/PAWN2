using PAWN2.Game;
using PAWN2.Models;

namespace PAWN2.Services;

public sealed class PersonalityService
{
    private readonly Random rng = new();
    private static readonly (string,string)[] Names =
    {
        ("GARY","THE HORSE LAWYER"),("KEVIN","THE LAST BRAIN CELL"),("TODD","THE DEPARTMENT OF BAD DECISIONS"),
        ("BOB","THE PROFESSIONAL MISTAKE"),("LINDA","THE QUEEN'S UNION REP"),("DEREK","THE UNLICENSED GRANDMASTER"),
        ("MELVIN","THE PAWN ACCOUNTANT"),("SANDY","THE SUSPICIOUSLY CONFIDENT ROOK")
    };
    private static readonly string[] Moods={"😇 HELPFUL","😈 EVIL","🤡 CHAOTIC","😡 PETTY","🤑 GREEDY","😱 PANICKY","🥱 SLEEPY","😎 OVERCONFIDENT","😭 DRAMATIC","🧠 ACCIDENTALLY GENIUS","😈 RAGEBAITER"};
    private static readonly string[] Traits={"🐴 HORSE OBSESSED","👑 QUEEN PROTECTOR","🥔 POTATO FANATIC","💰 GREEDY","💀 NO FEAR","🏃 COWARD","🎭 DRAMA","😈 RAGEBAITER"};

    public ComputerProfile Create()
    {
        var n=Names[rng.Next(Names.Length)];
        var p=new ComputerProfile{ Name=n.Item1,Title=n.Item2,Mood=Moods[rng.Next(Moods.Length)],Trait=Traits[rng.Next(Traits.Length)],Seed=rng.Next() };
        p.Ego=Next();p.Anger=Next();p.Panic=Next();p.Chaos=Next();p.Drama=Next();p.Helpfulness=Next();p.Risk=Next();p.Confidence=Next();p.Ragebait=Next();p.Greed=Next();p.Patience=Next();
        return p;
    }
    private int Next()=>rng.Next(5,101);
    public void PlayerCapture(ComputerProfile p,PieceType type){p.Anger=Math.Min(100,p.Anger+(type==PieceType.Queen?28:8));p.Ego=Math.Max(0,p.Ego-(type==PieceType.Queen?24:5));p.Panic=Math.Min(100,p.Panic+(type==PieceType.Queen?30:4));p.Ragebait=Math.Min(100,p.Ragebait+5);}
    public void Checked(ComputerProfile p){p.Panic=Math.Min(100,p.Panic+18);p.Anger=Math.Min(100,p.Anger+6);p.Ego=Math.Max(0,p.Ego-4);}
    public void StrongMove(ComputerProfile p){p.Confidence=Math.Min(100,p.Confidence+4);p.Ego=Math.Min(100,p.Ego+3);p.Panic=Math.Max(0,p.Panic-3);}
}
