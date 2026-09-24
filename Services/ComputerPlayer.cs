using PAWN2.Game;
using PAWN2.Models;

namespace PAWN2.Services;

public sealed class ComputerPlayer
{
    private readonly Random rng = new();

    public Move? Choose(
        ChessGame game,
        ComputerProfile profile,
        DifficultyLevel difficulty)
    {
        var legalMoves =
            game
                .LegalMoves(game.Turn)
                .ToList();

        if (legalMoves.Count == 0)
        {
            return null;
        }

        if (legalMoves.Count == 1)
        {
            return legalMoves[0];
        }

        var scored =
            new List<(Move Move, double Score)>();

        foreach (var move in legalMoves)
        {
            var score =
                EvaluateMove(
                    game,
                    move,
                    profile,
                    difficulty);

            scored.Add(
                (move, score));
        }

        scored.Sort(
            (a, b) =>
                b.Score.CompareTo(a.Score));

        var selectionRange =
            CalculateSelectionRange(
                scored.Count,
                profile,
                difficulty);

        /*
         * EASY
         *
         * The personality has much more control over the result.
         * The computer can deliberately choose a weaker move.
         */
        if (difficulty == DifficultyLevel.Easy)
        {
            return scored[
                rng.Next(selectionRange)
            ].Move;
        }

        /*
         * NORMAL
         *
         * Usually chooses from the strongest few moves.
         */
        if (difficulty == DifficultyLevel.Normal)
        {
            return scored[
                rng.Next(selectionRange)
            ].Move;
        }

        /*
         * HARD
         *
         * Strong preference for the top move.
         */
        if (difficulty == DifficultyLevel.Hard)
        {
            if (rng.Next(100) < 82)
            {
                return scored[0].Move;
            }

            return scored[
                rng.Next(
                    Math.Min(
                        2,
                        scored.Count))
            ].Move;
        }

        /*
         * BRUTAL
         *
         * Almost always takes the strongest evaluated move.
         *
         * Personality still influences the evaluation, but
         * randomness is dramatically reduced.
         */
        if (rng.Next(100) < 96)
        {
            return scored[0].Move;
        }

        return scored[
            rng.Next(
                Math.Min(
                    2,
                    scored.Count))
        ].Move;
    }

    private int CalculateSelectionRange(
        int moveCount,
        ComputerProfile p,
        DifficultyLevel difficulty)
    {
        int range;

        switch (difficulty)
        {
            case DifficultyLevel.Easy:

                if (p.Chaos >= 90)
                {
                    range = 8;
                }
                else if (p.Chaos >= 75)
                {
                    range = 6;
                }
                else if (p.Chaos >= 55)
                {
                    range = 5;
                }
                else if (p.Chaos >= 35)
                {
                    range = 4;
                }
                else
                {
                    range = 3;
                }

                break;

            case DifficultyLevel.Normal:

                if (p.Chaos >= 90)
                {
                    range = 5;
                }
                else if (p.Chaos >= 75)
                {
                    range = 4;
                }
                else if (p.Chaos >= 55)
                {
                    range = 3;
                }
                else
                {
                    range = 2;
                }

                break;

            case DifficultyLevel.Hard:

                if (p.Chaos >= 90)
                {
                    range = 3;
                }
                else if (p.Chaos >= 70)
                {
                    range = 2;
                }
                else
                {
                    range = 1;
                }

                break;

            case DifficultyLevel.Brutal:

                range = 1;

                break;

            default:

                range = 3;

                break;
        }

        /*
         * Personality modifies the final selection.
         */

        if (p.Patience >= 90)
        {
            range--;
        }

        if (p.Confidence >= 90)
        {
            range--;
        }

        if (p.Panic >= 80)
        {
            range++;
        }

        if (p.Ragebait >= 90)
        {
            range++;
        }

        if (p.NoFear)
        {
            range++;
        }

        if (p.Coward)
        {
            range--;
        }

        /*
         * Difficulty always has the final say.
         *
         * Brutal cannot become completely random because of
         * a chaotic personality.
         */
        if (difficulty == DifficultyLevel.Brutal)
        {
            range = Math.Min(range, 2);
        }

        if (difficulty == DifficultyLevel.Hard)
        {
            range = Math.Min(range, 3);
        }

        return Math.Clamp(
            range,
            1,
            Math.Min(
                range,
                moveCount));
    }

