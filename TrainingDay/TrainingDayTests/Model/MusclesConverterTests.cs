using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrainingDay.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDay.Model.Tests
{
    [TestClass()]
    public class MusclesConverterTests
    {
        [TestMethod()]
        public void SetMusclesTest_Fill()
        {
            var exp = new List<MuscleViewModel>();
            exp.Add(new MuscleViewModel(MusclesEnum.Abdominal));
            //exp.Add(new MuscleViewModel(MusclesEnum.Biceps));
            //exp.Add(new MuscleViewModel(MusclesEnum.Buttocks));
            var act = MusclesConverter.SetMuscles(MusclesEnum.Abdominal);

            CollectionAssert.AreEquivalent(exp, act);
        }

        [TestMethod()]
        public void ConvertToStringTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ConvertFromStringToListTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ConvertFromListToStringTest()
        {
            Assert.Fail();
        }
    }
}