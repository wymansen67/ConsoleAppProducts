using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleAppProductsList.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [StringLength(100)]
        public string CategoryName { get; set; }

        [NotMapped]
        private static AppDbContext context = new AppDbContext();
        [NotMapped]
        private static bool showError = true;

        //Список id - категория
        public static List<Category> GetList()
        {
            List<Category> categories = context.Categories.ToList();
            return categories;
        }

        //Создать категорию
        public static void Create(string name)
        {
            try
            {
                Category category = context.Categories.FirstOrDefault(c => c.CategoryName.ToLower() == name.ToLower());
                if (category == null)
                {
                    Category categ = new Category();
                    categ.CategoryName = name;
                    context.Categories.Add(categ);
                    context.SaveChanges();
                    GenerateError("✓ - категория успешно добавлена.");
                }
                else GenerateError("❌ - категория уже существует.");
            }
            catch
            {
                GenerateError("❌ - не удалось создать категорию.");
            }
        }

        //Обновить категорию
        public static void Update(int id, string name)
        {
            try
            {
                Category category = context.Categories.FirstOrDefault(c => c.CategoryId == id);
                if (category != null)
                {
                    if (category.CategoryName.ToLower() != name.ToLower())
                    {
                        category.CategoryName = name;
                        context.SaveChanges();
                        GenerateError("✓ - категория успешно обновлена.");
                    }
                    else GenerateError("❌ - категория уже существует.");
                }
                else GenerateError("❌ - категория не найдена.");
            }
            catch
            {
                GenerateError("❌ - не удалось обновить категорию.");
            }
        }

        //Удалить категорию
        public static void Delete(int id)
        {
            try
            {
                Category category = context.Categories.FirstOrDefault(c => c.CategoryId == id);
                if (category != null)
                {
                    var products = context.Products.Where(p => p.CategoryId == id);
                    if (products.Count() > 0) { context.Products.RemoveRange(products); }
                    context.Categories.Remove(category);
                    context.SaveChanges();
                    GenerateError("✓ - категория успешно удалена.");
                }
                else GenerateError("❌ - категория не найдена.");
            }
            catch
            {
                GenerateError("❌ - не удалось удалить категорию.");
            }
        }

        public static bool CheckIfExists(int categoryId)
        {
            Category category = context.Categories.FirstOrDefault(p => p.CategoryId == categoryId);
            if (category != null) return true;
            return false;
        }

        public static void ChangeErrorDisplayStatus(bool errorStatus)
        {
            showError = errorStatus;
        }

        public static void GenerateError(string message)
        {
            if (showError)
            {
                if (message.Contains("❌")) { Console.ForegroundColor = ConsoleColor.Red; }
                else if (message.Contains("⚠")) { Console.ForegroundColor = ConsoleColor.Yellow; }
                else if (message.Contains("✓")) { Console.ForegroundColor = ConsoleColor.Green; }
                Console.WriteLine($"\n\t{message}");
                Console.ResetColor();
            }
        }
    }
}