    private double EvaluateMove(
        ChessGame game,
        Move move,
        ComputerProfile p,
        DifficultyLevel difficulty)
    {
        double score = 0;

        var piece =
            game.Board[move.From];

        if (piece is null)
        {
            return -999999;
        }

        /*
         * Difficulty changes how strongly the computer values
         * material.
         */
        var materialMultiplier =
            difficulty switch
            {
                DifficultyLevel.Easy => 0.86,
                DifficultyLevel.Normal => 1.00,
                DifficultyLevel.Hard => 1.12,
                DifficultyLevel.Brutal => 1.22,
                _ => 1.00
            };

        /*
         * MATERIAL
         */

        if (move.Captured is not null)
        {
            var capturedValue =
                PieceValue(
                    move.Captured.Type);

            score +=
                capturedValue *
                materialMultiplier;

            score +=
                p.Greed * 0.30;

            if (p.Greedy)
            {
                score +=
                    capturedValue * 0.40;
            }

            if (p.Greed >= 80)
            {
                score +=
                    capturedValue * 0.15;
            }

            if (move.Captured.Type ==
                PieceType.Queen)
            {
                score +=
                    150 *
                    materialMultiplier;

                if (p.Greed >= 80)
                {
                    score += 50;
                }
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

            score -=
                AttackExposure(
                    game,
                    move) * 1.2;
        }

        /*
         * POTATO FANATIC
         */

        if (piece.Type ==
            PieceType.Pawn &&
            p.PotatoFanatic)
        {
            score += 10;

            if (p.Drama >= 75)
            {
                score += 5;
            }
        }

        /*
         * RISK
         */

        var exposure =
            AttackExposure(
                game,
                move);

        if (p.Coward)
        {
            score -=
                exposure *
                (1.0 +
                 p.Panic / 100.0);
        }

        if (p.NoFear)
        {
            score +=
                exposure *
                (0.35 +
                 p.Risk / 200.0);
        }

        if (p.Risk >= 75)
        {
            score +=
                rng.NextDouble() *
                DifficultyNoise(
                    difficulty,
                    15);
        }

        if (p.Risk >= 90)
        {
            score +=
                exposure * 0.75;
        }

        /*
         * EGO
         */

        if (p.Ego >= 80)
        {
            score +=
                AttackPressure(
                    game,
                    move) *
                0.55;
        }

        if (p.Ego >= 92)
        {
            score +=
                AttackPressure(
                    game,
                    move) *
                0.40;
        }

        /*
         * CONFIDENCE
         */

        if (p.Confidence >= 80)
        {
            score +=
                AttackPressure(
                    game,
                    move) *
                0.20;
        }

        if (p.Confidence >= 92)
        {
            score +=
                rng.NextDouble() *
                DifficultyNoise(
                    difficulty,
                    3);
        }

        if (p.Confidence <= 30)
        {
            score -=
                AttackPressure(
                    game,
                    move) *
                0.30;
        }

        /*
         * PANIC
         */

        if (p.Panic >= 70)
        {
            if (move.Captured is not null)
            {
                score +=
                    8 +
                    p.Panic * 0.10;
            }

            score +=
                rng.NextDouble() *
                DifficultyNoise(
                    difficulty,
                    p.Panic / 12.0);
        }

        /*
         * RAGEBAIT
         */

        if (p.Ragebait >= 75)
        {
            score +=
                AttackPressure(
                    game,
                    move) *
                0.65;
        }

        if (p.Ragebait >= 90)
        {
            score +=
                exposure *
                0.50;

            score +=
                rng.NextDouble() *
                DifficultyNoise(
                    difficulty,
                    8);
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

            if (p.Drama >= 90)
            {
                score +=
                    rng.NextDouble() *
                    DifficultyNoise(
                        difficulty,
                        10);
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

        if (p.Greed >= 90 &&
            move.Captured is not null)
        {
            score += 20;
        }

        /*
         * PATIENCE
         */

        if (p.Patience >= 80 &&
            move.Captured is null)
        {
            score += 3;
        }

        if (p.Patience >= 90 &&
            move.Captured is null)
        {
            score += 5;
        }

        /*
         * CHAOS
         *
         * Chaos has much less effect on BRUTAL.
         */
        if (p.Chaos >= 75)
        {
            score +=
                rng.NextDouble() *
                DifficultyNoise(
                    difficulty,
                    22);
        }

        if (p.Chaos >= 92)
        {
            score +=
                rng.Next(
                    DifficultyRandomMin(
                        difficulty,
                        -12),
                    DifficultyRandomMax(
                        difficulty,
                        18));
        }

        /*
         * HELPFULNESS
         */

        if (p.Helpfulness >= 35)
        {
            score +=
                Math.Max(
                    0,
                    p.Patience - 50) *
                0.05;
        }

        /*
         * SPECIAL PERSONALITY COMBINATIONS
         */

        if (p.Risk >= 75 &&
            p.Confidence >= 75)
        {
            score +=
                AttackPressure(
                    game,
                    move) *
                0.45;
        }

        if (p.Risk <= 30 &&
            p.Patience >= 75)
        {
            score -=
                exposure *
                0.45;
        }

        if (p.Anger >= 75 &&
            p.Ragebait >= 75)
        {
            score +=
                AttackPressure(
                    game,
                    move) *
                0.50;
        }

        if (p.Greed >= 80 &&
            p.Confidence >= 80 &&
            move.Captured is not null)
        {
            score += 15;
        }

        if (p.Panic >= 70 &&
            p.Coward)
        {
            score -=
                exposure *
                0.75;
        }

        if (p.Chaos >= 80 &&
            p.NoFear)
        {
            score +=
                rng.NextDouble() *
                DifficultyNoise(
                    difficulty,
                    20);
        }

        if (p.HorseObsessed &&
            p.Chaos >= 70 &&
            piece.Type == PieceType.Knight)
        {
            score +=
                rng.NextDouble() *
                DifficultyNoise(
                    difficulty,
                    12);
        }

        if (p.PotatoFanatic &&
            p.Drama >= 75 &&
            piece.Type == PieceType.Pawn)
        {
            score +=
                rng.NextDouble() *
                DifficultyNoise(
                    difficulty,
                    10);
        }

        /*
         * Final personality randomness.
         *
         * EASY gets a lot.
         * BRUTAL gets almost none.
         */
        score +=
            rng.NextDouble() *
            DifficultyNoise(
                difficulty,
                5);

        return score;
    }

    private static double DifficultyNoise(
        DifficultyLevel difficulty,
        double maximum)
    {
        return difficulty switch
        {
            DifficultyLevel.Easy =>
                maximum * 1.80,

            DifficultyLevel.Normal =>
                maximum,

            DifficultyLevel.Hard =>
                maximum * 0.45,

            DifficultyLevel.Brutal =>
                maximum * 0.12,

            _ =>
                maximum
        };
    }

    private static int DifficultyRandomMin(
        DifficultyLevel difficulty,
        int value)
    {
        return difficulty switch
        {
            DifficultyLevel.Easy =>
                value,

            DifficultyLevel.Normal =>
                Math.Max(
                    -8,
                    value),

            DifficultyLevel.Hard =>
                Math.Max(
                    -3,
                    value),

            DifficultyLevel.Brutal =>
                0,

            _ =>
                value
        };
    }

    private static int DifficultyRandomMax(
        DifficultyLevel difficulty,
        int value)
    {
        return difficulty switch
        {
            DifficultyLevel.Easy =>
                value,

            DifficultyLevel.Normal =>
                Math.Min(
                    12,
                    value),

            DifficultyLevel.Hard =>
                Math.Min(
                    6,
                    value),

            DifficultyLevel.Brutal =>
                1,

            _ =>
                value
        };
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

        if (piece.Type ==
            PieceType.Bishop)
        {
            value += 1.0;
        }

        if (piece.Type ==
            PieceType.Pawn)
        {
            value += 0.4;
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
            PieceType.King => 4.0,
            _ => 0
        };
    }
}
