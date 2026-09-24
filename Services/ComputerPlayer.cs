using PAWN2.Game;
using PAWN2.Models;

namespace PAWN2.Services;

public sealed class ComputerPlayer
{
    private readonly Random rng = new();

    public Move? Choose(
        ChessGame game,
        ComputerProfile profile)
    {
        var legalMoves =
            game
                .LegalMoves(game.Turn)
                .ToList();

        if (legalMoves.Count == 0)
        {
            return null;
        }

        var scored =
            new List<(Move Move, double Score)>();

        foreach (var move in legalMoves)
        {
            var score =
                EvaluateMove(
                    game,
                    move,
                    profile);

            score +=
                rng.NextDouble() *
                Math.Max(
                    1,
                    profile.Chaos / 8.0);

            scored.Add(
                (move, score));
        }

        scored.Sort(
            (a, b) =>
                b.Score.CompareTo(a.Score));

        int selectionRange;

        if (profile.Chaos >= 92)
        {
            selectionRange =
                Math.Min(
                    6,
                    scored.Count);
        }
        else if (profile.Chaos >= 80)
        {
            selectionRange =
                Math.Min(
                    4,
                    scored.Count);
        }
        else if (profile.Chaos >= 60)
        {
            selectionRange =
                Math.Min(
                    3,
                    scored.Count);
        }
        else
        {
            selectionRange =
                Math.Min(
                    2,
                    scored.Count);
        }

        return scored[
            rng.Next(selectionRange)
        ].Move;
    }

    private double EvaluateMove(
        ChessGame game,
        Move move,
        ComputerProfile p)
    {
        double score = 0;

        var piece =
            game.Board[move.From];

        if (piece is null)
        {
            return -999999;
        }

        /*
         * MATERIAL
         */

        if (move.Captured is not null)
        {
            var capturedValue =
                PieceValue(
                    move.Captured.Type);

            score += capturedValue;

            score +=
                p.Greed * 0.30;

            if (p.Greedy)
            {
                score +=
                    capturedValue * 0.40;
            }

            if (move.Captured.Type ==
                PieceType.Queen)
            {
                score += 150;
            }

            if (move.Captured.Type ==
                PieceType.Knight &&
                p.HorseObsessed)
            {
                score += 25;
            }

            if (move.Captured.Type ==
                PieceType.Pawn &&
                p.PotatoFanatic)
            {
                score += 18;
            }
        }

        /*
         * HORSE OBSESSION
         */

        if (piece.Type ==
            PieceType.Knight &&
            p.HorseObsessed)
        {
            score += 22;
        }

        /*
         * QUEEN PROTECTION
         */

        if (piece.Type ==
            PieceType.Queen &&
            p.QueenProtector)
        {
            score -= 8;
        }

        /*
         * POTATO FANATIC
         */

        if (piece.Type ==
            PieceType.Pawn &&
            p.PotatoFanatic)
        {
            score += 10;
        }

        /*
         * RISK
         */

        if (move.Captured is not null)
        {
            score +=
                p.Risk * 0.20;
        }

        if (p.Risk >= 75)
        {
            score +=
                rng.NextDouble() * 18;
        }

        if (p.Coward)
        {
            score -=
                AttackExposure(
                    game,
                    move) * 1.1;
        }

        if (p.NoFear)
        {
            score +=
                AttackExposure(
                    game,
                    move) * 0.45;
        }

        /*
         * EGO
         */

        if (p.Ego >= 80)
        {
            score +=
                AttackPressure(
                    game,
                    move) * 0.55;
        }

        /*
         * CONFIDENCE
         */

        if (p.Confidence >= 80)
        {
            score +=
                AttackPressure(
                    game,
                    move) * 0.20;
        }

        if (p.Confidence <= 30)
        {
            score -=
                AttackPressure(
                    game,
                    move) * 0.30;
        }

        /*
         * RAGEBAIT
         */

        if (p.Ragebait >= 75)
        {
            score +=
                AttackPressure(
                    game,
                    move) * 0.65;
        }

        /*
         * DRAMA
         */

        if (p.Drama >= 75)
        {
            if (move.Captured is not null)
            {
                score += 10;
            }

            if (piece.Type ==
                PieceType.Queen)
            {
                score += 5;
            }
        }

        /*
         * GREED
         */

        if (p.Greed >= 75 &&
            move.Captured is not null)
        {
            score += 25;
        }

        /*
         * PATIENCE
         */

        if (p.Patience >= 80 &&
            move.Captured is null)
        {
            score += 3;
        }

        /*
         * CHAOS
         */

        if (p.Chaos >= 75)
        {
            score +=
                rng.NextDouble() * 22;
        }

        if (p.Chaos >= 92)
        {
            score +=
                rng.Next(-12, 18);
        }

        /*
         * PERSONALITY NOISE
         */

        score +=
            rng.NextDouble() * 5;

        return score;
    }

    private static int PieceValue(
        PieceType type)
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
            value +=
                PieceValue(
                    move.Captured.Type) /
                100.0;
        }

        var piece =
            game.Board[move.From];

        if (piece is null)
        {
            return value;
        }

        if (piece.Type ==
            PieceType.Queen)
        {
            value += 2.5;
        }

        if (piece.Type ==
            PieceType.Knight)
        {
            value += 1.8;
        }

        if (piece.Type ==
            PieceType.Rook)
        {
            value += 1.2;
        }

        return value;
    }

    private static double AttackExposure(
        ChessGame game,
        Move move)
    {
        var piece =
            game.Board[move.From];

        if (piece is null)
        {
            return 0;
        }

        return piece.Type switch
        {
            PieceType.Queen => 3.5,
            PieceType.Rook => 2.5,
            PieceType.Bishop => 1.7,
            PieceType.Knight => 1.4,
            PieceType.Pawn => 0.5,
            _ => 0
        };
    }
}
