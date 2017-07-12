using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace enchanting
{
    public class Recipe
    {
        public string Name { get; set; }
        public string Ingredient1 { get; set; }
        public string Ingredient2 { get; set; }
        public string Ingredient3 { get; set; }
        public string Ingredient4 { get; set; }
        public bool Craftable { get; set; }

        public Recipe(string name, string ingredient1, string ingredient2, string ingredient3, string ingredient4)
        {
            Name = name;
            Ingredient1 = ingredient1;
            Ingredient2 = ingredient2;
            Ingredient3 = ingredient3;
            Ingredient4 = ingredient4;
            Craftable = true;
        }
    }
}
