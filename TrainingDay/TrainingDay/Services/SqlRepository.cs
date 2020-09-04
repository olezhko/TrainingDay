using Microsoft.AppCenter.Crashes;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using TrainingDay.Model;
using TrainingDay.ViewModels;
using TrainingDay.Views.Controls;
using Xamarin.Forms;

namespace TrainingDay.Services
{
    public class TrainingUnion
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        public string Name { get; set; }
        public string TrainingIDsString { get; set; }
        public bool IsExpanded { get; set; }
    }

    [Table("Alarm")]
    public class Alarm
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset TimeOffset { get; set; }
        public bool IsActive { get; set; }// need
        public int Days { get; set; }// need
        public int TrainingId { get; set; }// need
    }

    [Table("WeightNote")]
    public class WeightNote
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public double Weight { get; set; }
        public DateTime Date { get; set; }
    }

    [Table("TrainingExerciseComm")]
    public class TrainingExerciseComm
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        public int TrainingId { get; set; }
        public int ExerciseId { get; set; }
        public int OrderNumber { get; set; }
        public int SuperSetId { get; set; }
        public string WeightAndRepsString { get; set; }
    }

    [Table("Exercises")]
    public class Exercise
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        public string ExerciseItemName { get; set; }
        public string Description { get; set; }
        public string ExerciseImageUrl { get; set; }
        public string Version { get; set; }
        public string MusclesString { get; set; } // text, by "," enum collection
        public int TagsValue { get; set; }
        public int CodeNum { get; set; }
    }

    [Table("Trainings")]
    public class Training
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public string Title { get; set; }
    }

    [Table("LastTrainings")]
    public class LastTraining
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Time { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public int TrainingId { get; set; }
    }

    public class LastTrainingExercise
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public int LastTrainingId { get; set; }
        public string ExerciseName { get; set; }
        public string Description { get; set; }
        public string ExerciseImageUrl { get; set; }

        public int OrderNumber { get; set; }
        public int SuperSetId { get; set; }

        public string MusclesString { get; set; }
        public string WeightAndRepsString { get; set; }
        public int TagsValue { get; set; }
    }

    [Serializable]
    public class SuperSet
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public int Count { get; set; }
        public int TrainingId { get; set; }
    }

    public interface ISQLite
    {
        string GetDatabasePath(string filename);
    }

    public class Repository
    {
        SQLiteConnection database;

        public Repository(string filename)
        {
            try
            {
                string databasePath = DependencyService.Get<ISQLite>().GetDatabasePath(filename);
                database = new SQLiteConnection(databasePath);
            }
            catch (Exception e)
            {
                return;
            }

            //DropTable();
            InitBasic();
        }

        //функция подгружающая в программу стандартные тренировки
        private void InitBasic()
        {
            try
            {
                database.CreateTable<Exercise>();
                database.CreateTable<TrainingExerciseComm>();
                database.CreateTable<Training>();
                database.CreateTable<LastTraining>();
                database.CreateTable<WeightNote>();
                database.CreateTable<LastTrainingExercise>();
                database.CreateTable<Alarm>();
                database.CreateTable<SuperSet>();
                database.CreateTable<UpdateItem>();
                database.CreateTable<TrainingUnion>();
                try
                {
                    database.CreateTable<ImageData>();
                }
                catch (Exception e)
                {
                    database.DropTable<ImageData>();
                    database.CreateTable<ImageData>();
                }

                var exer = InitExercises();

                string MaxVersion = "1.0.0.0";

                if (Settings.IsFirstTime)
                {
                    Settings.IsFirstTime = false;
                    var versionSync = new Version(Settings.LastSyncVersion);
                    foreach (var exercise in exer)
                    {
                        var versionEx = new Version(exercise.Version);
                        if (versionEx.CompareTo(versionSync) > 0)
                        {
                            Settings.LastSyncVersion = exercise.Version;
                        }
                        SaveExerciseItem(exercise);
                    }
                }
                else
                {
                    foreach (var exercise in exer)
                    {
                        var versionSync = new Version(MaxVersion);
                        var version1 = new Version(exercise.Version);
                        var version2 = new Version(Settings.LastSyncVersion);

                        if (version1.CompareTo(version2) > 0) //"version1 is greater"
                            SaveExerciseItem(exercise);

                        if (version1.CompareTo(versionSync) > 0)
                        {
                            MaxVersion = exercise.Version;
                        }
                    }
                    Settings.LastSyncVersion = MaxVersion;
                }

                CorrectExercise(exer, GetExerciseItems());
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void CorrectExercise(ObservableCollection<Exercise> srcExercises, IEnumerable<Exercise> baseExercises)
        {
            foreach (var exercise in srcExercises)
            {
                try
                {
                    var fromBase = baseExercises.First(item => item.ExerciseImageUrl == exercise.ExerciseImageUrl);
                    
                    fromBase.Description = exercise.Description;
                    fromBase.CodeNum = exercise.CodeNum;
                    SaveExerciseItem(fromBase);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private ObservableCollection<Exercise> InitExercises()
        {
            try
            {
                var ci = DependencyService.Get<ILocalize>().GetCurrentLanguage();
                string filename = $"TrainingDay.Resources.exercises_{ci}.xml";

                var assembly = typeof(Repository).GetTypeInfo().Assembly;
                Stream stream = assembly.GetManifestResourceStream(filename);

                if (stream == null)
                {
                    stream = assembly.GetManifestResourceStream(@"TrainingDay.Resources.exercises_en.xml");
                }

                if (stream == null)
                {
                    return new ObservableCollection<Exercise>();
                }
                XDocument doc = XDocument.Load(stream);
                var exer = doc.Root.Elements("Exercise").Select(n => new Exercise()
                {
                    ExerciseItemName = n.Element("Name").Value,
                    Description = n.Element("Description").Value,
                    MusclesString = n.Element("Muscles").Value,
                    ExerciseImageUrl = n.Element("ExerciseImageUrl").Value,
                    Version = n.Element("Version").Value,
                    CodeNum = Convert.ToInt32(n.Element("CodeNum").Value),
                    TagsValue = ExerciseTagExtension.ConvertFromStringToInt(n.Element("Tag").Value)
                });


                return new ObservableCollection<Exercise>(exer);
            }
            catch (Exception e)
            {
                DependencyService.Get<IMessage>().ShortAlert("Database Exception Init");
                return new ObservableCollection<Exercise>();
            }
        }

        public int GetLastInsertId()
        {
            return (int)SQLite3.LastInsertRowid(database.Handle);
        }

        #region Weight Save And Load

        public int SaveWeightNotesItem(WeightNote item)
        {
            if (item.Id != 0)
            {
                database.Update(item);
                return item.Id;
            }
            return database.Insert(item);
        }

        public IEnumerable<WeightNote> GetWeightNotesItems()
        {
            return (from i in database.Table<WeightNote>() select i).ToList();
        }

        public void DeleteWeightNote(int senderId)
        {
            database.Delete<WeightNote>(senderId);
        }
        #endregion

        #region LastTrainings

        public IEnumerable<LastTraining> GetLastTrainingItems()
        {
            return (from i in database.Table<LastTraining>() select i).ToList();
        }

        public int SaveLastTrainingItem(LastTraining item)
        {
            if (item.Id != 0)
            {
                database.Update(item);
                return item.Id;
            }
            return database.Insert(item);
        }

        public int DeleteLastTraining(int lastId)
        {
            return database.Delete<LastTraining>(lastId);
        }

        public int SaveLastTrainingExerciseItem(LastTrainingExercise item)
        {
            if (item.Id != 0)
            {
                database.Update(item);
                return item.Id;
            }
            return database.Insert(item);
        }
        public IEnumerable<LastTrainingExercise> GetLastTrainingExerciseItems()
        {
            return (from i in database.Table<LastTrainingExercise>() select i).ToList();
        }

        public int DeleteLastTrainingExercise(int lastId)
        {
            return database.Delete<LastTrainingExercise>(lastId);
        }
        #endregion

        #region Exercise Methods
        public IEnumerable<Exercise> GetExerciseItems()
        {
            return (from i in database.Table<Exercise>() select i).ToList();
        }

        public Exercise GetExerciseItem(int id)
        {
            return database.Get<Exercise>(id);
        }

        public int DeleteExerciseItem(int id)
        {
            return database.Delete<Exercise>(id);
        }

        public int SaveExerciseItem(Exercise item)
        {
            if (item.Id != 0)
            {
                database.Update(item);
                return item.Id;
            }
            database.Insert(item);
            return GetLastInsertId();
        }
        #endregion

        #region TrainingExerciseComm Methods
        public IEnumerable<TrainingExerciseComm> GetTrainingExerciseItems()
        {
            return (from i in database.Table<TrainingExerciseComm>() select i).ToList();
        }

        public TrainingExerciseComm GetTrainingExerciseItem(int id)
        {
            return database.Get<TrainingExerciseComm>(id);
        }

        public int DeleteTrainingExerciseItem(int id)
        {
            return database.Delete<TrainingExerciseComm>(id);
        }

        public int SaveTrainingExerciseItem(TrainingExerciseComm item)
        {
            if (item.Id != 0)
            {
                database.Update(item);
                return item.Id;
            }
            database.Insert(item);
            return GetLastInsertId();
        }

        public List<TrainingExerciseViewModel> GetTrainingExerciseItemByTrainingId(int trainingId)
        {
            List<TrainingExerciseViewModel> items = new List<TrainingExerciseViewModel>();
            var allItems = GetTrainingExerciseItems();
            
            foreach (var trainingExerciseComm in allItems)
            {
                if (trainingExerciseComm.TrainingId == trainingId)
                {
                    items.Add(new TrainingExerciseViewModel(GetExerciseItem(trainingExerciseComm.ExerciseId),trainingExerciseComm));
                }
            }


            items = new List<TrainingExerciseViewModel>(items.OrderBy(a => a.OrderNumber));
            return items;
        }


        public void DeleteTrainingExerciseItemByTrainingId(int trainingId)
        {
            var allItems = GetTrainingExerciseItems();
            foreach (var trainingExerciseComm in allItems)
            {
                if (trainingExerciseComm.TrainingId == trainingId)
                {
                    DeleteTrainingExerciseItem(trainingExerciseComm.Id);
                }
            }
        }
        #endregion

        #region Training Methods
        public IEnumerable<Training> GetTrainingItems()
        {
            return (from i in database.Table<Training>() select i).ToList();
        }

        public Training GetTrainingItem(int id)
        {
            try
            {
                return database.Get<Training>(id);
            }
            catch
            {
                return new Training();
            }
        }

        public int DeleteTrainingItem(int id)
        {
            return database.Delete<Training>(id);
        }

        public int SaveTrainingItem(Training item)
        {
            if (item.Id != 0)
            {
                database.Update(item);
                return item.Id;
            }
            database.Insert(item);
            return GetLastInsertId();
        }
        #endregion

        #region Alarm

        public IEnumerable<Alarm> GetAlarmItems()
        {
            return (from i in database.Table<Alarm>() select i).ToList();
        }

        public Alarm GetAlarmItem(int id)
        {
            return database.Get<Alarm>(id);
        }

        public int DeleteAlarmItem(int id)
        {
            return database.Delete<Alarm>(id);
        }

        public int SaveAlarmItem(Alarm item)
        {
            if (item.Id != 0)
            {
                database.Update(item);
                return item.Id;
            }
            database.Insert(item);
            return GetLastInsertId();
        }

        #endregion

        #region SuperSet

        //public IEnumerable<SuperSetExercise> GetSuperSetItems()
        //{
        //    return (from i in database.Table<SuperSetExercise>() select i).ToList();
        //}

        //public SuperSetExercise GetSuperSetItem(int id)
        //{
        //    return database.Get<SuperSetExercise>(id);
        //}

        //public int SaveSuperSetExerciseItem(SuperSetExercise item)
        //{
        //    if (item.Id != 0)
        //    {
        //        database.Update(item);
        //        return item.Id;
        //    }
        //    database.Insert(item);
        //    return GetLastInsertId();
        //}


        public IEnumerable<SuperSet> GetSuperSetItems()
        {
            return (from i in database.Table<SuperSet>() select i).ToList();
        }

        public int SaveSuperSetItem(SuperSet item)
        {
            if (item.Id != 0)
            {
                database.Update(item);
                return item.Id;
            }
            database.Insert(item);
            return GetLastInsertId();
        }

        public int DeleteSuperSetItem(int id)
        {
            return database.Delete<SuperSet>(id);
        }

        public void DeleteSuperSetsByTrainingId(int trainingId)
        {
            var allItems = GetSuperSetItems();
            foreach (var item in allItems)
            {
                if (item.TrainingId == trainingId)
                {
                    DeleteTrainingExerciseItem(item.Id);
                }
            }
        }
        #endregion

        #region Image
        public ImageData GetImage(string imageUrl)
        {
            return database.Find<ImageData>(a => a.Url == imageUrl);
        }

        public int SaveImage(ImageData item)
        {
            if (item.Id != 0)
            {
                database.Update(item);
                return item.Id;
            }
            database.Insert(item);
            return GetLastInsertId();
        }


        #endregion

        public IEnumerable<TrainingUnion> GetTrainingsGroups()
        {
            return (from i in database.Table<TrainingUnion>() select i).ToList();
        }

        public int SaveTrainingGroup(TrainingUnion item)
        {
            if (item.Id != 0)
            {
                database.Update(item);
                return item.Id;
            }
            database.Insert(item);
            return GetLastInsertId();
        }

        public TrainingUnion GetTrainingGroup(int id)
        {
            return database.Get<TrainingUnion>(id);
        }

        public int DeleteTrainingGroup(int id)
        {
            return database.Delete<TrainingUnion>(id);
        }

        public void SaveUpdate(string versionSelected)
        {
            database.DeleteAll<UpdateItem>();
            database.Insert(new UpdateItem()
                {
                    Version = versionSelected,
            });
        }

        public UpdateItem GetUpdates()
        {
            try
            {
                return (from i in database.Table<UpdateItem>() select i).First();

            }
            catch (Exception e)
            {
                return null;
            }
        }


        public void DeleteAll<T>()
        {
            database.DeleteAll<T>();
        }
    }
}
