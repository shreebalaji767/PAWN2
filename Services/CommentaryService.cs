using PAWN2.Game;
using PAWN2.Models;

namespace PAWN2.Services;

public sealed class CommentaryService
{
    private readonly Random rng = new();

    private static readonly string[] Bad =
    {
        "Did you actually mean to do that?",
        "Interesting decision.",
        "Bold.",
        "Questionable.",
        "Are you sure?",
        "That was certainly a move.",
        "I have questions.",
        "Okay. I will allow it.",
        "That was... creative.",
        "I don't know whether to respect that.",
        "You clicked the square. Congratulations.",
        "Interesting. Very interesting.",
        "I have seen worse. Probably.",
        "I am choosing to believe that was intentional.",
        "You have a strategy. I assume.",
        "That move has consequences.",
        "I am going to pretend I understand the plan.",
        "Confidence is doing a lot of work here.",
        "That was certainly one way to play chess.",
        "Please continue. I am collecting evidence."
    };

    private static readonly string[] PlayerCapturePawn =
    {
        "You killed my potato.",
        "MY POTATO.",
        "That potato had a future.",
        "You just ate one of my potatoes.",
        "Fine. Take the potato.",
        "A potato has fallen.",
        "He was just doing his job.",
        "That potato had a family. Probably.",
        "I am emotionally recovering from that potato.",
        "WHY WOULD YOU TAKE THE POTATO?",
        "Another potato has been deleted.",
        "The agricultural sector is collapsing."
    };

    private static readonly string[] PlayerCaptureKnight =
    {
        "NOT THE HORSE.",
        "YOU TOOK MY HORSE.",
        "Why would you hurt the horse?",
        "The horse did nothing to you.",
        "I trusted that horse.",
        "That was my favorite horse.",
        "This is now personal.",
        "I am filing a complaint about the horse.",
        "THE HORSE LAWYER WILL HEAR ABOUT THIS.",
        "That horse had excellent references.",
        "I specifically asked you not to take the horse.",
        "The stable is empty. I am furious."
    };

    private static readonly string[] PlayerCaptureBishop =
    {
        "The accountant is gone.",
        "Who is doing the diagonals now?",
        "You eliminated middle management.",
        "My accountant!",
        "The accountant has been terminated.",
        "Fine. We can operate without accounting.",
        "The finance department has collapsed.",
        "That was a very expensive accountant.",
        "I need to speak with HR.",
        "Who approved this?"
    };

    private static readonly string[] PlayerCaptureRook =
    {
        "THE OFFICE FRIDGE.",
        "Who is going to store the snacks now?",
        "You destroyed the fridge.",
        "That was company property.",
        "The office fridge has been stolen.",
        "I had snacks in there.",
        "WHO TOOK THE FRIDGE?",
        "The entire office is now snackless.",
        "That fridge had tenure.",
        "This is unacceptable workplace behavior."
    };

    private static readonly string[] PlayerCaptureQueen =
    {
        "YOU TOOK MY QUEEN.",
        "Okay. New plan. PANIC.",
        "I would like to speak to management.",
        "SHE WAS IMPORTANT.",
        "That was the overpaid manager.",
        "You cannot just take my queen.",
        "This meeting could have been an email.",
        "I am experiencing a strategic inconvenience.",
        "My queen has LEFT THE BUILDING.",
        "THE MANAGER IS GONE.",
        "WHO IS RUNNING THE COMPANY NOW?",
        "This is an HR emergency.",
        "My entire business model has collapsed."
    };

    private static readonly string[] ComputerCapture =
    {
        "Nom.",
        "Goodbye.",
        "Mine now.",
        "Thank you.",
        "I'll take that.",
        "Oh, that's convenient.",
        "Free material.",
        "You left that there.",
        "Interesting donation.",
        "I accept your offering.",
        "Much appreciated.",
        "That belonged to you. Past tense.",
        "Thank you for the delivery.",
        "Excellent. I needed that."
    };

    private static readonly string[] StrongMove =
    {
        "Did you see that? I absolutely planned that.",
        "Do not inspect the code.",
        "That was calculated.",
        "I meant to do that.",
        "Okay, that was actually good.",
        "My brain cell has returned.",
        "I have temporarily become competent.",
        "Please document this moment.",
        "That was disturbingly intentional.",
        "I would like credit for that.",
        "WRITE THAT DOWN.",
        "For approximately three seconds, I understood chess.",
        "That was suspiciously competent.",
        "My shareholders are pleased."
    };

    private static readonly string[] CheckLines =
    {
        "🚨 CEO IN DANGER",
        "WHY IS MY KING BEING ATTACKED?",
        "THE CEO HAS RECEIVED A THREAT.",
        "SECURITY. SECURITY.",
        "WE HAVE A CEO PROBLEM.",
        "Everyone remain calm.",
        "I am completely calm.",
        "This is fine.",
        "WHO AUTHORIZED THE ATTACK?",
        "THE CEO NEEDS SECURITY.",
        "WE HAVE AN EXECUTIVE EMERGENCY."
    };

