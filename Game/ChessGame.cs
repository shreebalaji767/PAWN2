namespace PAWN2.Game;

public sealed class ChessGame
{
    public Piece?[] Board { get; private set; } = new Piece?[64];
    public PieceColor Turn { get; private set; } = PieceColor.White;
    public bool GameOver { get; private set; }
    public string Result { get; private set; } = "";
    public int? EnPassantTarget { get; private set; }
    public int HalfmoveClock { get; private set; }
    public List<Move> History { get; } = new();
    public List<Piece> CapturedWhite { get; } = new();
    public List<Piece> CapturedBlack { get; } = new();
    private readonly Dictionary<string,int> repetition = new();

    public ChessGame() => Reset();

    public void Reset()
    {
        Board = new Piece?[64]; Turn=PieceColor.White; GameOver=false; Result=""; EnPassantTarget=null; HalfmoveClock=0;
        History.Clear(); CapturedWhite.Clear(); CapturedBlack.Clear(); repetition.Clear();
        PieceType[] back={PieceType.Rook,PieceType.Knight,PieceType.Bishop,PieceType.Queen,PieceType.King,PieceType.Bishop,PieceType.Knight,PieceType.Rook};
        for(int c=0;c<8;c++) { Board[c]=new Piece(back[c],PieceColor.Black); Board[8+c]=new Piece(PieceType.Pawn,PieceColor.Black); Board[48+c]=new Piece(PieceType.Pawn,PieceColor.White); Board[56+c]=new Piece(back[c],PieceColor.White); }
        repetition[PositionKey()]=1;
    }

    public Piece? At(int r,int c)=>r>=0&&r<8&&c>=0&&c<8?Board[r*8+c]:null;
    public static int Row(int sq)=>sq/8;
    public static int Col(int sq)=>sq%8;
    public static string SquareName(int sq)=>$"{(char)('a'+Col(sq))}{8-Row(sq)}";
    public static int Square(string s)=>(8-(s[1]-'0'))*8+(s[0]-'a');
    public static PieceColor Opp(PieceColor c)=>c==PieceColor.White?PieceColor.Black:PieceColor.White;

    public List<Move> LegalMoves(PieceColor color)
    {
        var result=new List<Move>();
        for(int i=0;i<64;i++) if(Board[i]?.Color==color)
            foreach(var m in PseudoMoves(i,color))
            {
                var copy=Clone(); copy.ApplyUnchecked(m, false);
                if(!copy.IsInCheck(color)) result.Add(m);
            }
        return result;
    }

    private IEnumerable<Move> PseudoMoves(int from, PieceColor color)
    {
        var p=Board[from]!; int r=Row(from),c=Col(from); int dir=color==PieceColor.White?-1:1;
        if(p.Type==PieceType.Pawn)
        {
            int nr=r+dir;
            if(nr>=0&&nr<8)
            {
                int one=nr*8+c;
                if(Board[one]==null)
                {
                    foreach(var m in PromotionMoves(from,one,color)) yield return m;
                    int start=color==PieceColor.White?6:1;
                    int two=(r+2*dir)*8+c;
                    if(r==start&&!p.HasMoved&&Board[two]==null) yield return new Move{From=from,To=two};
                }
                foreach(int dc in new[]{-1,1})
                {
                    int nc=c+dc;if(nc<0||nc>7)continue;int to=nr*8+nc;
                    if(Board[to]?.Color==Opp(color)&&Board[to]?.Type!=PieceType.King)
                        foreach(var m in PromotionMoves(from,to,color)){m.Captured=Board[to];yield return m;}
                    if(EnPassantTarget==to&&Board[to]==null)
                        yield return new Move{From=from,To=to,IsEnPassant=true,Captured=Board[r*8+nc]};
                }
            }
            yield break;
        }
        if(p.Type==PieceType.Knight)
        {
            foreach(var d in KnightDirs){int nr=r+d.Item1,nc=c+d.Item2;if(In(nr,nc)){int to=nr*8+nc;if(Board[to]?.Color!=color&&Board[to]?.Type!=PieceType.King)yield return new Move{From=from,To=to,Captured=Board[to]};}}
            yield break;
        }
        if(p.Type==PieceType.King)
        {
            for(int dr=-1;dr<=1;dr++)for(int dc=-1;dc<=1;dc++){if(dr==0&&dc==0)continue;int nr=r+dr,nc=c+dc;if(In(nr,nc)){int to=nr*8+nc;if(Board[to]?.Color!=color&&Board[to]?.Type!=PieceType.King)yield return new Move{From=from,To=to,Captured=Board[to]};}}
            if(!p.HasMoved&&!IsInCheck(color))
            {
                int rank=r*8;
                if(Board[rank+7]?.Type==PieceType.Rook&&Board[rank+7]!.Color==color&&!Board[rank+7]!.HasMoved&&Board[rank+5]==null&&Board[rank+6]==null&&!IsSquareAttacked(rank+5,Opp(color))&&!IsSquareAttacked(rank+6,Opp(color)))yield return new Move{From=from,To=rank+6,IsCastle=true};
                if(Board[rank]?.Type==PieceType.Rook&&Board[rank]!.Color==color&&!Board[rank]!.HasMoved&&Board[rank+1]==null&&Board[rank+2]==null&&Board[rank+3]==null&&!IsSquareAttacked(rank+3,Opp(color))&&!IsSquareAttacked(rank+2,Opp(color)))yield return new Move{From=from,To=rank+2,IsCastle=true};
            }
            yield break;
        }
        var dirs=p.Type switch
        {
            PieceType.Bishop=>BishopDirs,
            PieceType.Rook=>RookDirs,
            _=>QueenDirs
        };
        foreach(var d in dirs){int nr=r+d.Item1,nc=c+d.Item2;while(In(nr,nc)){int to=nr*8+nc;if(Board[to]==null)yield return new Move{From=from,To=to};else{if(Board[to]!.Color!=color&&Board[to]!.Type!=PieceType.King)yield return new Move{From=from,To=to,Captured=Board[to]};break;}nr+=d.Item1;nc+=d.Item2;}}
    }

