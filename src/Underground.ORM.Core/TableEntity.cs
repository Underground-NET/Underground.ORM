using Urderground.ORM.Core.Entity;

namespace Urderground.ORM.Core
{
    public class TableEntity : OrmBaseEntity
    {
        public string Nome { get; set; }

        public DateTime DataNascimento { get; set; }

        public int Idade { get; set; }

        public TableEntity(string nome, DateTime dataNascimento, int idade)
        {
            Nome = nome;
            DataNascimento = dataNascimento;
            Idade = idade;
        }
    }
}
