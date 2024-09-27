using System.Text.RegularExpressions;

namespace ASGClient.Helpers
{
    public class IngredientsHelper
    {
        // Represents a parsed ingredient
        public class Ingredient
        {
            public string Name { get; set; }
            public double Quantity { get; set; }
            public string Unit { get; set; }
            public int RecipeCount { get; set; }
            public string Description { get; set; }

            public override string ToString()
            {
                string descriptionPart = !string.IsNullOrEmpty(Description) ? $" ({Description})" : "";
                return $"{Name} - {Quantity} {Unit} ({RecipeCount} recipe{(RecipeCount > 1 ? "s" : "")}){descriptionPart}";
            }
        }

        // Fraction word to numerical value mapping
        private readonly Dictionary<string, double> fractionWords = new Dictionary<string, double>
        {
            { "half", 0.5 },
            { "quarter", 0.25 },
            { "third", 1.0 / 3.0 },
            { "fourth", 0.25 }, // alternative to quarter
            { "fifth", 0.2 },
            { "sixth", 1.0 / 6.0 },
            { "seventh", 1.0 / 7.0 },
            { "eighth", 1.0 / 8.0 },
            { "ninth", 1.0 / 9.0 },
            { "tenth", 0.1 },
            { "eleventh", 1.0 / 11.0 },
            { "twelfth", 1.0 / 12.0 },
            { "one", 1.0 },
            { "two", 2.0 },
            { "three", 3.0 },
            { "four", 4.0 },
            { "five", 5.0 },
            { "six", 6.0 },
            { "seven", 7.0 },
            { "eight", 8.0 },
            { "nine", 9.0 },
            { "ten", 10.0 },
            { "eleven", 11.0 },
            { "twelve", 12.0 },
            { "three-quarters", 0.75 },
            { "three quarters", 0.75 },
            { "two-thirds", 2.0 / 3.0 },
            { "two thirds", 2.0 / 3.0 },
            { "one and a half", 1.5 },
            { "one-and-a-half", 1.5 },
            { "one half", 0.5 }, // variation of half
            { "two and a quarter", 2.25 },
            { "two and three-quarters", 2.75 },
            { "two and one quarter", 2.25 },
            { "two and a third", 2.33333 }
        };

        // Synonym mapping for common ingredient language variations
        // Expanded synonym mapping for common ingredient language variations
        private readonly Dictionary<string, string> ingredientSynonyms = new Dictionary<string, string>
        {
            // Vegetables
            { "aubergine", "eggplant" },
            { "courgette", "zucchini" },
            { "swede", "rutabaga" },
            { "broad beans", "fava beans" },
            { "rocket", "arugula" },
            { "spring onion", "green onion" },
            { "scallion", "green onion" },
            { "capsicum", "bell pepper" },
            { "mangetout", "snow peas" },
            { "beetroot", "beet" },
            { "chickpea", "garbanzo bean" },
            { "cress", "watercress" },
            { "yam", "sweet potato" },
            { "salad leaves", "lettuce" },
            { "spring greens", "cabbage" },
            { "celeriac", "celery root" },

            // Herbs and Spices
            { "coriander", "cilantro" },
            { "chilli", "chili" },
            { "chilli powder", "chili powder" },
            { "turmeric", "curcuma" },
            { "allspice", "pimento" },
            { "cloves garlic", "garlic" },
            { "aniseed", "fennel" },
            { "cardamom", "elaichi" },
            { "fresh thyme", "thyme" },

            // Fruits
            { "sultanas", "golden raisins" },
            { "papaya", "pawpaw" },
            { "stone fruit", "drupe" },
            { "sharon fruit", "persimmon" },
            { "pineapple", "ananas" },

            // Dairy
            { "double cream", "heavy cream" },
            { "single cream", "light cream" },
            { "curd", "yogurt" },
            { "curds", "yogurt" },
            { "wholemeal flour", "whole wheat flour" },
            { "strong flour", "bread flour" },
            { "buttermilk", "soured milk" },

            // Sweeteners
            { "caster sugar", "granulated sugar" },
            { "icing sugar", "powdered sugar" },
            { "confectioners' sugar", "powdered sugar" },
            { "demerara sugar", "brown sugar" },
            { "jaggery", "palm sugar" },
            { "golden syrup", "light molasses" },

            // Meats and Seafood
            { "prawn", "shrimp" },
            { "king prawn", "large shrimp" },
            { "bacon rashers", "bacon slices" },
            { "rashers", "bacon slices" },
            { "gammon", "ham" },
            { "minced meat", "ground meat" },
            { "minced beef", "ground beef" },
            { "lamb mince", "ground lamb" },
            { "chook", "chicken" },
            { "poultry", "chicken" },
            { "squid", "calamari" },

            // Oils, Vinegars, Sauces
            { "rapeseed oil", "canola oil" },
            { "groundnut oil", "peanut oil" },
            { "soya oil", "soybean oil" },
            { "fish sauce", "nam pla" },
            { "white vinegar", "distilled vinegar" },
            { "tomato ketchup", "tomato sauce" },
            { "kecap manis", "sweet soy sauce" },

            // Grains and Legumes
            { "polenta", "cornmeal" },
            { "semolina", "durum wheat" },
            { "barley", "pearl barley" },
            { "quinoa", "keen-wah" },
            { "freekeh", "green wheat" },

            // Nuts and Seeds
            { "groundnuts", "peanuts" },
            { "flaxseed", "linseed" },
            { "pumpkin seeds", "pepitas" },
            { "pistachio nuts", "pistachios" },

            // Plural Forms
            { "aubergines", "eggplants" },
            { "courgettes", "zucchinis" },
            { "corianders", "cilantros" },
            { "rockets", "arugulas" },
            { "scallions", "green onions" },
            { "prawns", "shrimps" },
            { "beetroots", "beets" }
        };

