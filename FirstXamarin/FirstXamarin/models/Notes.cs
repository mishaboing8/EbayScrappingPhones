using System;

using SQLite;
namespace FirstXamarin.models
{
    public class Notes
    {
        [PrimaryKey, AutoIncrement]

        public int ID { get; set; }

        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
}

