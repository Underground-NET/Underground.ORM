namespace Underground.ORM.Core.Syntax.Token.Block
{
    public class EndBlockToken : SyntaxTokenBase
    {
        public bool IsBlock { get => Is.Block; set => Is.Block = value; }

        public bool IsEndBlock { get => Is.EndBlock; set => Is.EndBlock = value; }

        public EndBlockToken() : this("END") 
        { 
        }

        public EndBlockToken(string token) :
            base(token)
        {
            IsBlock = true;
            IsEndBlock = true;
        }
    }
}