        // List of modifiers for packed/loose ingredients
        private readonly HashSet<string> packedModifiers = new HashSet<string>
        {
            "loosely packed", "firmly packed", "packed"
        };

        // Method to parse and aggregate a list of ingredients
        public List<Ingredient> ParseIngredients(List<string> ingredients)
        {
            var ingredientDict = new Dictionary<string, Ingredient>();

            foreach (var ingredientStr in ingredients)
            {
                var parsedIngredient = ParseIngredient(ingredientStr);
                if (parsedIngredient != null)
                {
                    // Normalize key by combining name and unit (avoiding unit mix-ups)
                    string key = $"{parsedIngredient.Name.ToLower()}_{parsedIngredient.Unit.ToLower()}";

                    if (ingredientDict.ContainsKey(key))
                    {
                        // Aggregate quantities and increment recipe count
                        ingredientDict[key].Quantity += parsedIngredient.Quantity;
                        ingredientDict[key].RecipeCount++;
                    }
                    else
                    {
                        ingredientDict[key] = parsedIngredient;
                    }
                }
            }

            return ingredientDict.Values.ToList();
        }

        // Method to parse individual ingredient string
        private Ingredient ParseIngredient(string ingredientStr)
        {
            // Clean the input by removing any stray characters
            ingredientStr = ingredientStr.Trim();

            // Enhanced regex pattern to capture fractions, numbers, units, and names
            string pattern = @"(?<quantity>\d+\/?\d*|\d+\s?\d*\/?\d*|half|quarter|third|three-quarters|one|two|three)?\s?(?<unit>cup|cups|tablespoon|tablespoons|teaspoon|teaspoons|pound|pounds|ounce|ounces|gram|grams|kilogram|kilograms|clove|cloves|inch|lb|lbs|tsp|tbsp|litre|litres|stick|sticks|sheet|can|jar|bag|bottle|pack|block)?\s?(?<name>.+)";
            var match = Regex.Match(ingredientStr, pattern);

            if (match.Success)
            {
                // Extract and handle quantity (including fractional words and combined numbers)
                string quantityStr = match.Groups["quantity"].Value.Trim().ToLower();
                double quantity = 1; // Default quantity if none provided

                if (!string.IsNullOrEmpty(quantityStr))
                {
                    // Handle mixed fractions like "1 1/2"
                    if (quantityStr.Contains(" "))
                    {
                        var parts = quantityStr.Split(' ');
                        if (parts.Length == 2 && double.TryParse(parts[0], out double whole) && ParseFraction(parts[1], out double fraction))
                        {
                            quantity = whole + fraction;
                        }
                    }
                    // Handle simple fractions (e.g., "1/2")
                    else if (ParseFraction(quantityStr, out double fraction))
                    {
                        quantity = fraction;
                    }
                    // Handle fractional words like "half", "quarter", etc.
                    else if (fractionWords.ContainsKey(quantityStr))
                    {
                        quantity = fractionWords[quantityStr];
                    }
                    else if (double.TryParse(quantityStr, out double parsedQuantity))
                    {
                        quantity = parsedQuantity;
                    }
                }

                // Extract unit and name, making sure there's no stray 's' or missing value
                string unit = match.Groups["unit"].Value.Trim();
                string name = match.Groups["name"].Value.Trim();

                // Check if the name starts with 's' and fix it (misparsed abbreviation or missing quantity)
                if (name.StartsWith("s "))
                {
                    name = name.Substring(1).Trim();  // Remove the stray 's' at the start
                }

                // Handle specific cases like lemons, limes, etc. where conversion to "liters" is incorrect
                if (name.Contains("lemon") && name.Contains("juice"))
                {
                    unit = "lemon";
                }
                else if (name.Contains("lime") && name.Contains("juice"))
                {
                    unit = "lime";
                }

                // Normalize the ingredient name
                name = NormalizeSynonyms(name);

                return new Ingredient
                {
                    Name = name,
                    Quantity = quantity,
                    Unit = unit,
                    RecipeCount = 1
                };
            }

            return null;
        }

        // Helper method to parse fractions
        private bool ParseFraction(string fractionStr, out double fraction)
        {
            fraction = 0;
            var parts = fractionStr.Split('/');
            if (parts.Length == 2 && double.TryParse(parts[0], out double numerator) && double.TryParse(parts[1], out double denominator))
            {
                fraction = numerator / denominator;
                return true;
            }
            return false;
        }

        // Normalize common abbreviations to standard units
        private string NormalizeUnit(string unit)
        {
            switch (unit.ToLower())
            {
                case "tsp": return "teaspoon";
                case "tbsp": return "tablespoon";
                case "lb": return "pound";
                case "l": return "liter";
                case "ml": return "milliliter";
                case "kg": return "kilogram";
                case "g": return "gram";
                default: return unit;
            }
        }

        // Normalize common ingredient synonyms (e.g., "aubergine" -> "eggplant")
        private string NormalizeSynonyms(string name)
        {
            string lowerName = name.ToLower();

            // Check and replace synonyms
            foreach (var synonym in ingredientSynonyms)
            {
                if (lowerName.Contains(synonym.Key))
                {
                    lowerName = lowerName.Replace(synonym.Key, synonym.Value);
                }
            }

            return lowerName;
        }

        // Detect and extract packed/loose modifiers from ingredient names
        private string GetPackedModifier(ref string name)
        {
            foreach (var modifier in packedModifiers)
            {
                if (name.ToLower().Contains(modifier))
                {
                    name = name.ToLower().Replace(modifier, "").Trim();
                    return modifier;
                }
            }
            return "";
        }
    }
}
