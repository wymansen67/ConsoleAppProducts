using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;

namespace ConsoleAppProductsList.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        
        [StringLength(250)]
        public string ProductName { get; set; }

        public string? ProductImagePath { get; set; }

        [StringLength(500)]
        public string? ProductDescription { get; set; }

        public int ProductQuantity { get; set; }

        public long ProductPrice { get; set; }

        public int ProductDiscount { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        [NotMapped]
        private static AppDbContext context = new AppDbContext();
        [NotMapped]
        private static bool showError = true;

        //Список продуктов
        public static List<Product> GetList()
        {
            List<Product> products = context.Products.ToList();
            return products;
        }

        //Список продуктов по категориям
        public static List<Product> GetByCategory(int categoryId)
        {
            List<Product> products = context.Products.Where(p => p.CategoryId == categoryId).ToList();
            return products;
        }

        //Три самых дорогих предмета
        public static List<Product> GetTopThreeMostExpensive()
        {
            List<Product> products = context.Products.OrderByDescending(p => p.ProductPrice).Take(3).ToList();
            return products;
        }

        //Поиск продукта
        public static List<Product> Search(string searchQuery)
        {
            List<Product> products = context.Products.Where(p => p.ProductName.ToLower().Contains(searchQuery.ToLower())).ToList();
            return products;
        }

        //Создать продукт
        public static void Create(string name, string description, string image, int quantity, long price, byte discount, int categoryId)
        {
            try
            {
                Product product = context.Products.FirstOrDefault(c => c.ProductName.ToLower() == name.ToLower());
                if (product == null)
                {
                    string imagec;
                    string descriptionc;

                    if (!string.IsNullOrEmpty(image)) { imagec = image; }
                    else imagec = null;

                    if (!string.IsNullOrEmpty(description)) { descriptionc = description; }
                    else descriptionc = null;

                    Product prod = new Product()
                    {
                        ProductName = name,
                        ProductDescription = descriptionc,
                        ProductImagePath = imagec,
                        ProductQuantity = quantity,
                        ProductPrice = price,
                        ProductDiscount = discount,
                        CategoryId = categoryId
                    };

                    context.Products.Add(prod);
                    context.SaveChanges();

                   GenerateError("✓ - товар успешно добавлен.");
                }
                else GenerateError("❌ - такой товар уже существует.");
            }
            catch
            {
                GenerateError("❌ - не удалось обновить товар.");
            }
        }

        //Обновить продукт
        public static void Update(int id, string name, string description, string image, int quantity, long price, byte discount, int categoryId)
        {
            Product product = context.Products.FirstOrDefault(p => p.ProductId == id);

            if (product != null)
            {
                if (!string.IsNullOrEmpty(name)) product.ProductName = name;

                if (!string.IsNullOrEmpty(image)) { product.ProductImagePath = image; }
                else product.ProductImagePath = null;

                if (!string.IsNullOrEmpty(description)) { product.ProductDescription = description; }
                else product.ProductDescription = null;

                product.ProductPrice = price;

                product.ProductDiscount = discount;

                product.CategoryId = categoryId;

                context.SaveChanges();

                GenerateError("✓ - товар успешно обновлён.");
            }
            else { GenerateError("❌ - не удалось обновить товар."); }
        }

        //Обновить кол-во продукта
        public static void Update(int id, int quantity)
        {
            Product product = context.Products.FirstOrDefault(p => p.ProductId == id);

            if (product != null)
            {
                product.ProductQuantity = quantity;
                context.SaveChanges();

                GenerateError("✓ - товар успешно обновлён.");
            }
            else { GenerateError("❌ - не удалось обновить товар."); }
        }

        //Удалить товар
        public static void Delete(int id)
        {
            Product product = context.Products.FirstOrDefault(p => p.ProductId == id);
            if (product != null)
            {
                try
                {
                    context.Products.Remove(product);
                    context.SaveChanges();

                    GenerateError("✓ - товар успешно обновлён.");
                }
                catch
                {
                    GenerateError("❌ - не удалось обновить товар.");
                }
            }
            else { GenerateError("❌ - товар не найден."); }
        }

        //Проверка на существование продукта
        public static bool CheckIfExists(int productId)
        {
            Product product = context.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product != null) return true; 
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