using PAWN2.Game;
using PAWN2.Models;

namespace PAWN2.Services;

public sealed class PersonalityService
{
    private readonly Random rng = new();

    private static readonly string[] Names =
    {
        "GARY",
        "KEVIN",
        "TODD",
        "BOB",
        "DEREK",
        "BRIAN",
        "STEVE",
        "CHAD",
        "CRAIG",
        "DOUG",
        "MIKE",
        "PHIL",
        "TREVOR",
        "GREG",
        "RICHARD",
        "DENNIS",
        "STAN",
        "WALTER",
        "CLIVE",
        "MARTIN",
        "FRANK",
        "DAVE",
        "RON",
        "BARRY",
        "NORMAN",
        "LARRY"
    };

    private static readonly string[] Titles =
    {
        "THE HORSE LAWYER",
        "THE LAST BRAIN CELL",
        "THE DEPARTMENT OF BAD DECISIONS",
        "THE PROFESSIONAL MISTAKE",
        "THE MIDDLE MANAGER",
        "THE UNPAID INTERN",
        "THE CHESS ACCOUNTANT",
        "THE CEO OF LOSING",
        "THE VICE PRESIDENT OF CHAOS",
        "THE SENIOR POTATO ANALYST",
        "THE DIRECTOR OF QUESTIONABLE MOVES",
        "THE REGIONAL RAGE MANAGER",
        "THE HUMAN BLUNDER",
        "THE NIGHT SHIFT GRANDMASTER",
        "THE OFFICE CHESS EXPERT",
        "THE KING'S PERSONAL LAWYER",
        "THE CHIEF OF BAD IDEAS",
        "THE MANAGER OF UNNECESSARY DRAMA",
        "THE DIRECTOR OF HORSE AFFAIRS",
        "THE DEPARTMENT OF FREE MATERIAL",
        "THE CORPORATE BLUNDER SPECIALIST"
    };

    private static readonly string[] Traits =
    {
        "😎 Overconfident",
        "🤡 Chaotic",
        "😈 Ragebaiter",
        "😭 Dramatic",
        "🤑 Greedy",
        "😡 Petty",
        "🧠 Accidentally Genius",
        "😱 Panicky",
        "🥱 Sleepy",
        "🤓 Chess Nerd",
        "🧂 Extremely Salty",
        "🧐 Suspicious",
        "💀 Merciless",
        "🤝 Fake Friendly",
        "🤯 Easily Confused",
        "🏃 Professional Coward",
        "🎭 Loves Attention",
        "🧠 Has One Good Idea",
        "😈 Professionally Annoying"
    };

    private static readonly string[] HiddenTraits =
    {
        "🐴 Horse Obsessed",
        "👑 Queen Protector",
        "🥔 Potato Fanatic",
        "💰 Greedy",
        "💀 No Fear",
        "🏃 Coward",
        "🎭 Drama",
        "😈 Ragebaiter"
    };

    private static readonly string[] Moods =
    {
        "😇 HELPFUL",
        "😈 EVIL",
        "🤡 CHAOTIC",
        "😡 PETTY",
        "🤑 GREEDY",
        "😱 PANICKY",
        "🥱 SLEEPY",
        "😎 OVERCONFIDENT",
        "😭 DRAMATIC",
        "🧠 ACCIDENTALLY GENIUS",
        "😈 RAGEBAITER",
        "🧂 SALTY"
    };

    public ComputerProfile Create()
    {
        var p =
            new ComputerProfile
            {
                Seed = rng.Next(),

                Name = Pick(Names),
                Title = Pick(Titles),
                Trait = Pick(Traits),
                HiddenTrait = Pick(HiddenTraits),
                Mood = Pick(Moods),

                Ego = rng.Next(35, 96),
                Anger = rng.Next(5, 70),
                Panic = rng.Next(5, 55),
                Chaos = rng.Next(20, 101),
                Drama = rng.Next(15, 96),
                Ragebait = rng.Next(15, 101),

                Greed = rng.Next(15, 91),
                Patience = rng.Next(10, 91),
                Helpfulness = rng.Next(0, 41),
                Risk = rng.Next(10, 96),
                Confidence = rng.Next(30, 96)
            };

        ApplyHiddenTrait(p);

        p.Clamp();

        return p;
    }