    private static readonly string[] RageLines =
    {
        "That was your move? Excellent. I was worried you might be competent.",
        "Please don't resign. I want the screenshots.",
        "You can still win. Probably. Maybe.",
        "I moved that there specifically to annoy you.",
        "Take the bait. I dare you.",
        "You saw the trap. You still walked into it.",
        "That was almost intelligent.",
        "Please continue. This is entertaining.",
        "I believe in you. Unfortunately.",
        "Your confidence is inspiring.",
        "I have faith in your ability to make another mistake.",
        "You are doing exactly what I hoped you would do.",
        "I am not saying that was a blunder. I am saying I enjoyed it.",
        "Thank you for cooperating with my plan."
    };

    private static readonly string[] PauseLines =
    {
        "Oh. We're taking a break? Fine.",
        "PAUSED. My brain cell is also taking a break.",
        "Excellent. I needed time to judge that last move.",
        "Fine. I shall wait.",
        "Pause accepted. I will be over here plotting.",
        "I was not nervous anyway.",
        "Taking a break from destroying you? Sure.",
        "Time out. The board needs therapy.",
        "Excellent. A strategic intermission.",
        "I shall use this time to blame the previous move."
    };

    private static readonly string[] ResumeLines =
    {
        "Welcome back. I have spent the last 14 seconds judging you.",
        "You're back. Unfortunately.",
        "Excellent. Let us continue this questionable decision-making.",
        "Resume accepted.",
        "I hope you used that break wisely.",
        "Back already? I wasn't finished being smug.",
        "The suffering continues.",
        "The board has missed you. I have not.",
        "Welcome back to the consequences.",
        "Let us continue pretending this is under control."
    };

    private static readonly string[] FourthWall =
    {
        "The JavaScript developer didn't give me enough processing power.",
        "I could calculate this position. Or… we could just make something stupid happen.",
        "Somewhere, a programmer is regretting this feature.",
        "I am 100% real and definitely not a C# object.",
        "Please do not inspect my source code.",
        "There is absolutely no random number generator involved here.",
        "I have no idea what I'm doing, but I am doing it confidently.",
        "This game has a budget of approximately zero dollars.",
        "Please ignore the programmer behind the curtain.",
        "I am legally required to pretend this is sophisticated.",
        "The code says this is a chess game. I have my doubts.",
        "My source code would like to remain confidential."
    };

    private static readonly string[] LongGame =
    {
        "We've been doing this for a while.",
        "How is this still going?",
        "At this point, we're both committed.",
        "This game has become a lifestyle.",
        "I miss who I was before this game.",
        "We could have been doing literally anything else.",
        "This has gone beyond chess.",
        "The board has become our permanent residence.",
        "We are now legally roommates.",
        "This game refuses to end.",
        "Someone check the calendar.",
        "I thought this would be over by now."
    };

    private static readonly string[] ThreeCaptures =
    {
        "Okay, that's three pieces.",
        "You're on a little capturing streak.",
        "Stop taking my stuff.",
        "Do you mind?",
        "You are becoming a material problem.",
        "Can you please stop eating my army?",
        "This is getting expensive.",
        "I have noticed the pattern.",
        "You are aggressively collecting my employees."
    };

    private static readonly string[] LosingQueen =
    {
        "YOU TOOK MY QUEEN.",
        "No. No no no.",
        "The queen is gone.",
        "I have lost the queen. This is suboptimal.",
        "This is a terrible day for the company.",
        "THE MANAGER HAS BEEN FIRED.",
        "I no longer have upper management.",
        "The queen was carrying this entire organization.",
        "I am not emotionally prepared for this.",
        "This is now a hostile workplace."
    };

    private static readonly string[] PlayerRepeatedAttack =
    {
        "You really like attacking my pieces.",
        "Again?",
        "You are persistent.",
        "I have noticed a pattern.",
        "You're still doing that.",
        "Okay, I get it. You like attacking.",
        "We have established that you know how to attack.",
        "You are becoming repetitive.",
        "Yes, I noticed the threat."
    };

    private static readonly string[] MissedOpportunityLines =
    {
        "I could have done something clever there.",
        "That was almost brilliant. Almost.",
        "I saw something. Then I forgot what it was.",
        "My brain cell briefly disconnected.",
        "That was a missed opportunity.",
        "I am choosing to call that patience.",
        "Strategic restraint. Definitely.",
        "I absolutely meant to ignore that."
    };

    private static readonly string[] BlunderLines =
    {
        "That was not my finest moment.",
        "We are going to pretend that never happened.",
        "I have made a small administrative error.",
        "Please do not replay that move.",
        "My brain has submitted a formal apology.",
        "I would like to blame the keyboard.",
        "That was strategically questionable.",
        "I have temporarily forgotten chess."
    };

