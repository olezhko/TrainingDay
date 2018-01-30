using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace TrainingDay.Model
{
    [Table("WeightNote")]
    public class WeightNote
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public double Weight { get; set; }
        public DateTime Date { get; set; }
    }
}
