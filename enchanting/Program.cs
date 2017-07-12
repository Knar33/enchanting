using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace enchanting
{
    class Program
    {
        static void Main(string[] args)
        {

            /*
            Dictionaries:
            RecipeList              Recipe Name         => Object class Recipe          Recipe list from CSV file. { Name, Ingredient1, Ingredient2, Ingredient3, Ingredient4, Craftable }
            Ingredients             Ingredient Name     => ingredient amount            User input ingredient amounts, when a recipe is made the ingredients are subtracted from this
            CraftList               Recipe Name         => Craft amount                 When a recipe is selected, add to this list and print after while loop
            LimitingReagants        recipe name         => limiting reagant name        Track the limiting reagant of every recipe that can be made. Reset for every recipe checked.
            TempIngredientCol       ingredient Name     => ingredient amount            Temporary array to find the limiting reagant of a given recipe
            ConflictsArray          recipe name         => # conflicts                  Tracks how many conflicting ingredients a recipe has            

            Steps:
            Find the limiting ingredient for each recipe that we can make at least 1 of (limiting ingredient = the ingredient you have the least of in that recipe) track in LimitingReagants dictionary
            For every recipe that we can make at least 1 of:
                Check the 4 ingredients to see if they are the same as any of the limiting ingredients in LimitingReagants collection
                    for every match, add 1 to "conflicts" variable. This is how many fewer recipes you could create if you made 1 of this recipe. Conflicts is reset for each recipe checked
                track the recipe name and the amount of conflicts it has in ConflictsArray
            Find the recipe with the least amount of conflicts
            Add it to CraftList, and remove 1 of each of the required ingredients from Ingredients
            Repeat until there are no more recipes to make
            Print craftarray

            ToDo:
            Break down into methods, classes
            */

            Dictionary<string, Recipe>  RecipeList  = new Dictionary<string, Recipe>();
            Dictionary<string, int>     Ingredients = new Dictionary<string, int>();
            Dictionary<string, int>     CraftList   = new Dictionary<string, int>();
            List<string> ing = new List<string>();                                                              //Ingredient list populated from CSV (Populate from user form later) JS validate input?
        

            string line;
            StreamReader file = new StreamReader("enchants.csv");                                               
            while ((line = file.ReadLine()) != null)                                                            //read CSV line by line until you hit a null lines
            {
                String[] field = line.Split(',');
                RecipeList.Add(field[2], new Recipe(field[2], field[3], field[4], field[5], field[6]));         //create new entry in RecipeList dictionary

                for (int i = 3; i < 7; i++)
                    if (!ing.Contains(field[i]))                                                                //Check if ingredient is already in ingredient list
                       ing.Add(field[i]);                                                                       //if not, add ingredient to list
            }
            file.Close();
            
            Console.WriteLine("Enter your ingredients:");                                                       //user submitted ingredients                                                
            for (int i = 0; i < ing.Count; i++)                                                                 //Loop through all ingredients
            {
                Console.Write(ing[i] + ": ");
                Ingredients.Add(ing[i], CheckNum(Console.ReadLine()));                                           //Ask user to input for every ingredient
            }
            Console.WriteLine();
            Console.WriteLine("Output:");
            int checksuccess = 0;                                                                               //variable to see if something was successfully added to craft list. If not, wrap up program and kill loop
            int x = 1;                                                                                          //variable to kill loop, x=0 when checksuccess=0, happens when nothing is craftable
            
            while (x == 1)                                                                                      //loops until there are no more things to make
            {                
                Dictionary<string, string> LimitingReagants = new Dictionary<string, string>();                 //create array to track the limiting reagants, recipename => limiting reagant name     
                                                                                                                //does variable created in while loop go out of scope each iteration?
                checksuccess = 0;                                                                               //reset success check
                foreach (KeyValuePair<string, Recipe> recipe in RecipeList)
                {                    
                    if ((Ingredients[recipe.Value.Ingredient1] > 0)                                             //if we have at least 1 of each of the required ingredients
                        && (Ingredients[recipe.Value.Ingredient2] > 0) 
                        && (Ingredients[recipe.Value.Ingredient3] > 0) 
                        && (Ingredients[recipe.Value.Ingredient4] > 0))                                         
                    {                        
                        checksuccess = 1;                                                                       //Something can be made                        
                        Dictionary<string, int> TempIngredientCol = new Dictionary<string, int>();              //add all ingredients and their values to a temporary dictionary, ingredient name => ingredient amount
                        TempIngredientCol.Add(recipe.Value.Ingredient1, Ingredients[recipe.Value.Ingredient1]);
                        TempIngredientCol.Add(recipe.Value.Ingredient2, Ingredients[recipe.Value.Ingredient2]);
                        TempIngredientCol.Add(recipe.Value.Ingredient3, Ingredients[recipe.Value.Ingredient3]);
                        TempIngredientCol.Add(recipe.Value.Ingredient4, Ingredients[recipe.Value.Ingredient4]);
                        
                        var Limiter = TempIngredientCol.OrderBy(key => key.Value).Take(1);                      //sort dictionary to find the ingredient we have the least of
                        foreach (KeyValuePair<string, int> limiter in Limiter)
                            LimitingReagants.Add(recipe.Key, limiter.Key);
                    }
                    else
                    {
                        RecipeList[recipe.Key].Craftable = false;
                    }
                }

                if (checksuccess == 1)                                                                          //check to see if something can be made
                {                    
                    Dictionary<string, int> ConflictArray = new Dictionary<string, int>();                      //conflict array to track how many conflicts each recipe has
                    int conflicts;                    
                    foreach (KeyValuePair<string, Recipe> recipe in RecipeList)                                 //loop through remaining recipe list
                    {
                        conflicts = 0;
                        if (RecipeList[recipe.Key].Craftable)
                        {                            
                            foreach (KeyValuePair<string, string> limitingreagants in LimitingReagants)         //loop through all recipes, then check for conflicts. use linq instead
                            {                                                                                                                
                                if (limitingreagants.Value == recipe.Value.Ingredient1)                         //conflict checks. /for each conflict, +1 to conflict variable
                                    conflicts++;                                                                //can you condense and ++ for each true result? like: if ((limitingreagants.Value == recipe.Value.Ingredient1) || (limitingreagants.Value == recipe.Value.Ingredient2) || (limitingreagants.Value == recipe.Value.Ingredient3) || (limitingreagants.Value == recipe.Value.Ingredient4))    
                                if (limitingreagants.Value == recipe.Value.Ingredient2)                         
                                    conflicts++;
                                if (limitingreagants.Value == recipe.Value.Ingredient3)
                                    conflicts++;
                                if (limitingreagants.Value == recipe.Value.Ingredient4)
                                    conflicts++;
                            }
                            ConflictArray.Add(recipe.Key, conflicts);
                        }
                    }
                    
                    var ConflictCheck = ConflictArray.OrderBy(key => key.Value).Take(1);                        //find the recipe with the least amount of conflicts - find better name
                    foreach (KeyValuePair<string, int> conflictcheck in ConflictCheck)
                    {
                        if (CraftList.ContainsKey(conflictcheck.Key))                                           //add that recipe to craft list,
                        { 
                            CraftList[conflictcheck.Key]++;
                        } else
                        {
                            CraftList.Add(conflictcheck.Key, 1);
                        }

                        Console.WriteLine(conflictcheck.Key);                                                   //Debug
                        Ingredients[RecipeList[conflictcheck.Key].Ingredient1]--;                               //remove its ingredients from collection
                        Ingredients[RecipeList[conflictcheck.Key].Ingredient2]--;
                        Ingredients[RecipeList[conflictcheck.Key].Ingredient3]--;
                        Ingredients[RecipeList[conflictcheck.Key].Ingredient4]--;
                    }
                }
                else
                {                    
                    x = 0;                                                                                      //end loop
                }
            }
            Console.WriteLine();
            Console.WriteLine("Craft List: ");
            foreach (KeyValuePair<string, int> craftlist in CraftList)
            {
                Console.WriteLine("{0}: {1}", craftlist.Key, craftlist.Value);                                  //print out craftinglist array.
            }                
            Console.ReadLine();
        }

        public static int CheckNum(string userEntry)                                                            //Validate user entry
        {
            if (Int32.TryParse(userEntry, out int result))
                return result;
            else
                return 0;
        }
    }
}