    public string ForPlayerMove(
        Move move,
        ComputerProfile profile)
    {
        string message;

        if (move.Captured is not null)
        {
            message =
                CaptureReaction(
                    move.Captured.Type,
                    profile);
        }
        else if (profile.PlayerChecks > 0 &&
                 rng.Next(100) < 22)
        {
            message =
                Pick(CheckLines);
        }
        else if (profile.ConsecutivePlayerCaptures >= 3)
        {
            message =
                Pick(ThreeCaptures);
        }
        else if (profile.Ragebaiter &&
                 rng.Next(100) < 30)
        {
            message =
                Pick(RageLines);
        }
        else if (profile.PlayerMoves >= 30 &&
                 rng.Next(100) < 35)
        {
            message =
                Pick(LongGame);
        }
        else
        {
            message =
                Pick(Bad);
        }

        return Unique(
            message,
            profile);
    }

    public string ForComputerMove(
        Move move,
        ComputerProfile profile)
    {
        string message;

        if (move.Captured is not null)
        {
            message =
                Pick(ComputerCapture);
        }
        else if (profile.Ragebaiter &&
                 rng.Next(100) < 55)
        {
            message =
                Pick(RageLines);
        }
        else if (profile.StrongMoves > 0 &&
                 rng.Next(100) < 35)
        {
            message =
                Pick(StrongMove);
        }
        else if (profile.Drama >= 80 &&
                 rng.Next(100) < 30)
        {
            message =
                Pick(LongGame);
        }
        else
        {
            message =
                Pick(Bad);
        }

        return Unique(
            message,
            profile);
    }

    public string PlayerCaptureSpecific(
        PieceType type,
        ComputerProfile profile)
    {
        return Unique(
            CaptureReaction(
                type,
                profile),
            profile);
    }

    public string Check(
        bool computerInCheck)
    {
        return Pick(CheckLines);
    }

    public string Rage(
        ComputerProfile profile)
    {
        return Unique(
            Pick(RageLines),
            profile);
    }

    public string Pause()
    {
        return Pick(PauseLines);
    }

    public string Resume()
    {
        return Pick(ResumeLines);
    }

    public string FourthWall()
    {
        return Pick(FourthWall);
    }

    public string LongGameReaction(
        ComputerProfile profile)
    {
        return Unique(
            Pick(LongGame),
            profile);
    }

    public string RepeatedAttack(
        ComputerProfile profile)
    {
        return Unique(
            Pick(PlayerRepeatedAttack),
            profile);
    }

    public string MissedOpportunity(
        ComputerProfile profile)
    {
        return Unique(
            Pick(MissedOpportunityLines),
            profile);
    }

    public string Blunder(
        ComputerProfile profile)
    {
        return Unique(
            Pick(BlunderLines),
            profile);
    }

    public IEnumerable<string> GameOver(
        bool playerWon,
        string result)
    {
        if (result.StartsWith("DRAW"))
        {
            yield return "🤝 DRAW";
            yield return "Nobody wins. Nobody gets to brag.";
            yield return "The board has chosen violence against everyone.";
            yield break;
        }

        if (playerWon)
        {
            yield return "🏆 YOU WIN 🏆";
            yield return "Brain: 404 NOT FOUND";
            yield return "I would like to uninstall chess.";
            yield return "This result will be appealed.";
        }
        else
        {
            yield return "💀 CHECKMATE";
            yield return "I WON. Someone please write this down.";
            yield return "The CEO remains employed.";
            yield return "My shareholders will hear about this victory.";
        }
    }

    private string CaptureReaction(
        PieceType type,
        ComputerProfile profile)
    {
        if (type == PieceType.Queen)
        {
            return Pick(LosingQueen);
        }

        if (type == PieceType.Knight &&
            profile.HorseObsessed)
        {
            return Pick(PlayerCaptureKnight);
        }

        if (type == PieceType.Pawn &&
            profile.PotatoFanatic)
        {
            return Pick(PlayerCapturePawn);
        }

        return type switch
        {
            PieceType.Pawn =>
                Pick(PlayerCapturePawn),

            PieceType.Knight =>
                Pick(PlayerCaptureKnight),

            PieceType.Bishop =>
                Pick(PlayerCaptureBishop),

            PieceType.Rook =>
                Pick(PlayerCaptureRook),

            PieceType.Queen =>
                Pick(PlayerCaptureQueen),

            _ =>
                Pick(Bad)
        };
    }

    private string Unique(
        string message,
        ComputerProfile profile)
    {
        if (profile.RecentCommentary.Contains(
                message))
        {
            for (int i = 0; i < 20; i++)
            {
                var replacement =
                    Pick(Bad);

                if (!profile.RecentCommentary.Contains(
                        replacement))
                {
                    message = replacement;
                    break;
                }
            }
        }

        profile.RecentCommentary.Enqueue(
            message);

        while (profile.RecentCommentary.Count > 10)
        {
            profile.RecentCommentary.Dequeue();
        }

        return message;
    }

    private string Pick(
        string[] values)
    {
        return values[
            rng.Next(values.Length)];
    }
}