    private void ApplyHiddenTrait(
        ComputerProfile p)
    {
        switch (p.HiddenTrait)
        {
            case "🐴 Horse Obsessed":

                p.HorseObsessed = true;
                p.Chaos += 10;
                p.Ego += 5;
                p.Patience -= 5;

                break;

            case "👑 Queen Protector":

                p.QueenProtector = true;
                p.Patience += 15;
                p.Risk -= 10;
                p.Confidence += 5;

                break;

            case "🥔 Potato Fanatic":

                p.PotatoFanatic = true;
                p.Drama += 15;
                p.Greed -= 5;

                break;

            case "💰 Greedy":

                p.Greedy = true;
                p.Greed += 30;
                p.Risk += 15;

                break;

            case "💀 No Fear":

                p.NoFear = true;
                p.Risk += 25;
                p.Panic -= 15;
                p.Confidence += 8;

                break;

            case "🏃 Coward":

                p.Coward = true;
                p.Patience += 20;
                p.Risk -= 25;
                p.Panic += 10;
                p.Confidence -= 5;

                break;

            case "🎭 Drama":

                p.DramaQueen = true;
                p.Drama += 30;
                p.Patience -= 10;

                break;

            case "😈 Ragebaiter":

                p.Ragebaiter = true;
                p.Ragebait += 25;
                p.Chaos += 15;
                p.Drama += 10;

                break;
        }

        p.Clamp();
    }

    public void PlayerMove(
        ComputerProfile p)
    {
        p.PlayerMoves++;

        if (p.PlayerMoves > 20)
        {
            p.Patience -= 1;
            p.Drama += 1;
        }

        if (p.PlayerMoves % 7 == 0)
        {
            p.Anger -= 3;
        }

        if (p.PlayerMoves % 11 == 0)
        {
            p.Patience -= 2;
        }

        p.Clamp();
    }

    public void PlayerCapture(
        ComputerProfile p,
        PieceType type)
    {
        p.PlayerCaptures++;

        p.ConsecutivePlayerCaptures++;

        p.ConsecutiveComputerCaptures = 0;

        switch (type)
        {
            case PieceType.Pawn:

                p.PlayerPawnsCaptured++;

                p.Anger +=
                    p.PotatoFanatic
                        ? 20
                        : 8;

                p.Drama +=
                    p.PotatoFanatic
                        ? 12
                        : 3;

                break;

            case PieceType.Knight:

                p.PlayerKnightsCaptured++;

                p.Anger +=
                    p.HorseObsessed
                        ? 30
                        : 14;

                p.Ego -= 8;
                p.Panic += 5;

                break;

            case PieceType.Bishop:

                p.PlayerBishopsCaptured++;

                p.Anger += 12;
                p.Ego -= 6;

                break;

            case PieceType.Rook:

                p.PlayerRooksCaptured++;

                p.Anger += 15;
                p.Ego -= 8;

                break;

            case PieceType.Queen:

                p.PlayerQueensCaptured++;

                p.ComputerQueenLost = 1;

                p.QueenAlive = false;

                p.Anger += 35;
                p.Panic += 35;
                p.Ego -= 25;
                p.Drama += 30;

                break;

            case PieceType.King:

                p.Anger += 100;
                p.Panic += 100;

                break;
        }

        if (p.ConsecutivePlayerCaptures >= 3)
        {
            p.Anger += 8;
            p.Panic += 5;
            p.Ragebait += 3;
        }

        p.Clamp();
    }

    public void Checked(
        ComputerProfile p)
    {
        p.PlayerChecks++;

        p.HasBeenChecked = true;

        p.Panic += 15;
        p.Anger += 5;
        p.Confidence -= 8;
        p.Ego -= 5;

        if (p.PlayerChecks >= 3)
        {
            p.Panic += 10;
            p.Drama += 10;
        }

        p.Clamp();
    }

    public void ComputerMove(
        ComputerProfile p)
    {
        p.ComputerMoves++;

        if (p.ComputerMoves % 5 == 0)
        {
            p.Confidence += 2;
        }

        p.Clamp();
    }

    public void StrongMove(
        ComputerProfile p)
    {
        p.StrongMoves++;

        p.Ego += 7;
        p.Confidence += 10;
        p.Panic -= 5;

        if (p.StrongMoves >= 3)
        {
            p.Ego += 5;
        }

        p.Clamp();
    }

