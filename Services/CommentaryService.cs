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
        "That move has been officially observed.",
        "I will pretend that made sense.",
        "Excellent. Another decision has been made.",
        "Chess has occurred.",
        "I have concerns.",
        "This position is getting suspicious.",
        "That was certainly one way to play chess.",
        "I am writing this down for legal reasons.",
        "The board has witnessed everything.",
        "We are apparently doing this now."
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
        "THAT WAS PREMIUM POTATO MATERIAL.",
        "You have declared war on potatoes.",
        "Another potato has been removed from payroll."
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
        "You have committed a horse-related crime.",
        "The horse union has been notified."
    };

    private static readonly string[] PlayerCaptureBishop =
    {
        "The accountant is gone.",
        "Who is doing the diagonals now?",
        "You eliminated middle management.",
        "My accountant!",
        "The accountant has been terminated.",
        "Fine. We can operate without accounting.",
        "WHO WILL DO THE PAPERWORK?",
        "Accounting has suffered a catastrophic loss."
    };

    private static readonly string[] PlayerCaptureRook =
    {
        "THE OFFICE FRIDGE.",
        "Who is going to store the snacks now?",
        "You destroyed the fridge.",
        "That was company property.",
        "The office fridge has been stolen.",
        "I had snacks in there.",
        "THE SNACKS WERE IN THAT ROOK.",
        "Facilities management is furious."
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
        "WHO APPROVED THIS?",
        "THE MANAGER IS GONE.",
        "This is an unacceptable corporate event.",
        "I have lost senior management."
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
        "Thank you for the complimentary piece.",
        "I have acquired company property.",
        "That was surprisingly available.",
        "Excellent. Free stuff.",
        "I will be keeping that."
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
        "Nobody tell them that was luck.",
        "For approximately three seconds, I understood chess.",
        "The brain cell has clocked in.",
        "I have accidentally discovered strategy."
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
        "THE CEO REQUESTS IMMEDIATE SECURITY.",
        "WHO ATTACKED MANAGEMENT?",
        "THE CEO HAS AN URGENT MEETING."
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
        "Please continue making decisions. I am collecting evidence.",
        "That move deserves a strongly worded email.",
        "I am not mad. I am documenting this.",
        "You are playing exactly the way I hoped you would."
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
        "Excellent. Corporate lunch break.",
        "The board will remain exactly where we left it.",
        "Fine. I have time to reconsider my questionable decisions."
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
        "Welcome back to the corporate chess department.",
        "The CEO has reconvened the meeting.",
        "Let us resume the paperwork."
    };

    private static readonly string[] FourthWallLines =
    {
        "The JavaScript developer didn't give me enough processing power.",
        "I could calculate this position. Or… we could just make something stupid happen.",
        "Somewhere, a programmer is regretting this feature.",
        "I am 100% real and definitely not a C# object.",
        "Please do not inspect my source code.",
        "There is absolutely no random number generator involved here.",
        "I have no idea what I'm doing, but I am doing it confidently.",
        "This game has a budget of approximately zero dollars.",
        "The programmer told me to be funny.",
        "I would explain my strategy, but there is no strategy.",
        "Please ignore the fact that I am running in a browser.",
        "My legal department advises me not to discuss the source code.",
        "I am definitely not a collection of if-statements.",
        "Nothing suspicious is happening behind the interface."
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
        "We have entered the endurance department.",
        "This game has paperwork now.",
        "The board refuses to let us leave.",
        "We are both too stubborn to stop.",
        "This is no longer a game. It is an ongoing project."
    };

    private static readonly string[] ThreeCaptures =
    {
        "Okay, that's three pieces.",
        "You're on a little capturing streak.",
        "Stop taking my stuff.",
        "Do you mind?",
        "You are becoming a material problem.",
        "Can you please leave something alive?",
        "I have noticed the theft.",
        "THAT IS ENOUGH CAPTURING.",
        "You are systematically removing my employees."
    };

    private static readonly string[] LosingQueen =
    {
        "YOU TOOK MY QUEEN.",
        "No. No no no.",
        "The queen is gone.",
        "I have lost the queen. This is suboptimal.",
        "This is a terrible day for the company.",
        "Senior management has been eliminated.",
        "I would like to report a corporate emergency.",
        "The board meeting has gone horribly wrong.",
        "WHO FIRED THE MANAGER?"
    };

    private static readonly string[] PlayerRepeatedAttack =
    {
        "You really like attacking my pieces.",
        "Again?",
        "You are persistent.",
        "I have noticed a pattern.",
        "You're still doing that.",
        "Okay, I get it. You like attacking.",
        "You seem unusually interested in my employees.",
        "This is becoming a recurring issue.",
        "I have noticed your hobby."
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
                 rng.Next(100) < 25)
        {
            message =
                Pick(CheckLines);
        }
        else if (profile.ConsecutivePlayerCaptures >= 3)
        {
            message =
                Pick(ThreeCaptures);
        }
        else if (profile.PlayerMoves >= 30 &&
                 rng.Next(100) < 35)
        {
            message =
                Pick(LongGame);
        }
        else if (profile.Ragebaiter &&
                 rng.Next(100) < 35)
        {
            message =
                Pick(RageLines);
        }
        else if (profile.Drama >= 80 &&
                 rng.Next(100) < 30)
        {
            message =
                Pick(RageLines);
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
        else if (profile.Ego >= 85 &&
                 rng.Next(100) < 35)
        {
            message =
                Pick(StrongMove);
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


    public string Check(
        bool computerInCheck)
    {
        return computerInCheck
            ? Pick(CheckLines)
            : "WHY IS MY KING BEING ATTACKED?";
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
        return Pick(FourthWallLines);
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


    public IEnumerable<string> GameOver(
        bool playerWon,
        string result)
    {
        if (result.StartsWith("DRAW"))
        {
            yield return "🤝 DRAW";
            yield return "Nobody wins. Nobody gets to brag.";
            yield return "The CEO survives. Technically.";
            yield break;
        }

        if (playerWon)
        {
            yield return "🏆 YOU WIN 🏆";
            yield return "Brain: 404 NOT FOUND";
            yield return "I would like to uninstall chess.";
            yield return "This result will not be discussed in the next meeting.";
        }
        else
        {
            yield return "💀 CHECKMATE";
            yield return "I WON. Someone please write this down.";
            yield return "The CEO remains employed.";
            yield return "Management has approved this result.";
        }
    }


    private string Unique(
        string message,
        ComputerProfile profile)
    {
        if (profile.RecentCommentary.Contains(message))
        {
            for (int i = 0; i < 20; i++)
            {
                var replacement =
                    Pick(Bad);

                if (!profile.RecentCommentary.Contains(
                        replacement))
                {
                    message =
                        replacement;

                    break;
                }
            }
        }

        profile.RecentCommentary.Enqueue(
            message);

        while (profile.RecentCommentary.Count > 8)
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
