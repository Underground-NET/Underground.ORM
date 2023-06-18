namespace Underground.ORM.Core.Entity
{
    public class SampleTableEntity : OrmEntityBase
    {
        public string Name { get; set; }

        public DateTime Birthday { get; set; }

        public int Age { get; set; }

        public SampleTableEntity(string name,
                                 DateTime birthday,
                                 int age)
        {
            Name = name;
            Birthday = birthday;
            Age = age;
        }
    }
}
