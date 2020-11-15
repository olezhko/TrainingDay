using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace TrainingDay.Services
{
    [Serializable]
    public class TrainingSerialize
    {
        public string Title;
        public ObservableCollection<TrainingExerciseSerialize> Items;
        public int Id { get; set; }

        public TrainingSerialize()
        {
            Items = new ObservableCollection<TrainingExerciseSerialize>();
        }


        public void SaveToFile(string filename)
        {
            var stream = new FileStream(filename, FileMode.Create);
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(stream, this);
            stream.Close();
        }

        public static TrainingSerialize LoadFromFile(byte[] data)
        {
            try
            {
                MemoryStream dataStream = new MemoryStream(data);
                BinaryFormatter b = new BinaryFormatter();
                var training = b.Deserialize(dataStream) as TrainingSerialize;
                dataStream.Close();
                return training;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }

    [Serializable]
    public class TrainingExerciseSerialize
    {
        public int ExerciseId { get; set; }
        public int TrainingId { get; set; }
        public string ExerciseItemName;
        public string ShortDescription;
        public string ExerciseImageUrl;
        public int OrderNumber { get; set; }
        public string Muscles { get; set; }
        public int SuperSetId { get; set; } = -1;
        public string WeightAndRepsString { get; set; }
        public bool IsNotFinished { get; set; }
        public int SuperSetNum { get; set; }
        public int TagsValue { get; set; }
        public int TrainingExerciseId { get; set; }

        public int CodeNum { get; set; }
    }
}
