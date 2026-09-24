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
        "You have a strategy. I assume."
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
        "I am emotionally recovering from that potato."
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
        "I am filing a complaint about the horse."
    };

    private static readonly string[] PlayerCaptureBishop =
    {
        "The accountant is gone.",
        "Who is doing the diagonals now?",
        "You eliminated middle management.",
        "My accountant!",
        "The accountant has been terminated.",
        "Fine. We can operate without accounting."
    };

    private static readonly string[] PlayerCaptureRook =
    {
        "THE OFFICE FRIDGE.",
        "Who is going to store the snacks now?",
        "You destroyed the fridge.",
        "That was company property.",
        "The office fridge has been stolen.",
        "I had snacks in there."
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
        "My queen has LEFT THE BUILDING."
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
        "I accept your offering."
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
        "I would like credit for that."
    };

    private static readonly string[] Blunder =
    {
        "I have made a small tactical error.",
        "By small, I mean catastrophic.",
        "Nobody saw that.",
        "We are not going to discuss what just happened.",
        "That was a warm-up.",
        "I was testing you.",
        "Obviously intentional.",
        "The board looked different a second ago.",
        "I meant to do something else.",
        "That was a highly experimental move."
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
        "This is fine."
    };

    private static readonly string[] PlayerCheckmate =
    {
        "🏆 YOU WIN 🏆",
        "Brain: 404 NOT FOUND",
        "I would like to uninstall chess.",
        "Fine. You win this one.",
        "I have suffered a corporate defeat.",
        "My CEO has been fired.",
        "I will remember this betrayal.",
        "Congratulations. I am furious.",
        "Okay. That was actually impressive."
    };

    private static readonly string[] ComputerCheckmate =
    {
        "💀 CHECKMATE",
        "I WON.",
        "Someone please write this down.",
        "I need evidence of this victory.",
        "The CEO remains employed.",
        "You have been professionally outmaneuvered.",
        "I would like this victory framed.",
        "Please don't close the browser. I need to enjoy this.",
        "That went exactly according to the plan I definitely had."
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
        "I have faith in your ability to make another mistake."
    };

    private static readonly string[] PauseLines =
    {
        "Oh. We're taking a break? Fine.",
        "PAUSED. My brain cell is also taking a break.",
        "Excellent. I needed time to judge that last move.",
        "Fine. I shall wait.",
        "Pause accepted. I will be over here plotting.",
        "I was not nervous anyway.",
        "Taking a break from destroying you? Sure."
    };

    private static readonly string[] ResumeLines =
    {
        "Welcome back. I have spent the last 14 seconds judging you.",
        "You're back. Unfortunately.",
        "Excellent. Let us continue this questionable decision-making.",
        "Resume accepted.",
        "I hope you used that break wisely.",
        "Back already? I wasn't finished being smug.",
        "The suffering continues."
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
        "This game has a budget of approximately zero dollars."
    };

    private static readonly string[] LongGame =
    {
        "We've been doing this for a while.",
        "How is this still going?",
        "At this point, we're both committed.",
        "This game has become a lifestyle.",
        "I miss who I was before this game.",
        "We could have been doing literally anything else.",
        "This has gone beyond chess."
    };

    private static readonly string[] ThreeCaptures =
    {
        "Okay, that's three pieces.",
        "You're on a little capturing streak.",
        "Stop taking my stuff.",
        "Do you mind?",
        "You are becoming a material problem."
    };

    private static readonly string[] LosingQueen =
    {
        "YOU TOOK MY QUEEN.",
        "No. No no no.",
        "The queen is gone.",
        "I have lost the queen. This is suboptimal.",
        "This is a terrible day for the company."
    };

    private static readonly string[] PlayerRepeatedAttack =
    {
        "You really like attacking my pieces.",
        "Again?",
        "You are persistent.",
        "I have noticed a pattern.",
        "You're still doing that.",
        "Okay, I get it. You like attacking."
    };

    public string ForPlayerMove(Move m, ComputerProfile p)
    {
        string message;

        if (m.Captured != null)
        {
            message = CaptureReaction(m.Captured.Type, p);
        }
        else if (p.PlayerChecks > 0 && rng.Next(100) < 25)
        {
            message = Pick(CheckLines);
        }
        else if (p.ConsecutivePlayerCaptures >= 3)
        {
            message = Pick(ThreeCaptures);
        }
        else if (p.PlayerMoves >= 30 && rng.Next(100) < 35)
        {
            message = Pick(LongGame);
        }
        else
        {
            message = Pick(Bad);
        }

        return Unique(message, p);
    }

    public string ForComputerMove(Move m, ComputerProfile p)
    {
        string message;

        if (m.Captured != null)
        {
            message = Pick(ComputerCapture);
        }
        else if (p.Ragebaiter && rng.Next(100) < 55)
        {
            message = Pick(RageLines);
        }
        else if (p.StrongMoves > 0 && rng.Next(100) < 35)
        {
            message = Pick(StrongMove);
        }
        else
        {
            message = Pick(Bad);
        }

        return Unique(message, p);
    }

    public string PlayerCaptureSpecific(PieceType type, ComputerProfile p)
    {
        return Unique(CaptureReaction(type, p), p);
    }

    private string CaptureReaction(PieceType type, ComputerProfile p)
    {
        if (type == PieceType.Queen)
        {
            return Pick(LosingQueen);
        }

        if (type == PieceType.Knight && p.HorseObsessed)
        {
            return Pick(PlayerCaptureKnight);
        }

        if (type == PieceType.Pawn && p.PotatoFanatic)
        {
            return Pick(PlayerCapturePawn);
        }

        return type switch
        {
            PieceType.Pawn => Pick(PlayerCapturePawn),
            PieceType.Knight => Pick(PlayerCaptureKnight),
            PieceType.Bishop => Pick(PlayerCaptureBishop),
            PieceType.Rook => Pick(PlayerCaptureRook),
            PieceType.Queen => Pick(PlayerCaptureQueen),
            _ => Pick(Bad)
        };
    }

    public string Check(bool computerInCheck)
    {
        return computerInCheck
            ? Pick(CheckLines)
            : "WHY IS MY KING BEING ATTACKED?";
    }

    public string Rage(ComputerProfile p)
    {
        return Unique(Pick(RageLines), p);
    }

    public string Blunder(ComputerProfile p)
    {
        return Unique(Pick(Blunder), p);
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

    public string LongGameReaction(ComputerProfile p)
    {
        return Unique(Pick(LongGame), p);
    }

    public string RepeatedAttack(ComputerProfile p)
    {
        return Unique(Pick(PlayerRepeatedAttack), p);
    }

    public IEnumerable<string> GameOver(bool playerWon, string result)
    {
        if (result.StartsWith("DRAW"))
        {
            yield return "🤝 DRAW";
            yield return "Nobody wins. Nobody gets to brag.";
            yield break;
        }

        if (playerWon)
        {
            foreach (var line in RandomDistinct(PlayerCheckmate, 3))
            {
                yield return line;
            }

            yield break;
        }

        foreach (var line in RandomDistinct(ComputerCheckmate, 3))
        {
            yield return line;
        }
    }

    private string Unique(string message, ComputerProfile p)
    {
        // Prevent immediate/recent repetition.
        if (p.RecentCommentary.Contains(message))
        {
            for (int i = 0; i < 12; i++)
            {
                var replacement = Pick(Bad);

                if (!p.RecentCommentary.Contains(replacement))
                {
                    message = replacement;
                    break;
                }
            }
        }

        p.RecentCommentary.Enqueue(message);

        while (p.RecentCommentary.Count > 8)
        {
            p.RecentCommentary.Dequeue();
        }

        return message;
    }

    private IEnumerable<string> RandomDistinct(string[] source, int count)
    {
        var available = source.ToList();

        for (int i = 0; i < count && available.Count > 0; i++)
        {
            var index = rng.Next(available.Count);
            var selected = available[index];

            available.RemoveAt(index);

            yield return selected;
        }
    }

    private string Pick(string[] values)
    {
        return values[rng.Next(values.Length)];
    }
}
