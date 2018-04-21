using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;

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
