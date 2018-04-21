using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using TrainingDay.Helpers;
using TrainingDay.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace TrainingDay.Model
{
    public interface ISQLite
    {
        string GetDatabasePath(string filename);
    }

    public class Repository
    {
        SQLiteConnection database;

        public Repository(string filename)
        {
            string databasePath = DependencyService.Get<ISQLite>().GetDatabasePath(filename);
            database = new SQLiteConnection(databasePath);
            //DropTable();
            if (Settings.IsFirstTime)
                InitBasic();
        }

        private void DropTable()
        {
            try
            {
                Settings.IsFirstTime = true;
                database.DeleteAll<Exercise>();
                database.DeleteAll<Training>();
            }
            catch
            {
                //ignore
            }
        }

        //функция подгружающая в программу стандартные тренировки
        private void InitBasic()
        {
            Settings.IsFirstTime = false;
            database.CreateTable<Exercise>();
            database.CreateTable<TrainingExerciseComm>();
            database.CreateTable<Training>();
            database.CreateTable<LastTraining>();
            database.CreateTable<WeightNote>();
            var Exercises = new ObservableCollection<Exercise>();

            // ноги
            var foots = InitDefaultExecricesOfFoot();
            // грудь
            var chests = InitDefaultExecricesOfChest();
            // спина
            var rear = InitDefaultExecricesOfRear();
            var hands = InitDefaultExecricesOfHands();

            foots.ForEach(exercise => Exercises.Add(exercise));
            chests.ForEach(exercise => Exercises.Add(exercise));
            rear.ForEach(exercise => Exercises.Add(exercise));
            hands.ForEach(exercise => Exercises.Add(exercise));
            foreach (var exercise in Exercises)
            {
                SaveExerciseItem(exercise);
            }
        }

        private ObservableCollection<Exercise> InitDefaultExecricesOfHands()
        {
            var ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            var Exercises = new ObservableCollection<Exercise>();
            // ноги
            Exercises.Add(new Exercise()
            {
                Muscles = MusclesConverter.SetMuscles(MusclesEnum.Shoulders, MusclesEnum.Trapezium, MusclesEnum.Deltoid),
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = ci.Name == "ru-Ru"?"Поочерёдные подъёмы рук с гантелями вперёд": "Alternete hand lifts with dumbbells",
                ShortDescription = "сидя или стоя, гантели в опущенных руках, перед бёдрами. Поднимите одну гантель вперед до уровня глаз. Опустите гантель в исходное положение, одновременно поднимая другую гантель. Гантели не должны расходиться "
            });

            Exercises.Add(new Exercise()
            {
                Muscles = MusclesConverter.SetMuscles(MusclesEnum.Shoulders, MusclesEnum.Trapezium, MusclesEnum.Deltoid),
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = ci.Name == "ru-Ru" ? "Подъёмы рук с гантелями через стороны": "Rises of hands with dumbbells through the sides",
                ShortDescription = "возьмите  гантели, ноги на ширину плеч, немного наклонитесь вперед, согните руки в локтях и поднимите гантели по широкой дуге вверх и немного вперед на уровень чуть выше плеч. Ладони при этом направлены вниз. В верхней точке мизинцы должны быть выше больших пальцев. Задержитесь в этом положении и медленно опустите руки в исходное положение. Для большего эффекта старайтесь не раскачивать тело, лучше выполнять сидя."
            });

            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = ci.Name == "ru-Ru" ? "Сгибания рук со штангой": "Bending of arms with a barbell",
                ShortDescription = "возьмите гриф хватом снизу так, чтобы руки находились на расстоянии немного превышающем ширину Ваших плеч. В исходном положении гриф должен находиться в опущенных руках чуть ниже пояса. Для правильного исполнения этого упражнения необходимо прижимать локти к туловищу и сохранять их неподвижными в течение всего подхода. Из исходного положения поднимите штангу к плечам, в этой точке необходимо задержатся на секунду."
            });

            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = ci.Name == "ru-Ru" ? "Сгибания рук со штангой на \"скамье проповедника\"": "Bending of hands with a barbell on a \"preacher's bench\"",
                ShortDescription = "займите исходное положение, упритесь грудью в так называемую \"скамью проповедника\" и возьмите штангу хватом снизу. В таком положении бицепсы должны быть параллельными друг другу. Сохраняя туловище неподвижным, поднимите штангу вверх, тянув её к плечам, затем, сохраняя контроль над весом, опустите штангу до полного выпрямления рук."
            });

            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = ci.Name == "ru-Ru" ? "Сгибания рук со штангой обратным хватом на \"скамье проповедника\"": "Bending of hands with a bar with a back grip on a \"preacher's bench\"",
                ShortDescription = "займите исходное положение расположив руки на \"скамье проповедника\". Возьмитесь за гриф штанги хватом сверху на ширине плеч. Сгибая руки в локтях, медленно поднимайте штангу вверх и также медленно, сохраняя контроль над весом, верните ее в исходное положение. При выполнении упражнения необходимо удерживать корпус неподвижно"
            });

            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = ci.Name == "ru-Ru" ? "Сгибания рук с гантелями стоя, сидя" : "Bending of hands with dumbbells while standing or sitting",
                ShortDescription = "Стоя: исходное положение, стоя прямо, гантели в опущенных руках. Сохраняя корпус прямым, одновременно поднимайте обе гантели к плечам. При этом запястья слегка поворачивайте наружу, чтобы максимально сократить бицепс. Медленно, сопровождая вес, опустите гантели вниз в исходное положение. "
            });

            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "Поочерёдные сгибания рук с гантелями стоя",
                ShortDescription = "исходное положение, стоя прямо, гантели в опущенных руках, ладони обращены друг к другу. Сохраняйте корпус прямым и удерживайте локоть неподвижным, согните одну руку, и поднимайте гантель к плечу, одновременно поворачивая ладонь к себе и наружу для усиления сокращения мышцы. Медленно вернитесь в исходное положение. Выполните подход другой рукой."
            });

            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "КОНЦЕНТРИРОВАННЫЕ СГИБАНИЯ СИДЯ",
                ShortDescription = "исходное положение, сидя на краю скамьи. Возьмите гантель в руку, опустите ее вниз и упритесь задней поверхностью руки в бедро. Сгибая руку в локте, поднимите гантель к плечу, одновременно поворачивая кисть наружу. Задержитесь в этом положении на секунду, затем плавно вернитесь в исходное положение. Повторите движение. Выполните подход другой рукой."
            });

            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "РАЗГИБАНИЯ РУКИ НАЗАД В НАКЛОНЕ",
                ShortDescription = "возьмите гантель в одну руку, наклонитесь и другой рукой упритесь в скамью или колено, поставьте одну ногу впереди другой. Руку с гантелей согните в локте на 90° так, чтобы предплечье было перпендикулярно полу. Из этого положения, сохраняя локоть неподвижным и не отрывая руку от туловища, поднимите гантель так, чтобы в верхней точке предплечье оказалось параллельным полу. Задержитесь в этом положении на несколько секунд и по тойже траектории вернитесь в исходное положение. На протяжении всего упражнения ладонь обращена к себе."
            });

            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "РАЗГИБАНИЯ ОДНОЙ РУКИ В ПОЛОЖЕНИИ СТОЯ",
                ShortDescription = "исходное положение, лёжа на скамье, гантели подняты над головой, ладони обращены друг к другу, руки параллельны друг другу. Из этого положения, сохраняя локти неподвижными, опустите гантели вниз. По тойже траектории вернитесь в исходное положение."
            });
            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "ЖИМ ЛЁЖА УЗКИМ ХВАТОМ",
                ShortDescription = "исходное положение лёжа на скамье, обхватите гриф хватом сверху на расстоянии 15-20см. Снимите штангу со стоек и выпрямите руки вверх так, чтобы плечевые суставы были под грифом. Медленно опустите штангу так, чтобы гриф коснулся верхней части груди. Локти в нижней точке должны быть приблизительно 45° к телу."
            });

            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "РАЗГИБАНИЯ РУК ИЗ-ЗА ГОЛОВЫ",
                ShortDescription = "исходное положение, сидя на горизонтальной скамье. Обхватите гриф штанги узким хватом сверху и поднимите штангу над головой. Из этого положения, не двигая локтями, опустите штангу за голову. По той же траектории поднимите штангу вверх. На протяжении всего упражнения локти неподвижны и прижаты максимально близко к голове."
            });
            return Exercises;
        }

        private ObservableCollection<Exercise> InitDefaultExecricesOfRear()
        {
            var Exercises = new ObservableCollection<Exercise>();
            // ноги
            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "СТАНОВАЯ ТЯГА",
                ShortDescription = "обхватите гриф штанги хватом сверху и, удерживая голову на одной линии со спиной, полностью выпрямитесь. Наклонитесь вперед так, чтобы в нижней точке спина оказалась параллельной полу. Медленно вернитесь в исходное положение. Ноги не должны сгибаться на протяжении всего упражнения."
            });

            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "ТЯГА ШТАНГИ К ПОЯСУ В НАКЛОНЕ",
                ShortDescription = "исходное положение, ноги на ширине плеч. Немного сгибая ноги в коленях, наклонитесь вперёд так, чтобы корпус оказался параллельным полу. Обхватите гриф штанги хватом сверху, на расстоянии чуть больше ширины плеч. Смотря вперёд, удерживая спину ровной, подтяните штангу к нижней части груди. Опустите штангу в исходное положение."
            });
            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "\"ГУД МОРНИНГ\"",
                ShortDescription = "возьмите лёгкую штангу хватом сверху и расположите её на плечах. Наклонитесь вперёд, удерживая спину и голову прямой так, чтобы спина оказалась параллельной полу. На протяжении всего упражнения ноги остаются прямыми. Вернитесь в исходное положение."
            });
            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "ШРАГИ СО ШТАНГОЙ",
                ShortDescription = "обхватите гриф штанги хватом сверху. Удерживая спину прямой, поднимите плечи на максимальную высоту, в верхней точке задержитесь, затем медленно опустите плечи."
            });
            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "МЁРТВАЯ ТЯГА",
                ShortDescription = "исходное положение - ноги на ширине плеч. Сгибая ноги в коленях, удерживая спину прямой, присядьте и обхватите гриф штанги хватом сверху на расстоянии ширины плеч. Отводя грудь вперёд, удерживая спину прямой, выпрямитесь до вертикального положения."
            });
            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "ТЯГА ШТАНГИ К ПОДБОРОДКУ",
                ShortDescription = "ноги на ширине плеч. Возьмите гриф хватом сверху на расстоянии 15-20 см, поднимите штангу к подбородку, удерживая ее максимально близко к телу. Спина должна быть прямой, локти выше грифа. Опустите штангу в исходное положение. Выполните следующее повторение."
            });
            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "ТЯГА ГАНТЕЛЕЙ К ПОЯСУ В НАКЛОНЕ",
                ShortDescription = "исходное положение - ноги слегка согнуты в коленях, спина параллельна полу, гантели в опущенных руках, ладони обращены друг к другу, голова поднята вверх. Подтягиваем гантели к поясу максимально высоко. Опустите гантели в исходное положение. Повторите движение."
            });
            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "ТЯГА ОДНОЙ ГАНТЕЛИ К ПОЯСУ В НАКЛОНЕ",
                ShortDescription = "исходное положение - стоя, спина параллельна полу, одна рука держится за опору, другой рукой возьмите гантель, ладонь обращена к себе. Сохраняя корпус неподвижным, подтяните гантель к груди максимально высоко. Опустите гантель в исходное положение."
            });
            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "ШРАГИ С ГАНТЕЛЯМИ",
                ShortDescription = "исходное положение стоя, ноги на ширине плечей, гантели в опущенных руках, ладони обращены друг к другу. Из этого положения поднимите плечи максимально высоко, в верхней точке задержитесь. Вернитесь в исходное положение."
            });
            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "ВЕРХНЯЯ ТЯГА ТРОСА ЗА ГОЛОВУ",
                ShortDescription = "обхватите ручки тренажёра хватом сверху, хватом чуть шире плеч. Колени закреплены, спина прогнута. Подтяните ручку тренажёра до затылка. Вернитесь в исходное положение. Это упражнение подходит не каждому, всё зависит от строения скелета. Если вы чувствуете дискомфорт при выполнении этого упражнения, чтобы не получить травму, стоит от него отказаться."
            });
            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "ВЕРХНЯЯ ТЯГА ТРОСА К ГРУДИ",
                ShortDescription = "сядьте на тренажёр, закрепите ноги, возьмитесь за ручки или стержень, ладони обращены друг к другу. Подтяните трос к себе. Вернитесь в исходное положение. До касания ручек грудью старайтесь не отклонять корпус назад, чтобы не подключались мышцы нижней части спины."
            });
            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "ТЯГА ТРОСА К ПОЯСУ",
                ShortDescription = "исходное положение - сидя на тренажёре, ноги упираются в подставку, немного согнуты, руки держатся за ручки. Сохраняя спину прогнутой (лопатки стараются коснуться друг друга), подтяните ручку тренажёра к прессу."
            });
            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "ПОДТЯГИВАНИЯ НА ПЕРЕКЛАДИНЕ",
                ShortDescription = "продемонстрировано подтягивание за голову с использованием широкого хвата. Виснем на турнике, хват должен быть таким, чтобы в верхней точке предплечья были параллельны друг другу. Подтягиваемся до касания перекладины трапециевидных мышц. Медленно возвращаемся в исходное положение. Упражнение может быть травмоопасным, в зависимости от индивидуального строения скелета. Если чувствуете дискомфорт при таком выполнении, откажитесь от этого упражнения. Выполняйте его в варианте, как показано на второй фотографии. Тоже самое, только в верхней точке касаемся перекладины грудью."
            });
            return Exercises;
        }

        private ObservableCollection<Exercise> InitDefaultExecricesOfFoot()
        {
            var Exercises = new ObservableCollection<Exercise>();
            // ноги
            Exercises.Add(new Exercise()
            {
                Muscles = MusclesConverter.SetMuscles(MusclesEnum.Caviar, MusclesEnum.Thighs),
                Weight = 35,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "Сгибания ног на тренажёре",
                ShortDescription = "Исходное положение - лёжа на скамье тренажёра лицом вниз, ступни заведены за валиками. Из этого положения согните ноги в коленях, стараясь коснуться пятками ягодиц. Опустите ноги в исходное положение."
            });

            Exercises.Add(new Exercise()
            {
                Weight = 35,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "Разгибания ног на тренажёре",
                ShortDescription =
                    "Исходное положение - сидя на тренажёре, ступни заведены за валики. Из этого положения разогните ноги в коленях."
            });
            Exercises.Add(new Exercise()
            {
                Weight = 3,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "Приседания со штангой на плечах",
                ShortDescription =
                    "Исходное положение - стоя спиной к штанге, лежащей на стойках. Приподнимите штангу плечами и снимите её со стоек, сделайте шаг вперед. Сохраняя туловище (чтобы не травмировать спину) и голову прямыми (выберите перед собой точку выше своего роста и смотрите на неё), медленно присядьте. Поднимитесь в исходное положение. Под пятки желательно подкладывайте блин, как на левой фотографии. Приседая, делаем вдох, поднимаясь - выдох."
            });


            Exercises.Add(new Exercise()
            {
                Weight = 35,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "Жим ногами на тренажере под углом 45 градусов",
                ShortDescription = "Исходное положение, лёжа на тренажёре для жима ногами. Упритесь ступнями в платформу с грузом. Из этого положения поднимите платформу вверх, затем вниз. Во избежание травмирования коленных суставов, выполняйте движение непрерывно. Можно использовать разное положение ступней."
            });

            Exercises.Add(new Exercise()
            {
                Weight = 35,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "Разведение ног на тренажере",
                ShortDescription = "Исходное положение - сидя на тренажере: - сделать вдох и развести бедра с максимально возможной амплитудой."
            });

            Exercises.Add(new Exercise()
            {
                Weight = 35,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "Приседания на тренажёре",
                ShortDescription = "Заведите плечи под гриф тренажёра и выпрямитесь. Из этого положения, сохраняя спину и голову прямой, присядьте до положения, чтобы таз оказался ниже коленей. Вернитесь в исходное положение."
            });


            Exercises.Add(new Exercise()
            {
                Weight = 35,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "Жим штанги лежа",
                ShortDescription = "Лежа на горизонтальной скамье, штанга берется двумя руками с креплений и опускается к середине груди до легкого касания. Затем без выдоха штанга выжимается вверх до момента фиксирования локтей. Ноги стоят на полу, лопатки сведены, грудь выставлена вперед, ягодицы прижаты к скамье."
            });


            Exercises.Add(new Exercise()
            {
                Weight = 35,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "Сведение ног в тренажере",
                ShortDescription = "Обязательное условие при сведении ног на тренажере -двигаться плавно. Это убережет вас от растяжений внутренних мышц бедра. Шаг 1. Сидя на тренажере так, чтобы валики располагались на внутренней части бедер, сделайте вдох и сведите бедра с максимально возможной амплитудой. Шаг 2. Задержитесь на 1-2 с в конечной точке. Шаг 3. Плавно, не бросая вес, вернитесь в исходное положение. "
            });

            Exercises.Add(new Exercise()
            {
                Weight = 35,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "Приседания со штангой на груди",
                ShortDescription = "Исходное положение - стоя перед штангой, лежащей на стойках, захватите гриф, как показано на фотографии, и сделайте шаг вперёд. Сохраняя туловище (чтобы не травмировать спину) и голову прямыми (выберите перед собой точку выше своего роста и смотрите на неё), медленно присядьте. Поднимитесь в исходное положение. Под пятки желательно подкладывайте блин. Приседая, делаем вдох, поднимаясь - выдох."
            });

            return Exercises;
        }

        private ObservableCollection<Exercise> InitDefaultExecricesOfChest()
        {
            var Exercises = new ObservableCollection<Exercise>();
            Exercises.Add(new Exercise()
            {
                Weight = 20,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "СВЕДЕНИЕ РУК НА ТРЕНАЖЁРЕ БАТТЕРФЛЯЙ(БАБОЧКА)",
                ShortDescription = "исходное положение, сидя на тренажёре, возьмитесь за ручки тренажёра, локти отведены максимально назад, они должны быть на одном уровне с плечами. Сведите руки до касания рычагов друг друга. Вернитесь в исходное положение. На протяжении всего упражнения спина не должна отрываться от спинки тренажёра."
            });

            Exercises.Add(new Exercise()
            {
                Weight = 10,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "СВЕДЕНИЕ РУК НА ТРЕНАЖЁРЕ ДЛЯ 'КРОССОВЕРОВ'",
                ShortDescription = "исходное положение стоя, немного наклонитесь вперёд, ручки тренажёра в выпрямленных руках. Сведите руки друг к другу, перед собой. "
            });


            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "ЖИМ ШТАНГИ ЛЕЖА НА НАКЛОННОЙ СКАМЬЕ",
                ShortDescription = "исходное положение, лёжа на наклонной скамье. Обхватите гриф штанги хватом сверху, снимите штангу со стоек и выпрямите руки. Опустите штангу на уровень ключицы. Вернитесь в исходное положение. Выполняя данное упражнение, старайтесь, чтобы локти были разведены в стороны."
            });

            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "ПУЛОВЕР С ГАНТЕЛЕЙ",
                ShortDescription = "исходное положение, лёжа на скамье. Обхватите гантель двумя руками и поднимите ее над грудью. Опустите гантель максимально низко за голову, затем поднимите гантель вверх по такой же траектории. "
            });

            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "ЖИМ ШТАНГИ ЛЁЖА НА ГОРИЗОНТАЛЬНОЙ СКАМЬЕ",
                ShortDescription = "лягте на горизонтальную скамью так, чтобы гриф находился над глазами, спина немного прогнута (так, чтобы таз не отрывался от скамьи). Хват должен быть таким, чтобы в нижней точке кисти находились над локтями, ноги стоят в упоре. Из этого положения снимите штангу со стоек и медленно, делая вдох, опустите ее в центр грудных мышц. Старайтесь разводить локти в стороны, чтобы задействовать мышцы груди полностью. Делая выдох, вернитесь в исходное положение. Выполняйте упражнение с полной амплитудой, голова не должна отрываться от скамьи."
            });

            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "РАЗВЕДЕНИЕ РУК С ГАНТЕЛЯМИ, ЛЁЖА НА СКАМЬЕ",
                ShortDescription = "исходное положение, лёжа на скамье. Гантели подняты над грудью, ладони должны быть, как показано на картинке (расстояние между мизинцев больше расстояния между большими пальцами), иначе вовлекутся в работу бицепсы. На протяжении всего упражнения гантели в верхней точке не должны пересекать вертикаль, то есть расстояние от кисти до кисти должно быть 20-30см, чтобы грудные мышцы в верхней точке постоянно получали нагрузку. Медленно разведите руки до горизонтального положения по отношению к полу, в нижней точке предплечья должны быть расположены на 45° по отношению к телу. Также следите за тем, чтобы руки в любой позиции образовывали с туловищем прямой угол. В верхней точке сократите мышцы груди, чтобы дать им дополнительную нагрузку. Не рекомендуется выполнять это упражнение прямыми руками"
            });

            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "ПУЛОВЕР С EZ-ШТАНГОЙ",
                ShortDescription = "исходное положение, лёжа на скамье. Обхватите гриф хватом сверху на расстоянии 20-25см, руки перпендикулярны полу. Из этого положения занесите штангу за голову максимально низко. "
            });

            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "СВЕДЕНИЕ РУК НА ТРОСОВОМ ТРЕНАЖЁРЕ, ЛЕЖА НА НАКЛОННОЙ СКАМЬЕ",
                ShortDescription = "исходное положение, лёжа на наклонной скамье, установленной между блоками со шкивами. Ручки тренажёра должны быть установлены на самый нижний уровень. Обхватите ручки тренажёра хватом снизу, немного согните руки в локтях. Сведите руки перед собой по широкой дуге. "
            });

            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "ЖИМ ЛЁЖА УЗКИМ ХВАТОМ",
                ShortDescription = "исходное положение - лёжа на скамье, обхватите гриф хватом сверху на расстоянии 15-20см. Снимите штангу со стоек и выпрямите руки вверх так, чтобы плечевые суставы были под грифом. Медленно опустите штангу так, чтобы гриф коснулся верхней части груди. Локти в нижней точке должны быть приблизительно 45° к телу."
            });

            Exercises.Add(new Exercise()
            {
                Weight = 5,
                CountOfApproches = 3,
                CountOfTimes = 15,
                ExerciseItemName = "ЖИМ ГАНТЕЛЕЙ ЛЁЖА НА ГОРИЗОНТАЛЬНОЙ СКАМЬЕ",
                ShortDescription = "исходное положение, лёжа на скамье. Возьмите гантели хватом сверху, предплечья перпендикулярны полу, локти опущены максимально низко. Поднимите гантели вверх, выпрямляя руки. "
            });

            Exercises.Add(new Exercise()
            {
                Weight = 0,
                CountOfApproches = 0,
                CountOfTimes = 15,
                ExerciseItemName = "ОТЖИМАНИЯ НА БРУСЬЯХ",
                ShortDescription = "Исходное положение - локти разведены, грудь округлена. Медленно, вдыхая, опуститесь не глубоко, затем, выдыхая, поднимитесь в исходное положение. В верхней точке движения дополнительно напрягите мышцы груди. В этом упражнении, чем больше вы наклоняетесь вперед, тем большую нагрузку получают мышцы груди, соответственно, чем ровнее ваш корпус, тем больше работают трицепсы."
            });

            return Exercises;
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
            return database.Insert(item);
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
            return database.Insert(item);
        }

        public List<Exercise> GetTrainingExerciseItemByTraningId(int trainingId)
        {
            List<Exercise> items = new List<Exercise>();
            var allItems = GetTrainingExerciseItems();
            foreach (var trainingExerciseComm in allItems)
            {
                if (trainingExerciseComm.TrainingId == trainingId)
                {
                    items.Add(GetExerciseItem(trainingExerciseComm.ExerciseId));
                }
            }

            return items;
        }


        public void DeleteTrainingExerciseItemByTraningId(int trainingId)
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

        public int GetLastInsertId()
        {
            return (int)SQLite3.LastInsertRowid(database.Handle);
        }

        #region Training Methods
        public IEnumerable<Training> GetTrainingItems()
        {
            return (from i in database.Table<Training>() select i).ToList();
        }

        public Training GetTrainingItem(int id)
        {
            return database.Get<Training>(id);
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
            return database.Insert(item);
        }
        #endregion

    }
}
