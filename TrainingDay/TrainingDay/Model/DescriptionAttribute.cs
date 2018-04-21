using System;

namespace TrainingDay.Model
{
    internal class DescriptionAttribute : Attribute
    {
        public string InfoRu;
        public string InfoEn;
        public DescriptionAttribute(string infoRu, string infoEn)
        {
            InfoEn = infoEn;
            InfoRu = infoRu;
        }
    }
}