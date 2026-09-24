using PAWN2.Game;
using PAWN2.Models;

namespace PAWN2.Services;

public sealed class PersonalityService
{
    private readonly Random rng = new();

    /*
     * Keeps signatures of personalities already generated during
     * this browser session.
     *
     * This prevents the exact same personality combination from
     * appearing again during the current session.
     */
    private readonly HashSet<string> usedPersonalities =
        new(StringComparer.Ordinal);

    /*
     * A large collection of first names.
     */
    private static readonly string[] FirstNames =
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
        "LARRY",
        "ALAN",
        "COLIN",
        "GRAHAM",
        "HOWARD",
        "NEIL",
        "PETER",
        "SIMON",
        "BRUCE",
        "DAN",
        "ERIC",
        "JASON",
        "JUSTIN",
        "MARK",
        "PAUL",
        "RUSSELL",
        "TIM",
        "VINCE",
        "WARREN",
        "DUNCAN",
        "MARTY",
        "DOUGLAS",
        "CLARK",
        "RAY",
        "ERNIE",
        "MELVIN",
        "HAROLD",
        "PHILIP",
        "STUART",
        "LEONARD",
        "GERALD",
        "KEN",
        "SAM",
        "TERRY",
        "RONALD",
        "EDWIN",
        "FRED",
        "HUGH",
        "NIGEL",
        "PATRICK",
        "RICH",
        "SEAN",
        "ADAM",
        "ANDY",
        "JAMIE",
        "DANIEL",
        "MATTHEW",
        "RYAN",
        "SCOTT",
        "ALEX",
        "CHRIS",
        "JORDAN",
        "TYLER",
        "JOSH",
        "BEN",
        "LUKE",
        "MAX",
        "OSCAR",
        "HARRY",
        "FELIX",
        "SAMUEL",
        "OLIVER",
        "GEORGE",
        "ARTHUR",
        "EDWARD",
        "CHARLIE",
        "HENRY",
        "ALBERT",
        "REGINALD",
        "MORTON",
        "HERBERT",
        "DOUGIE",
        "WILBUR",
        "GORDON"
    };

    /*
     * Large title pool.
     */
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
        "THE CORPORATE BLUNDER SPECIALIST",
        "THE ASSISTANT TO THE ASSISTANT GRANDMASTER",
        "THE SENIOR VICE PRESIDENT OF PANIC",
        "THE HEAD OF UNNECESSARY SACRIFICES",
        "THE OFFICE POLITICS GRANDMASTER",
        "THE CHIEF POTATO OFFICER",
        "THE DIRECTOR OF BAD TIMING",
        "THE EXECUTIVE OF QUESTIONABLE STRATEGY",
        "THE HEAD OF HORSE-BASED OPERATIONS",
        "THE REGIONAL MANAGER OF CHECKMATE",
        "THE CORPORATE TACTICS INTERN",
        "THE CHIEF FINANCIAL OFFICER OF PAWNS",
        "THE DIRECTOR OF UNRECOVERABLE BLUNDERS",
        "THE SENIOR MANAGER OF TAKING FREE PIECES",
        "THE CEO OF OVERTHINKING",
        "THE HEAD OF EMOTIONAL CHESS",
        "THE DIRECTOR OF UNAUTHORIZED ATTACKS",
        "THE MINISTER OF BAD KNIGHT MOVES",
        "THE PRESIDENT OF PANIC",
        "THE CHAIRMAN OF QUESTIONABLE OPENINGS",
        "THE DEPUTY DIRECTOR OF CHAOS",
        "THE SENIOR CONSULTANT OF BLUNDERS",
        "THE GENERAL MANAGER OF HORSE VIOLENCE",
        "THE CORPORATE DIRECTOR OF CHECKS",
        "THE EXECUTIVE POTATO STRATEGIST",
        "THE HEAD OF MATERIAL ACQUISITION",
        "THE DEPARTMENT OF REVENGE",
        "THE CHIEF OPERATING OFFICER OF RAGE",
        "THE OFFICE'S UNLICENSED GRANDMASTER",
        "THE SENIOR DIRECTOR OF BAD IDEAS",
        "THE HEAD OF EXTREMELY CONFIDENT MOVES",
        "THE CORPORATE DEFENSE FAILURE SPECIALIST",
        "THE DIRECTOR OF EMERGENCY CASTLING",
        "THE CEO OF TAKING YOUR QUEEN",
        "THE MANAGER OF TACTICAL NONSENSE",
        "THE CHIEF HORSE INSPECTOR",
        "THE DIRECTOR OF PAWN POLITICS",
        "THE HEAD OF ILLEGAL-LOOKING LEGAL MOVES",
        "THE SENIOR BLUNDER CONSULTANT"
    };

    /*
     * General personality traits.
     */
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
        "😈 Professionally Annoying",
        "😐 Emotionally Unavailable",
        "🫠 Barely Functioning",
        "📊 Spreadsheet Warrior",
        "🦆 Strategically Confused",
        "🧨 Walking Tactical Accident",
        "🧊 Emotionally Cold",
        "🎯 Weirdly Accurate",
        "🪦 Remembers Every Insult",
        "🛡️ Extremely Defensive",
        "⚔️ Starts Fights",
        "🥸 Pretends To Understand",
        "👔 Corporate Menace",
        "🧙 Suspiciously Old-School",
        "🤖 Emotionally Robotic",
        "🐌 Takes Forever",
        "⚡ Impulsive",
        "🧠 Thinks Too Much",
        "🙃 Makes Everything Personal",
        "🤑 Materialistic",
        "😤 Easily Offended",
        "🤨 Never Trusts A Pawn"
    };

    /*
     * Hidden personality types.
     */
    private static readonly string[] HiddenTraits =
    {
        "🐴 Horse Obsessed",
        "👑 Queen Protector",
        "🥔 Potato Fanatic",
        "💰 Greedy",
        "💀 No Fear",
        "🏃 Coward",
        "🎭 Drama",
        "😈 Ragebaiter",
        "🧠 Calculation Addict",
        "🛡️ Defensive Specialist",
        "⚔️ Attack Addict",
        "🎯 Tactical Hunter",
        "🧊 Ice Cold",
        "🔥 Revenge Seeker",
        "📈 Material Investor",
        "🪤 Trap Addict",
        "🏰 Castle Defender",
        "👀 Suspicious Of Everything",
        "🧨 Sacrifice Enjoyer",
        "🥱 Low-Energy Strategist",
        "🧠 Opening Nerd",
        "🐌 Slow Planner",
        "⚡ Speed Freak",
        "🤑 Free-Piece Collector",
        "👑 King Protector",
        "🥔 Pawn General"
    };

    /*
     * Base moods.
     *
     * UpdateMood() can replace these dynamically during the game.
     */
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
        "🧂 SALTY",
        "🧐 SUSPICIOUS",
        "🛡️ DEFENSIVE",
        "⚔️ AGGRESSIVE",
        "🧊 ICE COLD",
        "🔥 REVENGE MODE",
        "🎯 LOCKED IN",
        "🙃 QUESTIONABLE",
        "🫠 BARELY FUNCTIONING"
    };

    /*
     * Create a new personality.
     *
     * The generator attempts many combinations until it finds
     * one that has not already appeared in this session.
     */
    public ComputerProfile Create()
    {
        const int maxAttempts = 500;

        for (int attempt = 0;
             attempt < maxAttempts;
             attempt++)
        {
            var profile = GenerateProfile();

            var signature =
                BuildSignature(profile);

            if (usedPersonalities.Add(signature))
            {
                profile.Seed = rng.Next();

                profile.Clamp();

                return profile;
            }
        }

        /*
         * The fallback is extremely unlikely to be reached,
         * but guarantees that Create() always returns a profile.
         */
        var fallback = GenerateProfile();

        fallback.Seed = rng.Next();

        fallback.Name =
            fallback.Name + " " +
            rng.Next(10, 1000);

        fallback.Clamp();

        return fallback;
    }

    private ComputerProfile GenerateProfile()
    {
        var p =
            new ComputerProfile
            {
                Seed = rng.Next(),

                Name = Pick(FirstNames),
                Title = Pick(Titles),
                Trait = Pick(Traits),
                HiddenTrait = Pick(HiddenTraits),
                Mood = Pick(Moods),

                Ego = rng.Next(25, 96),
                Anger = rng.Next(5, 76),
                Panic = rng.Next(5, 66),
                Chaos = rng.Next(10, 101),
                Drama = rng.Next(10, 101),
                Ragebait = rng.Next(5, 101),

                Greed = rng.Next(10, 96),
                Patience = rng.Next(10, 96),
                Helpfulness = rng.Next(0, 51),
                Risk = rng.Next(5, 101),
                Confidence = rng.Next(20, 101)
            };

        /*
         * First apply a personality archetype.
         *
         * This makes the numbers coherent instead of completely
         * random.
         */
        ApplyTraitPersonality(p);

        /*
         * Then apply the hidden personality.
         */
        ApplyHiddenTrait(p);

        /*
         * Small mutation prevents personalities belonging to the
         * same archetype from feeling identical.
         */
        MutateStats(p);

        p.Clamp();

        return p;
    }

    private void ApplyTraitPersonality(
        ComputerProfile p)
    {
        switch (p.Trait)
        {
            case "😎 Overconfident":

                p.Ego += 25;
                p.Confidence += 20;
                p.Risk += 15;
                p.Panic -= 10;

                break;

            case "🤡 Chaotic":

                p.Chaos += 30;
                p.Risk += 15;
                p.Patience -= 15;
                p.Ragebait += 10;

                break;

            case "😈 Ragebaiter":

                p.Ragebait += 35;
                p.Chaos += 20;
                p.Drama += 15;
                p.Ego += 10;

                break;

            case "😭 Dramatic":

                p.Drama += 35;
                p.Panic += 15;
                p.Ragebait += 10;

                break;

            case "🤑 Greedy":

                p.Greed += 35;
                p.Confidence += 10;
                p.Risk += 10;

                break;

            case "😡 Petty":

                p.Anger += 30;
                p.Ragebait += 20;
                p.Drama += 15;

                break;

            case "🧠 Accidentally Genius":

                p.Patience += 25;
                p.Confidence += 10;
                p.Chaos -= 10;
                p.Greed += 5;

                break;

            case "😱 Panicky":

                p.Panic += 35;
                p.Confidence -= 20;
                p.Risk -= 15;
                p.Patience += 10;

                break;

            case "🥱 Sleepy":

                p.Patience += 20;
                p.Chaos -= 15;
                p.Risk -= 10;
                p.Confidence -= 5;

                break;

            case "🤓 Chess Nerd":

                p.Patience += 30;
                p.Confidence += 15;
                p.Greed += 10;
                p.Chaos -= 10;

                break;

            case "🧂 Extremely Salty":

                p.Anger += 35;
                p.Drama += 20;
                p.Ragebait += 25;

                break;

            case "🧐 Suspicious":

                p.Patience += 25;
                p.Risk -= 15;
                p.Confidence += 5;

                break;

            case "💀 Merciless":

                p.Risk += 25;
                p.Greed += 20;
                p.Anger += 10;
                p.Confidence += 15;

                break;

            case "🤝 Fake Friendly":

                p.Helpfulness += 25;
                p.Patience += 10;
                p.Ragebait += 15;

                break;

            case "🤯 Easily Confused":

                p.Chaos += 35;
                p.Panic += 25;
                p.Confidence -= 20;

                break;

            case "🏃 Professional Coward":

                p.Patience += 30;
                p.Risk -= 30;
                p.Panic += 20;
                p.Confidence -= 10;

                break;

            case "🎭 Loves Attention":

                p.Drama += 30;
                p.Ragebait += 20;
                p.Chaos += 15;

                break;

            case "🧠 Has One Good Idea":

                p.Confidence += 15;
                p.Patience += 10;
                p.Chaos += 15;

                break;

            case "😈 Professionally Annoying":

                p.Ragebait += 30;
                p.Drama += 20;
                p.Chaos += 20;

                break;

            case "😐 Emotionally Unavailable":

                p.Panic -= 15;
                p.Anger -= 10;
                p.Drama -= 15;
                p.Patience += 20;

                break;

            case "🫠 Barely Functioning":

                p.Chaos += 35;
                p.Panic += 20;
                p.Confidence -= 25;

                break;

            case "📊 Spreadsheet Warrior":

                p.Patience += 30;
                p.Greed += 20;
                p.Risk -= 10;

                break;

            case "🦆 Strategically Confused":

                p.Chaos += 30;
                p.Risk += 10;
                p.Patience -= 20;

                break;

            case "🧨 Walking Tactical Accident":

                p.Chaos += 35;
                p.Risk += 35;
                p.Panic += 10;

                break;

            case "🧊 Emotionally Cold":

                p.Panic -= 20;
                p.Anger -= 10;
                p.Drama -= 20;
                p.Patience += 20;
                p.Confidence += 10;

                break;

            case "🎯 Weirdly Accurate":

                p.Confidence += 20;
                p.Patience += 20;
                p.Chaos -= 15;

                break;

            case "🪦 Remembers Every Insult":

                p.Anger += 30;
                p.Ragebait += 25;
                p.Drama += 15;

                break;

            case "🛡️ Extremely Defensive":

                p.Patience += 30;
                p.Risk -= 25;
                p.Panic -= 5;

                break;

            case "⚔️ Starts Fights":

                p.Risk += 30;
                p.Anger += 15;
                p.Chaos += 15;

                break;

            case "🥸 Pretends To Understand":

                p.Confidence += 25;
                p.Patience -= 10;
                p.Chaos += 15;

                break;

            case "👔 Corporate Menace":

                p.Ego += 20;
                p.Greed += 20;
                p.Confidence += 15;
                p.Drama += 10;

                break;

            case "🧙 Suspiciously Old-School":

                p.Patience += 35;
                p.Risk -= 5;
                p.Chaos -= 15;

                break;

            case "🤖 Emotionally Robotic":

                p.Patience += 30;
                p.Panic -= 20;
                p.Anger -= 20;
                p.Drama -= 20;

                break;

            case "🐌 Takes Forever":

                p.Patience += 35;
                p.Risk -= 10;
                p.Chaos -= 20;

                break;

            case "⚡ Impulsive":

                p.Risk += 30;
                p.Chaos += 25;
                p.Patience -= 25;

                break;

            case "🧠 Thinks Too Much":

                p.Patience += 35;
                p.Risk -= 10;
                p.Confidence += 10;

                break;

            case "🙃 Makes Everything Personal":

                p.Anger += 25;
                p.Drama += 25;
                p.Ragebait += 20;

                break;

            case "🤑 Materialistic":

                p.Greed += 40;
                p.Risk += 10;
                p.Patience += 5;

                break;

            case "😤 Easily Offended":

                p.Anger += 35;
                p.Drama += 20;
                p.Ragebait += 20;

                break;

            case "🤨 Never Trusts A Pawn":

                p.Patience += 15;
                p.Risk -= 10;
                p.Greed += 10;

                break;
        }
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

            case "🧠 Calculation Addict":

                p.Patience += 25;
                p.Confidence += 10;
                p.Chaos -= 10;

                break;

            case "🛡️ Defensive Specialist":

                p.Patience += 25;
                p.Risk -= 20;
                p.Confidence += 5;

                break;

            case "⚔️ Attack Addict":

                p.Risk += 25;
                p.Chaos += 15;
                p.Ragebait += 10;

                break;

            case "🎯 Tactical Hunter":

                p.Confidence += 15;
                p.Risk += 10;
                p.Patience += 10;

                break;

            case "🧊 Ice Cold":

                p.Panic -= 20;
                p.Anger -= 15;
                p.Drama -= 15;
                p.Patience += 20;

                break;

            case "🔥 Revenge Seeker":

                p.Anger += 30;
                p.Ragebait += 20;
                p.Risk += 10;

                break;

            case "📈 Material Investor":

                p.Greed += 30;
                p.Patience += 15;

                break;

            case "🪤 Trap Addict":

                p.Chaos += 20;
                p.Risk += 20;
                p.Patience += 10;

                break;

            case "🏰 Castle Defender":

                p.Patience += 25;
                p.Risk -= 15;
                p.Panic -= 5;

                break;

            case "👀 Suspicious Of Everything":

                p.Patience += 20;
                p.Risk -= 15;
                p.Confidence -= 5;

                break;

            case "🧨 Sacrifice Enjoyer":

                p.Risk += 40;
                p.Chaos += 20;
                p.Greed -= 10;

                break;

            case "🥱 Low-Energy Strategist":

                p.Patience += 25;
                p.Chaos -= 20;
                p.Risk -= 10;

                break;

            case "🧠 Opening Nerd":

                p.Patience += 30;
                p.Confidence += 10;
                p.Chaos -= 15;

                break;

            case "🐌 Slow Planner":

                p.Patience += 35;
                p.Risk -= 10;
                p.Chaos -= 15;

                break;

            case "⚡ Speed Freak":

                p.Chaos += 20;
                p.Risk += 20;
                p.Patience -= 25;

                break;

            case "🤑 Free-Piece Collector":

                p.Greed += 35;
                p.Confidence += 10;

                break;

            case "👑 King Protector":

                p.Patience += 20;
                p.Risk -= 15;
                p.Panic -= 10;

                break;

            case "🥔 Pawn General":

                p.PotatoFanatic = true;
                p.Drama += 10;
                p.Patience += 10;

                break;
        }

        p.Clamp();
    }

    private void MutateStats(
        ComputerProfile p)
    {
        /*
         * Small independent mutations make two personalities
         * with the same trait/hidden trait feel different.
         */
        p.Ego += RandomMutation(12);
        p.Anger += RandomMutation(12);
        p.Panic += RandomMutation(12);
        p.Chaos += RandomMutation(15);
        p.Drama += RandomMutation(15);
        p.Ragebait += RandomMutation(15);

        p.Greed += RandomMutation(12);
        p.Patience += RandomMutation(12);
        p.Helpfulness += RandomMutation(10);
        p.Risk += RandomMutation(15);
        p.Confidence += RandomMutation(12);

        /*
         * Give the profile a chance to have an unusual combination.
         */
        if (rng.Next(100) < 18)
        {
            p.Chaos += rng.Next(10, 25);
            p.Patience -= rng.Next(5, 20);
        }

        if (rng.Next(100) < 18)
        {
            p.Patience += rng.Next(10, 25);
            p.Risk -= rng.Next(5, 20);
        }

        if (rng.Next(100) < 15)
        {
            p.Ragebait += rng.Next(10, 25);
            p.Drama += rng.Next(5, 20);
        }

        p.Clamp();
    }

    private int RandomMutation(
        int amount)
    {
        return rng.Next(
            -amount,
            amount + 1);
    }

    private string BuildSignature(
        ComputerProfile p)
    {
        /*
         * The signature includes both identity and personality
         * characteristics.
         *
         * Therefore the same name with a completely different
         * personality is allowed, while the same complete
         * personality is rejected.
         */
        return string.Join(
            "|",
            p.Name,
            p.Title,
            p.Trait,
            p.HiddenTrait,
            p.Ego,
            p.Anger,
            p.Panic,
            p.Chaos,
            p.Drama,
            p.Ragebait,
            p.Greed,
            p.Patience,
            p.Helpfulness,
            p.Risk,
            p.Confidence);
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
        else if (p.Anger >= 90)
        {
            p.Mood =
                "😡 ABSOLUTELY FURIOUS";
        }
        else if (p.Ragebait >= 95)
        {
            p.Mood =
                "😈 ABSOLUTE RAGEBAITER";
        }
        else if (p.Ego >= 95 &&
                 p.Confidence >= 85)
        {
            p.Mood =
                "😎 UNBEARABLY CONFIDENT";
        }
        else if (p.Drama >= 95)
        {
            p.Mood =
                "😭 ABSOLUTELY DRAMATIC";
        }
        else if (p.Chaos >= 95)
        {
            p.Mood =
                "🤡 TOTAL CHAOS";
        }
        else if (p.Greed >= 90)
        {
            p.Mood =
                "🤑 COUNTING EVERYTHING";
        }
        else if (p.Confidence <= 25)
        {
            p.Mood =
                "🥲 LOSING CONFIDENCE";
        }
        else if (p.Panic >= 70)
        {
            p.Mood =
                "😱 PANICKING";
        }
        else if (p.Anger >= 70)
        {
            p.Mood =
                "😡 GETTING ANNOYED";
        }
        else if (p.Ragebait >= 80)
        {
            p.Mood =
                "😈 LOOKING FOR TROUBLE";
        }
        else if (p.Chaos >= 80)
        {
            p.Mood =
                "🤡 QUESTIONABLE ENERGY";
        }
        else if (p.Patience >= 85)
        {
            p.Mood =
                "🧊 CALM AND CALCULATING";
        }
        else if (p.Confidence >= 85)
        {
            p.Mood =
                "😎 FEELING DANGEROUS";
        }
        else
        {
            /*
             * Even the fallback mood changes rather than always
             * returning the same string.
             */
            p.Mood = Pick(Moods);
        }
    }

    private string Pick(
        string[] values)
    {
        return values[
            rng.Next(values.Length)];
    }
}
