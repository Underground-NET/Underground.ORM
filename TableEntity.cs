namespace MySqlOrm
{
    public class TableEntity : MySqlOrmEntity
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
