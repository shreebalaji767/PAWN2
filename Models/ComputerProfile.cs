namespace PAWN2.Models;

public sealed class ComputerProfile
{
    public int Seed { get; set; }

    public string Name { get; set; } = "";
    public string Title { get; set; } = "";
    public string Mood { get; set; } = "";

    public string Trait { get; set; } = "";
    public string HiddenTrait { get; set; } = "";

    public int Ego { get; set; }
    public int Anger { get; set; }
    public int Panic { get; set; }
    public int Chaos { get; set; }
    public int Drama { get; set; }
    public int Ragebait { get; set; }

    public int Greed { get; set; }
    public int Patience { get; set; }
    public int Helpfulness { get; set; }
    public int Risk { get; set; }
    public int Confidence { get; set; }

    public int PlayerCaptures { get; set; }
    public int PlayerChecks { get; set; }
    public int ComputerCaptures { get; set; }

    public int PlayerPawnsCaptured { get; set; }
    public int PlayerKnightsCaptured { get; set; }
    public int PlayerQueensCaptured { get; set; }
    public int PlayerRooksCaptured { get; set; }
    public int PlayerBishopsCaptured { get; set; }

    public int ComputerPawnsLost { get; set; }
    public int ComputerKnightsLost { get; set; }
    public int ComputerQueenLost { get; set; }
    public int ComputerRooksLost { get; set; }
    public int ComputerBishopsLost { get; set; }

    public int PlayerMoves { get; set; }
    public int ComputerMoves { get; set; }

    public int ConsecutivePlayerCaptures { get; set; }
    public int ConsecutiveComputerCaptures { get; set; }

    public int MissedOpportunities { get; set; }
    public int Blunders { get; set; }
    public int StrongMoves { get; set; }

    public bool QueenAlive { get; set; } = true;
    public bool HasBeenChecked { get; set; }

    public bool HorseObsessed { get; set; }
    public bool QueenProtector { get; set; }
    public bool PotatoFanatic { get; set; }
    public bool Greedy { get; set; }
    public bool NoFear { get; set; }
    public bool Coward { get; set; }
    public bool DramaQueen { get; set; }
    public bool Ragebaiter { get; set; }

    public Queue<string> RecentCommentary { get; } = new();

    public void Clamp()
    {
        Ego = Math.Clamp(Ego, 0, 100);
        Anger = Math.Clamp(Anger, 0, 100);
        Panic = Math.Clamp(Panic, 0, 100);
        Chaos = Math.Clamp(Chaos, 0, 100);
        Drama = Math.Clamp(Drama, 0, 100);
        Ragebait = Math.Clamp(Ragebait, 0, 100);

        Greed = Math.Clamp(Greed, 0, 100);
        Patience = Math.Clamp(Patience, 0, 100);
        Helpfulness = Math.Clamp(Helpfulness, 0, 100);
        Risk = Math.Clamp(Risk, 0, 100);
        Confidence = Math.Clamp(Confidence, 0, 100);
    }
}