    private static readonly (int,int)[] KnightDirs={(-2,-1),(-2,1),(-1,-2),(-1,2),(1,-2),(1,2),(2,-1),(2,1)};
    private static readonly (int,int)[] BishopDirs={(-1,-1),(-1,1),(1,-1),(1,1)};
    private static readonly (int,int)[] RookDirs={(-1,0),(1,0),(0,-1),(0,1)};
    private static readonly (int,int)[] QueenDirs={(-1,-1),(-1,1),(1,-1),(1,1),(-1,0),(1,0),(0,-1),(0,1)};
    private static bool In(int r,int c)=>r>=0&&r<8&&c>=0&&c<8;

    private IEnumerable<Move> PromotionMoves(int from,int to,PieceColor color)
    {
        if(Row(to)==(color==PieceColor.White?0:7)) foreach(var t in new[]{PieceType.Queen,PieceType.Rook,PieceType.Bishop,PieceType.Knight}) yield return new Move{From=from,To=to,Promotion=t};
        else yield return new Move{From=from,To=to};
    }

    public void Apply(Move move)
    {
        move.Captured=move.IsEnPassant?Board[Row(move.From)*8+Col(move.To)]:Board[move.To];
        bool pawnMove=Board[move.From]?.Type==PieceType.Pawn;
        ApplyUnchecked(move,true);
        if(pawnMove||move.Captured!=null)HalfmoveClock=0;else HalfmoveClock++;
        History.Add(move); Turn=Opp(Turn);
        string key=PositionKey(); repetition[key]=repetition.TryGetValue(key,out var n)?n+1:1;
        UpdateStatus();
    }

    private void ApplyUnchecked(Move move,bool trackCapture)
    {
        var p=Board[move.From]!;Board[move.From]=null;
        if(move.IsEnPassant){int cap=Row(move.From)*8+Col(move.To);Board[cap]=null;}
        Board[move.To]=p;p.HasMoved=true;
        if(move.Promotion.HasValue)Board[move.To]=new Piece(move.Promotion.Value,p.Color,true);
        if(move.IsCastle){int r=Row(move.From);if(move.To>move.From){Board[r*8+5]=Board[r*8+7];Board[r*8+7]=null;Board[r*8+5]!.HasMoved=true;}else{Board[r*8+3]=Board[r*8];Board[r*8]=null;Board[r*8+3]!.HasMoved=true;}}
        EnPassantTarget=null;if(p.Type==PieceType.Pawn&&Math.Abs(Row(move.To)-Row(move.From))==2)EnPassantTarget=(move.From+move.To)/2;
        if(trackCapture&&move.Captured!=null){if(move.Captured.Color==PieceColor.White)CapturedWhite.Add(move.Captured);else CapturedBlack.Add(move.Captured);}
    }

    private void UpdateStatus()
    {
        var moves=LegalMoves(Turn);
        if(moves.Count==0){GameOver=true;Result=IsInCheck(Turn)?(Turn==PieceColor.White?"BLACK CHECKMATE":"WHITE CHECKMATE"):"STALEMATE";return;}
        if(HalfmoveClock>=100){GameOver=true;Result="DRAW — FIFTY-MOVE RULE";return;}
        if(repetition.TryGetValue(PositionKey(),out var count)&&count>=3){GameOver=true;Result="DRAW — THREEFOLD REPETITION";return;}
        if(IsInsufficientMaterial()){GameOver=true;Result="DRAW — INSUFFICIENT MATERIAL";}
    }

