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

        /*
         * Only one legal move.
         */
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
                    profile);

            /*
             * Personality-specific randomness.
             *
             * High chaos means the computer is less predictable.
             * Low chaos means it tends to choose the objectively
             * better-scoring move.
             */
            var chaosNoise =
                rng.NextDouble() *
                Math.Max(
                    0.5,
                    profile.Chaos / 7.0);

            score += chaosNoise;

            /*
             * High ragebait personalities deliberately like
             * provocative attacking moves.
             */
            if (profile.Ragebait >= 80)
            {
                score +=
                    rng.NextDouble() *
                    (profile.Ragebait / 12.0);
            }

            /*
             * Very confident personalities have less randomness.
             * They believe their top choice is correct.
             */
            if (profile.Confidence >= 85)
            {
                score +=
                    rng.NextDouble() *
                    2.5;
            }

            scored.Add(
                (move, score));
        }

        scored.Sort(
            (a, b) =>
                b.Score.CompareTo(a.Score));

        var selectionRange =
            CalculateSelectionRange(
                scored.Count,
                profile);

        /*
         * Occasionally a personality will deliberately choose
         * something other than the absolute top move.
         *
         * This is what gives PAWN² its personality instead of
         * making every computer opponent play identically.
         */
        return scored[
            rng.Next(selectionRange)
        ].Move;
    }

    private int CalculateSelectionRange(
        int moveCount,
        ComputerProfile p)
    {
        /*
         * Base range from chaos.
         */
        int range;

        if (p.Chaos >= 95)
        {
            range = 7;
        }
        else if (p.Chaos >= 85)
        {
            range = 5;
        }
        else if (p.Chaos >= 70)
        {
            range = 4;
        }
        else if (p.Chaos >= 50)
        {
            range = 3;
        }
        else if (p.Chaos >= 30)
        {
            range = 2;
        }
        else
        {
            range = 1;
        }

        /*
         * Patience makes the computer more selective.
         */
        if (p.Patience >= 85)
        {
            range--;
        }

        /*
         * Confidence makes it trust the best move.
         */
        if (p.Confidence >= 90)
        {
            range--;
        }

        /*
         * Panic can make the computer choose more erratically.
         */
        if (p.Panic >= 80)
        {
            range++;
        }

        /*
         * Ragebait personalities like alternatives.
         */
        if (p.Ragebait >= 90)
        {
            range++;
        }

        /*
         * No Fear personalities are willing to choose risky
         * alternatives.
         */
        if (p.NoFear)
        {
            range++;
        }

        /*
         * Cowards prefer the safest/highest scoring option.
         */
        if (p.Coward)
        {
            range--;
        }

        return Math.Clamp(
            range,
            1,
            Math.Min(
                7,
                moveCount));
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
         * ---------------------------------------------------------
         * MATERIAL
         * ---------------------------------------------------------
         */

        if (move.Captured is not null)
        {
            var capturedValue =
                PieceValue(
                    move.Captured.Type);

            score += capturedValue;

            /*
             * General greed.
             */
            score +=
                p.Greed * 0.30;

            /*
             * Strongly greedy personality.
             */
            if (p.Greedy)
            {
                score +=
                    capturedValue * 0.40;
            }

            /*
             * High material value gets increasingly attractive
             * to materialistic personalities.
             */
            if (p.Greed >= 80)
            {
                score +=
                    capturedValue *
                    0.15;
            }

            /*
             * Queen capture is naturally extremely valuable.
             */
            if (move.Captured.Type ==
                PieceType.Queen)
            {
                score += 150;

                if (p.Greed >= 80)
                {
                    score += 50;
                }
            }

            /*
             * Horse obsession.
             */
            if (move.Captured.Type ==
                PieceType.Knight &&
                p.HorseObsessed)
            {
                score += 25;
            }

            /*
             * Potato fanatic.
             */
            if (move.Captured.Type ==
                PieceType.Pawn &&
                p.PotatoFanatic)
            {
                score += 18;
            }
        }

        /*
         * ---------------------------------------------------------
         * PIECE PERSONALITY
         * ---------------------------------------------------------
         */

        if (piece.Type ==
            PieceType.Knight &&
            p.HorseObsessed)
        {
            score += 22;
        }

        if (piece.Type ==
            PieceType.Queen &&
            p.QueenProtector)
        {
            /*
             * Queen protector dislikes throwing the queen into
             * dangerous positions.
             */
            score -= 8;

            score -=
                AttackExposure(
                    game,
                    move) * 1.2;
        }

        if (piece.Type ==
            PieceType.Pawn &&
            p.PotatoFanatic)
        {
            score += 10;

            /*
             * Stronger potato personalities get an additional
             * preference for pawn activity.
             */
            if (p.Drama >= 75)
            {
                score += 5;
            }
        }

        /*
         * ---------------------------------------------------------
         * RISK PERSONALITY
         * ---------------------------------------------------------
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
                15;
        }

        if (p.Risk >= 90)
        {
            score +=
                exposure *
                0.75;
        }

        /*
         * ---------------------------------------------------------
         * EGO
         * ---------------------------------------------------------
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
         * ---------------------------------------------------------
         * CONFIDENCE
         * ---------------------------------------------------------
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
                3;
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
         * ---------------------------------------------------------
         * PANIC
         * ---------------------------------------------------------
         */

        if (p.Panic >= 70)
        {
            /*
             * Panicked computers prefer captures because removing
             * an enemy piece feels safer.
             */
            if (move.Captured is not null)
            {
                score +=
                    8 +
                    p.Panic * 0.10;
            }

            /*
             * But extremely panicky personalities become noisy.
             */
            score +=
                rng.NextDouble() *
                (p.Panic / 12.0);
        }

        /*
         * ---------------------------------------------------------
         * RAGEBAIT
         * ---------------------------------------------------------
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
                8;
        }

        /*
         * ---------------------------------------------------------
         * DRAMA
         * ---------------------------------------------------------
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
                    10;
            }
        }

        /*
         * ---------------------------------------------------------
         * GREED
         * ---------------------------------------------------------
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
         * ---------------------------------------------------------
         * PATIENCE
         * ---------------------------------------------------------
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
         * Patient personalities dislike completely random
         * tactical nonsense.
         */
        if (p.Patience >= 85)
        {
            score -=
                rng.NextDouble() *
                2;
        }

        /*
         * ---------------------------------------------------------
         * CHAOS
         * ---------------------------------------------------------
         */

        if (p.Chaos >= 75)
        {
            score +=
                rng.NextDouble() *
                22;
        }

        if (p.Chaos >= 92)
        {
            score +=
                rng.Next(-12, 18);
        }

        /*
         * ---------------------------------------------------------
         * HELPFULNESS
         * ---------------------------------------------------------
         *
         * Helpful personalities are less interested in ragebait
         * behavior and slightly prefer sensible moves.
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
         * ---------------------------------------------------------
         * SPECIAL PERSONALITY COMBINATIONS
         * ---------------------------------------------------------
         */

        /*
         * Aggressive + confident.
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

        /*
         * Defensive + patient.
         */
        if (p.Risk <= 30 &&
            p.Patience >= 75)
        {
            score -=
                exposure *
                0.45;
        }

        /*
         * Angry + ragebaiting.
         */
        if (p.Anger >= 75 &&
            p.Ragebait >= 75)
        {
            score +=
                AttackPressure(
                    game,
                    move) *
                0.50;
        }

        /*
         * Greedy + high confidence.
         */
        if (p.Greed >= 80 &&
            p.Confidence >= 80 &&
            move.Captured is not null)
        {
            score += 15;
        }

        /*
         * Panic + coward.
         */
        if (p.Panic >= 70 &&
            p.Coward)
        {
            score -=
                exposure *
                0.75;
        }

        /*
         * Chaos + no fear.
         */
        if (p.Chaos >= 80 &&
            p.NoFear)
        {
            score +=
                rng.NextDouble() *
                20;
        }

        /*
         * Horse obsession + chaos.
         */
        if (p.HorseObsessed &&
            p.Chaos >= 70 &&
            piece.Type == PieceType.Knight)
        {
            score +=
                rng.NextDouble() *
                12;
        }

        /*
         * Potato fanatic + drama.
         */
        if (p.PotatoFanatic &&
            p.Drama >= 75 &&
            piece.Type == PieceType.Pawn)
        {
            score +=
                rng.NextDouble() *
                10;
        }

        /*
         * Final small personality noise.
         */
        score +=
            rng.NextDouble() *
            5;

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
