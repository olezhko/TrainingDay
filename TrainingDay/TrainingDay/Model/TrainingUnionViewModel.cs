using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using TrainingDay.Services;

namespace TrainingDay.Model
{
    public class TrainingUnionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> TrainingIDs { get; set; }

        public TrainingUnion Model =>
            new TrainingUnion()
            {
                Id = Id,
                Name = Name,
                TrainingIDsString = JsonConvert.SerializeObject(TrainingIDs)
            };

        public TrainingUnionViewModel()
        {
            TrainingIDs = new List<int>();
        }

        public TrainingUnionViewModel(TrainingUnion union)
        {
            Id = union.Id;
            Name = union.Name;
            if (!string.IsNullOrEmpty(union.TrainingIDsString))
                TrainingIDs = JsonConvert.DeserializeObject<List<int>>(union.TrainingIDsString);
            else
                TrainingIDs = new List<int>();
        }
    }
}