    public bool IsInCheck(PieceColor color){int king=Array.FindIndex(Board,p=>p?.Color==color&&p.Type==PieceType.King);return king>=0&&IsSquareAttacked(king,Opp(color));}
    public bool IsSquareAttacked(int sq,PieceColor by)
    {
        int r=Row(sq),c=Col(sq), pawnSourceRow=r-(by==PieceColor.White?-1:1);
        foreach(int dc in new[]{-1,1}){int pc=c-dc;if(In(pawnSourceRow,pc)&&Board[pawnSourceRow*8+pc]?.Color==by&&Board[pawnSourceRow*8+pc]?.Type==PieceType.Pawn)return true;}
        foreach(var d in KnightDirs){int nr=r+d.Item1,nc=c+d.Item2;if(In(nr,nc)&&Board[nr*8+nc]?.Color==by&&Board[nr*8+nc]?.Type==PieceType.Knight)return true;}
        foreach(var d in BishopDirs)if(Ray(r,c,d,by,PieceType.Bishop,PieceType.Queen))return true;
        foreach(var d in RookDirs)if(Ray(r,c,d,by,PieceType.Rook,PieceType.Queen))return true;
        for(int dr=-1;dr<=1;dr++)for(int dc=-1;dc<=1;dc++){if(dr==0&&dc==0)continue;int nr=r+dr,nc=c+dc;if(In(nr,nc)&&Board[nr*8+nc]?.Color==by&&Board[nr*8+nc]?.Type==PieceType.King)return true;}
        return false;
    }
    private bool Ray(int r,int c,(int,int)d,PieceColor by,PieceType a,PieceType b){int nr=r+d.Item1,nc=c+d.Item2;while(In(nr,nc)){var p=Board[nr*8+nc];if(p!=null)return p.Color==by&&(p.Type==a||p.Type==b);nr+=d.Item1;nc+=d.Item2;}return false;}

    public ChessGame Clone()
    {
        var g=new ChessGame();g.Board=Board.Select(p=>p?.Clone()).ToArray();g.Turn=Turn;g.GameOver=GameOver;g.Result=Result;g.EnPassantTarget=EnPassantTarget;g.HalfmoveClock=HalfmoveClock;g.repetition.Clear();foreach(var kv in repetition)g.repetition[kv.Key]=kv.Value;return g;
    }

    private bool IsInsufficientMaterial()
    {
        var pieces=Board.Where(p=>p!=null).Select(p=>p!).ToList();
        if(pieces.Any(p=>p.Type is PieceType.Pawn or PieceType.Rook or PieceType.Queen))return false;
        int bishops=pieces.Count(p=>p.Type==PieceType.Bishop), knights=pieces.Count(p=>p.Type==PieceType.Knight);
        if(bishops+knights<=1)return true;
        if(knights==0&&bishops==2){var squares=Board.Select((p,i)=>(p,i)).Where(x=>x.p?.Type==PieceType.Bishop).Select(x=>x.i).ToArray();return ((Row(squares[0])+Col(squares[0]))%2)==((Row(squares[1])+Col(squares[1]))%2);}
        return false;
    }

    public string PositionKey()
    {
        var sb=new System.Text.StringBuilder();
        for(int i=0;i<64;i++){var p=Board[i];sb.Append(p==null?'.':PieceLetter(p.Type)+(p.Color==PieceColor.White?'w':'b'));}
        sb.Append(Turn==PieceColor.White?'w':'b').Append('|').Append(CastlingRights()).Append('|').Append(EnPassantTarget?.ToString()??"-");
        return sb.ToString();
    }

    private string CastlingRights()
    {
        var sb=new System.Text.StringBuilder();
        if(Board[60]?.Type==PieceType.King&&Board[60]?.Color==PieceColor.White&&!Board[60]!.HasMoved){if(Board[63]?.Type==PieceType.Rook&&Board[63]?.Color==PieceColor.White&&!Board[63]!.HasMoved)sb.Append("K");if(Board[56]?.Type==PieceType.Rook&&Board[56]?.Color==PieceColor.White&&!Board[56]!.HasMoved)sb.Append("Q");}
        if(Board[4]?.Type==PieceType.King&&Board[4]?.Color==PieceColor.Black&&!Board[4]!.HasMoved){if(Board[7]?.Type==PieceType.Rook&&Board[7]?.Color==PieceColor.Black&&!Board[7]!.HasMoved)sb.Append("k");if(Board[0]?.Type==PieceType.Rook&&Board[0]?.Color==PieceColor.Black&&!Board[0]!.HasMoved)sb.Append("q");}
        return sb.Length==0?"-":sb.ToString();
    }

    public static string PieceLetter(PieceType t)=>t switch{PieceType.King=>"K",PieceType.Queen=>"Q",PieceType.Rook=>"R",PieceType.Bishop=>"B",PieceType.Knight=>"N",_=>""};
}