    public void ComputerCapture(
        ComputerProfile p,
        PieceType type)
    {
        p.ComputerCaptures++;

        p.ConsecutiveComputerCaptures++;

        p.ConsecutivePlayerCaptures = 0;

        p.Confidence += 5;
        p.Ego += 4;

        if (type == PieceType.Queen)
        {
            p.Ego += 15;
            p.Drama += 8;
        }

        if (p.ConsecutiveComputerCaptures >= 3)
        {
            p.Ego += 5;
            p.Confidence += 3;
        }

        p.Clamp();
    }

    public void ComputerLostPiece(
        ComputerProfile p,
        PieceType type)
    {
        switch (type)
        {
            case PieceType.Pawn:
                p.ComputerPawnsLost++;
                break;

            case PieceType.Knight:
                p.ComputerKnightsLost++;
                break;

            case PieceType.Bishop:
                p.ComputerBishopsLost++;
                break;

            case PieceType.Rook:
                p.ComputerRooksLost++;
                break;

            case PieceType.Queen:
                p.ComputerQueenLost++;
                break;
        }

        p.Anger +=
            type switch
            {
                PieceType.Queen =>
                    35,

                PieceType.Rook =>
                    15,

                PieceType.Knight =>
                    p.HorseObsessed
                        ? 30
                        : 12,

                PieceType.Bishop =>
                    10,

                _ =>
                    p.PotatoFanatic
                        ? 20
                        : 7
            };

        p.Panic +=
            type switch
            {
                PieceType.Queen =>
                    30,

                PieceType.Rook =>
                    10,

                _ =>
                    4
            };

        p.Confidence -=
            type switch
            {
                PieceType.Queen =>
                    20,

                PieceType.Rook =>
                    8,

                _ =>
                    3
            };

        p.Ego -=
            type == PieceType.Queen
                ? 20
                : 5;

        p.Clamp();
    }

    public void Blunder(
        ComputerProfile p)
    {
        p.Blunders++;

        p.Ego -= 15;
        p.Confidence -= 15;
        p.Panic += 10;
        p.Drama += 15;

        p.Clamp();
    }

    public void MissedOpportunity(
        ComputerProfile p)
    {
        p.MissedOpportunities++;

        p.Anger += 4;
        p.Drama += 4;
        p.Confidence -= 3;

        p.Clamp();
    }

    public void ResetAfterQuietTurn(
        ComputerProfile p)
    {
        if (p.Anger > 50)
        {
            p.Anger -= 1;
        }

        if (p.Panic > 40)
        {
            p.Panic -= 1;
        }

        p.Clamp();
    }

    public void UpdateMood(
        ComputerProfile p)
    {
        if (!p.QueenAlive &&
            p.Panic >= 60)
        {
            p.Mood =
                "😭 QUEENLESS PANIC";
        }
        else if (p.PlayerChecks >= 3 &&
                 p.Panic >= 65)
        {
            p.Mood =
                "😱 UNDER CONSTANT ATTACK";
        }
        else if (p.Anger >= 85)
        {
            p.Mood =
                "😡 FURIOUS";
        }
        else if (p.Ragebait >= 90)
        {
            p.Mood =
                "😈 ABSOLUTE RAGEBAITER";
        }
        else if (p.Ego >= 90 &&
                 p.Confidence >= 80)
        {
            p.Mood =
                "😎 UNBEARABLY CONFIDENT";
        }
        else if (p.Drama >= 90)
        {
            p.Mood =
                "😭 ABSOLUTELY DRAMATIC";
        }
        else if (p.Chaos >= 90)
        {
            p.Mood =
                "🤡 TOTAL CHAOS";
        }
        else if (p.Greed >= 85)
        {
            p.Mood =
                "🤑 COUNTING MATERIAL";
        }
        else if (p.Confidence <= 25)
        {
            p.Mood =
                "🥲 LOSING CONFIDENCE";
        }
        else if (p.Panic >= 60)
        {
            p.Mood =
                "😱 PANICKY";
        }
        else if (p.Anger >= 65)
        {
            p.Mood =
                "😡 GETTING ANNOYED";
        }
        else
        {
            p.Mood =
                Pick(Moods);
        }
    }

    private string Pick(
        string[] values)
    {
        return values[
            rng.Next(values.Length)];
    }
}
