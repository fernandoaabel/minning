namespace Avaliador_Textual.Models.Outros
{
    public class Token
    {
        public string Palavra { get; set; }
        public string Radical { get; set; }
        public int? Frequencia { get; set; }

        public Token ()
        {

        }

        public Token(string palavra)
        {
            Palavra    = palavra;
            Frequencia = 1;
        }

        public Token(string palavra, int? frequencia)
        {
            Palavra    = palavra;
            Frequencia = frequencia;
        }
    }
}