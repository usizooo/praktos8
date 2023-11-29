using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_8
{
    // Здесь просто храним в списке тексты для уровней.
    public static class Levels
    {
        public static List<string> Texts { get; private set; } = new List<string>();

        public static void InitLevels(List<string> texts) => Texts = texts;
    }
}