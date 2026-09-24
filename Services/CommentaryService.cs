using PAWN2.Game;
using PAWN2.Models;

namespace PAWN2.Services;

public sealed class ComputerPlayer
{
    private readonly Random rng = new();

    public Move? Choose(ChessGame game, ComputerProfile profile)
    {
        var legalMoves = game
            .LegalMoves(game.Turn)
            .ToList();

        if (legalMoves.Count == 0)
        {
            return null;
        }

        var scored = new List<(Move Move, double Score)>();

        foreach (var move in legalMoves)
        {
            double score = EvaluateMove(game, move, profile);

            // Personality noise prevents the same computer from always
            // choosing the exact same move.
            score += rng.NextDouble() * (profile.Chaos / 10.0);

            scored.Add((move, score));
        }

        scored.Sort((a, b) => b.Score.CompareTo(a.Score));

        // Usually pick one of the best moves, but chaotic personalities
        // occasionally choose a lower-ranked legal move.
        int selectionRange;

        if (profile.Chaos >= 85)
        {
            selectionRange = Math.Min(5, scored.Count);
        }
        else if (profile.Chaos >= 65)
        {
            selectionRange = Math.Min(3, scored.Count);
        }
        else
        {
            selectionRange = Math.Min(2, scored.Count);
        }

        return scored[rng.Next(selectionRange)].Move;
    }

    private double EvaluateMove(
        ChessGame game,
        Move move,
        ComputerProfile p)
    {
        double score = 0;

        var piece = game.Board[move.From];

        if (piece is null)
        {
            return -999999;
        }

        // ------------------------------------------------------------
        // CAPTURE PERSONALITY
        // ------------------------------------------------------------

        if (move.Captured is not null)
        {
            score += PieceValue(move.Captured.Type);

            score += p.Greed * 0.25;

            if (p.Greedy)
            {
                score += PieceValue(move.Captured.Type) * 0.35;
            }

            if (move.Captured.Type == PieceType.Queen)
            {
                score += 100;
            }

            if (move.Captured.Type == PieceType.Knight &&
                p.HorseObsessed)
            {
                // Horse-obsessed computers protect their own horses,
                // so capturing the opponent's horse can also feel attractive.
                score += 15;
            }
        }

        // ------------------------------------------------------------
        // HORSE OBSESSION
        // ------------------------------------------------------------

        if (piece.Type == PieceType.Knight &&
            p.HorseObsessed)
        {
            score += 18;
        }

        // ------------------------------------------------------------
        // QUEEN PROTECTOR
        // ------------------------------------------------------------

        if (piece.Type == PieceType.Queen &&
            p.QueenProtector)
        {
            score -= 10;
        }

        // ------------------------------------------------------------
        // RISK
        // ------------------------------------------------------------

        if (move.Captured is not null)
        {
            score += p.Risk * 0.15;
        }

        // High risk personalities like moving pieces aggressively.
        if (p.Risk > 70)
        {
            score += rng.NextDouble() * 15;
        }

        // Cowards prefer safer behavior.
        if (p.Coward)
        {
            score -= AttackExposure(game, move) * 0.8;
        }

        // No-fear computers don't care about exposure.
        if (p.NoFear)
        {
            score += AttackExposure(game, move) * 0.35;
        }

        // ------------------------------------------------------------
        // CHAOS
        // ------------------------------------------------------------

        if (p.Chaos > 75)
        {
            score += rng.NextDouble() * 20;
        }

        if (p.Chaos > 90)
        {
            score += rng.Next(-8, 15);
        }

        // ------------------------------------------------------------
        // EGO / CONFIDENCE
        // ------------------------------------------------------------

        if (p.Ego > 80)
        {
            // Confident computers like moves that attack something.
            score += AttackPressure(game, move) * 0.35;
        }

        if (p.Confidence < 30)
        {
            // Nervous computers prefer quieter moves.
            score -= AttackPressure(game, move) * 0.2;
        }

        // ------------------------------------------------------------
        // RAGEBAIT
        // ------------------------------------------------------------

        if (p.Ragebait > 75)
        {
            // Prefer moves that create annoying threats.
            score += AttackPressure(game, move) * 0.45;
        }

        // ------------------------------------------------------------
        // DRAMA
        // ------------------------------------------------------------

        if (p.Drama > 75)
        {
            // Checks and captures feel "dramatic".
            if (move.Captured is not null)
            {
                score += 8;
            }
        }

        // ------------------------------------------------------------
        // GREED
        // ------------------------------------------------------------

        if (p.Greed > 75)
        {
            if (move.Captured is not null)
            {
                score += 20;
            }
        }

        // ------------------------------------------------------------
        // POTATO FANATIC
        // ------------------------------------------------------------

        if (p.PotatoFanatic &&
            piece.Type == PieceType.Pawn)
        {
            score += 10;
        }

        // ------------------------------------------------------------
        // RANDOM PERSONALITY FACTOR
        // ------------------------------------------------------------

        score += rng.NextDouble() * 5;

        return score;
    }

    private static int PieceValue(PieceType type)
    {
        return type switch
        {
            PieceType.Pawn => 100,
            PieceType.Knight => 320,
            PieceType.Bishop => 330,
            PieceType.Rook => 500,
            PieceType.Queen => 900,
            PieceType.King => 10000,
            _ => 0
        };
    }

    private static double AttackPressure(
        ChessGame game,
        Move move)
    {
        double value = 0;

        if (move.Captured is not null)
        {
            value += PieceValue(move.Captured.Type) / 100.0;
        }

        var piece = game.Board[move.From];

        if (piece is not null)
        {
            if (piece.Type == PieceType.Queen)
            {
                value += 2;
            }

            if (piece.Type == PieceType.Knight)
            {
                value += 1.5;
            }
        }

        return value;
    }

    private static double AttackExposure(
        ChessGame game,
        Move move)
    {
        var piece = game.Board[move.From];

        if (piece is null)
        {
            return 0;
        }

        double exposure = 0;

        if (piece.Type == PieceType.Queen)
        {
            exposure += 3;
        }

        if (piece.Type == PieceType.Rook)
        {
            exposure += 2;
        }

        if (piece.Type == PieceType.Knight)
        {
            exposure += 1;
        }

        return exposure;
    }
}
