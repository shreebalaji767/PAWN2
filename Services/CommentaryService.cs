using PAWN2.Game;
using PAWN2.Models;

namespace PAWN2.Services;

public sealed class CommentaryService
{
    private readonly Random rng=new();
    private static readonly string[] Bad={"Did you actually mean to do that?","Interesting decision.","Bold.","Questionable.","Are you sure?"};
    private static readonly string[] Capture={"Nom.","Goodbye.","Mine now."};
    private static readonly string[] Rage={"That was your move? Excellent. I was worried you might be competent.","Please don't resign. I want the screenshots.","You can still win. Probably. Maybe.","I moved that there specifically to annoy you.","Take the bait. I dare you.","You saw the trap. You still walked into it."};
    public string ForPlayerMove(Move m,ComputerProfile p)=>m.Captured!=null?Capture[rng.Next(Capture.Length)]:Bad[rng.Next(Bad.Length)];
    public string ForComputerMove(Move m,ComputerProfile p)=>m.Captured!=null?Capture[rng.Next(Capture.Length)]:p.Ragebait>75?Rage[rng.Next(Rage.Length)]:"Did you see that? I absolutely planned that. Do not inspect the code.";
    public string Check(bool computerInCheck)=>computerInCheck?"🚨 CEO IN DANGER":"WHY IS MY KING BEING ATTACKED?";
    public string Rage(ComputerProfile p)=>Rage[rng.Next(Rage.Length)];
    public string Pause()=>"Oh. We're taking a break? Fine.";
    public string Resume()=>"Welcome back. I have spent the last 14 seconds judging you.";
    public string FourthWall()=>rng.Next(2)==0?"The JavaScript developer didn't give me enough processing power.":"I could calculate this position. Or… we could just make something stupid happen.";
    public IEnumerable<string> GameOver(bool playerWon,string result)
    {
        if(result.StartsWith("DRAW"))yield return "🤝 DRAW";
        else if(playerWon){yield return "🏆 YOU WIN 🏆";yield return "Brain: 404 NOT FOUND";yield return "I would like to uninstall chess.";}
        else {yield return "💀 CHECKMATE";yield return "I WON. Someone please write this down. I need evidence.";}
    }
}
