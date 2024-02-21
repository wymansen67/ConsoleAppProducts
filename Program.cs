using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using ConsoleAppProductsList.Models;
using Microsoft.IdentityModel.Tokens;

namespace ConsoleAppProductsList
{
    internal class Program
    {
        public static bool showError = true;
        public static bool exit = false;
        public static Regex numberReg = new Regex("[0-9]");
        public static Regex letterReg = new Regex("[a-zA-Z]");

        private static int GetActions()
        {
            int action = -1;
            bool isParsed = false;

            do
            {
                Console.WriteLine("\nВыберите действие:\n");
                string[] actions = new string[]
                {
                    "Закрыть программу",
                    "Вывести список товаров",
                    "Вывести товары по категориям",
                    "Посмотреть три самых дорогих товара",
                    "Поиск товаров",
                    "Добавить товар",
                    "Изменить товар",
                    "Изменить количество товара",
                    "Удалить товар",
                    "Добавить категорию",
                    "Изменить категорию",
                    "Удалить категорию",
                    "Переключить отображение ошибок ошибки"
                };

                Console.WriteLine("\tТовары:");

                for (int i = 1; i < 9; i++)
                {
                    Console.WriteLine($"\t\t{i} 🠆 {actions[i]}");
                }

                GenerateError("Категории:");

                for (int i = 9; i < 12; i++)
                {
                    Console.WriteLine($"\t\t{i} 🠆 {actions[i]}");
                }

                GenerateError("Другое:");
                Console.WriteLine($"\t\t{0} 🠆 {actions[0]}");
                Console.WriteLine($"\t\t{12} 🠆 {actions[12]}\n");

                Console.Write("Action: ");
                isParsed = int.TryParse(Console.ReadLine(), out action);
                Console.WriteLine("");

                if (!isParsed) GenerateError("❌ - не удалось распознать действие.");

            } while (!isParsed);
            return action;
        }

        private static void ExecuteAction(int action)
        {
            switch (action)
            {
                case 0:
                    {
                        exit = true;
                        break;
                    }
                case 1:
                    {
                        try
                        {
                            List<Product> products = Product.GetList();
                            if (products.Count == 0) { GenerateError("❌ - список продуктов пуст."); break; }
                            else DisplayProductList(products, "advanced");
                        }
                        catch
                        {
                            GenerateError("❌ - Произошла ошибка обновления данных.");
                        }
                        break;
                    }
                case 2:
                    {
                        try 
                        {
                        List<Category> categories = Category.GetList();
                        List<Product> products = Product.GetList();
                        bool success = false;
                        if (products.Count == 0) { GenerateError("❌ - список продуктов пуст."); break; }
                        do
                        {
                            DisplayCategoryList(categories);
                            Console.Write("\nВведите номер категории: ");
                            string input = Console.ReadLine();
                            int categoryId = 0;
                            bool isParsed = int.TryParse(input, out categoryId);
                            if (isParsed)
                            {
                                if (Category.CheckIfExists(categoryId))
                                {
                                    products = Product.GetByCategory(categoryId);
                                    if (products.Count == 0) { GenerateError("❌ - данная категория ещё пустует."); break; }
                                    DisplayProductList(products, "advanced");
                                }
                                success = true;
                            }

                        } while (!success);
                    }
                        catch
                        {
                        GenerateError("❌ - Произошла ошибка обновления данных.");
                    }
                    break;
            }
                case 3:
                    {
                        try
                        {
                        List<Product> products = Product.GetTopThreeMostExpensive();
                        if (products.Count == 0) { GenerateError("❌ - список продуктов пуст."); break; }
                        DisplayProductList(products, "advanced");
                        }
                        catch
                        {
                            GenerateError("❌ - Произошла ошибка обновления данных.");
                        }
                        break;
                    }
                case 4:
                    {
                        try
                        {
                        Console.Write("Введите название товара: ");
                        string input = Console.ReadLine();
                        if (!input.IsNullOrEmpty())
                        {
                            string searchString = input.TrimStart().TrimEnd();
                            List<Product> products = Product.Search(searchString);
                            if (products.Count == 0) { GenerateError("❌ - список продуктов пуст."); break; }
                            DisplayProductList(products, "advanced");
                        }
                        }
                        catch
                        {
                            GenerateError("❌ - Произошла ошибка обновления данных.");
                        }
                        break;
                    }
                case 5:
                    {
                        try
                        {
                        List<Category> categories = Category.GetList();

                        if (categories.Count() > 0)
                        {
                            string name = string.Empty, description, image;
                            int quantity = 0, category = 0;
                            long price = 0;
                            byte discount = 0;

                            string input;

                            do
                            {
                                Console.Write("\nВведите название товара: ");
                                input = Console.ReadLine();
                                if (!input.IsNullOrEmpty()) name = input.TrimStart().TrimEnd();
                                break;

                            } while (!name.IsNullOrEmpty());

                            Console.Write("Введите описание товара (Оставьте пустым если не нужно): ");
                            input = Console.ReadLine();

                            description = input.TrimStart().TrimEnd();

                            Console.Write("Введите изображение товара (Оставьте пустым если не нужно): ");
                            input = Console.ReadLine();

                            image = input.TrimStart().TrimEnd();

                            foreach (char c in image)
                            {
                                if (!numberReg.IsMatch(c.ToString()) && !letterReg.IsMatch(c.ToString()) && c != '.') image.Remove(c);
                            }

                            bool quantityParsed = false;
                            do
                            {
                                Console.Write("Введите кол-во товара: ");
                                input = Console.ReadLine();
                                quantityParsed = int.TryParse(input, out quantity);

                            } while (!quantityParsed && quantity > int.MaxValue);

                            bool priceParsed = false;
                            do
                            {
                                Console.Write("Введите цену товара: ");
                                input = Console.ReadLine();
                                priceParsed = long.TryParse(input, out price);

                            } while (!priceParsed && price > long.MaxValue);

                            bool discountParsed = false;
                            do
                            {
                                Console.Write("Введите скидку на товар: ");
                                input = Console.ReadLine();
                                discountParsed = byte.TryParse(input, out discount);

                            } while (!discountParsed && discount > 100);

                            DisplayCategoryList(categories);

                            bool categoryParsed = false;
                            do
                            {
                                Console.Write("Введите категорию товара: ");
                                input = Console.ReadLine();
                                categoryParsed = int.TryParse(input, out category);


                            } while (!categoryParsed && !Category.CheckIfExists(category));


                            Product.Create(name, description, image, quantity, price, discount, category);
                            break;
                        }
                        else GenerateError("❌ - необходимо создать минимум 1 категорию.");
                        }
                        catch
                        {
                            GenerateError("❌ - Произошла ошибка обновления данных.");
                        }
                        break;
                    }
                case 6:
                    {
                        try
                        {
                        List<Product> products = Product.GetList();
                        List<Category> categories = Category.GetList();
                        if (products.Count() > 0)
                        {
                            Console.WriteLine("Какой товар вы желаете изменить?");

                            int prod;
                            string input;
                            bool parsed = false;

                            do
                            {
                                DisplayProductList(products, "simple");
                                Console.Write("Номер товара для изменения: ");
                                input = Console.ReadLine();
                                parsed = int.TryParse(input, out prod);

                            } while (!parsed && !Product.CheckIfExists(prod));


                            string name = string.Empty, description, image;
                            int quantity = 0, category = 0;
                            long price = 0;
                            byte discount = 0;

                            do
                            {
                                Console.Write("\nВведите название товара: ");
                                input = Console.ReadLine();
                                if (!input.IsNullOrEmpty()) name = input.TrimStart().TrimEnd();
                                break;

                            } while (!name.IsNullOrEmpty());

                            Console.Write("Введите описание товара (Оставьте пустым если не нужно): ");
                            input = Console.ReadLine();

                            description = input.TrimStart().TrimEnd();

                            Console.Write("Введите изображение товара (Оставьте пустым если не нужно): ");
                            input = Console.ReadLine();

                            image = input.TrimStart().TrimEnd();

                            foreach (char c in image)
                            {
                                if (numberReg.IsMatch(c.ToString()) || letterReg.IsMatch(c.ToString()) || c != '.') image.Remove(c);
                            }

                            bool quantityParsed = false;
                            do
                            {
                                Console.Write("Введите кол-во товара: ");
                                input = Console.ReadLine();

                                quantityParsed = int.TryParse(input, out quantity);

                            } while (!quantityParsed && quantity > int.MaxValue);

                            bool priceParsed = false;
                            do
                            {
                                Console.Write("Введите цену товара: ");
                                input = Console.ReadLine();
                                priceParsed = long.TryParse(input, out price);

                            } while (!priceParsed && price > long.MaxValue);

                            bool discountParsed = false;
                            do
                            {
                                Console.Write("Введите скидку на товар: ");
                                input = Console.ReadLine();
                                discountParsed = byte.TryParse(input, out discount);

                            } while (!discountParsed && discount > 100);

                            DisplayCategoryList(categories);

                            bool categoryParsed = false;
                            do
                            {
                                Console.Write("Введите категорию товара: ");
                                input = Console.ReadLine();
                                categoryParsed = int.TryParse(input, out category);

                            } while (!categoryParsed && !Category.CheckIfExists(category));

                            Product.Update(prod, name, description, image, quantity, price, discount, category);
                        }
                        else GenerateError("❌ - Нечего обновлять");
                        }
                        catch
                        {
                            GenerateError("❌ - Произошла ошибка обновления данных.");
                        }
                        break;
                    }
                case 7:
                    {
                        try
                        {
                        List<Product> products = Product.GetList();
                        List<Category> categories = Category.GetList();
                        if (products.Count() > 0)
                        {
                            Console.WriteLine("Какой товар вы желаете изменить?");

                            int prod;
                            string input;
                            bool parsed = false;
                            int quantity = 0;

                            do
                            {
                                DisplayProductList(products, "simple");
                                Console.Write("Номер товара для изменения количества: ");
                                input = Console.ReadLine();
                                parsed = int.TryParse(input, out prod);

                            } while (!parsed && !Product.CheckIfExists(prod));

                            bool quantityParsed = false;
                            do
                            {
                                Console.Write("Введите кол-во товара: ");
                                input = Console.ReadLine();

                                quantityParsed = int.TryParse(input, out quantity);

                            } while (!quantityParsed && quantity > int.MaxValue);


                            Product.Update(prod, quantity);
                            break;
                        }
                        else GenerateError("❌ - Нечего обновлять");
                        }
                        catch
                        {
                            GenerateError("❌ - Произошла ошибка обновления данных.");
                        }
                        break;
                    }
                case 8:
                    {
                        try
                        {
                        List<Product> products = Product.GetList();
                        if (products.Count() > 0)
                        {
                            int prod;
                            string input;
                            bool parsed = false;

                            do
                            {
                                DisplayProductList(products, "simple");
                                Console.Write("Номер товара для удаления количества: ");
                                input = Console.ReadLine();
                                parsed = int.TryParse(input, out prod);

                            } while (!parsed && !Product.CheckIfExists(prod));

                            Product.Delete(prod);
                            break;
                        }
                        else GenerateError("❌ - Нечего удалять");
                        }
                        catch
                        {
                            GenerateError("❌ - Произошла ошибка обновления данных.");
                        }
                        break;
                    }
                case 9:
                    {
                        try 
                        {
                        string name = null;
                        string input;

                        do
                        {
                            Console.Write("\nВведите название категории: ");
                            input = Console.ReadLine();
                            if (!input.IsNullOrEmpty()) name = input.TrimStart().TrimEnd();
                            break;

                        } while (!name.IsNullOrEmpty());

                        Category.Create(name);
                        }
                        catch
                        {
                            GenerateError("❌ - Произошла ошибка обновления данных.");
                        }
                        break;
                    }
                case 10:
                    {
                        try
                        {
                        List<Category> categories = Category.GetList();
                        if (categories.Count() > 0)
                        {
                            int categ;
                            string input, name = null;
                            bool parsed = false;

                            do
                            {
                                DisplayCategoryList(categories);
                                Console.Write("Номер категории для изменения названия: ");
                                input = Console.ReadLine();
                                parsed = int.TryParse(input, out categ);

                            } while (!parsed && !Category.CheckIfExists(categ));

                            do
                            {
                                Console.Write("\nВведите название категории: ");
                                input = Console.ReadLine();
                                if (!input.IsNullOrEmpty()) name = input.TrimStart().TrimEnd();
                                break;

                            } while (!name.IsNullOrEmpty());


                            Category.Update(categ, name);
                            break;

                        }
                        else GenerateError("❌ - Нечего обновлять");
                        }
                        catch
                        {
                            GenerateError("❌ - Произошла ошибка обновления данных.");
                        }
                        break;
                    }
                case 11:
                    {
                        try
                        {
                        List<Category> categories = Category.GetList();
                        if (categories.Count() > 0)
                        {

                            int categ;
                            string input, name = null;
                            bool parsed = false;

                            do
                            {
                                DisplayCategoryList(categories);
                                Console.Write("Номер категории для удаления: ");
                                input = Console.ReadLine();
                                parsed = int.TryParse(input, out categ);

                            } while (!parsed && !Category.CheckIfExists(categ));

                            Category.Delete(categ);
                            break;

                        }
                        else GenerateError("❌ - Нечего удалять");
                        }
                        catch
                        {
                            GenerateError("❌ - Произошла ошибка обновления данных.");
                        }
                        break;
                    }
                case 12:
                    {
                        if (!showError)
                        {
                            showError = true;
                            GenerateError("⚠ - отображение ошибок включено");
                            Product.ChangeErrorDisplayStatus(showError);
                            Category.ChangeErrorDisplayStatus(showError);
                        }
                        else
                        {
                            GenerateError("⚠ - отображение ошибок выключено");
                            showError = false;
                            Product.ChangeErrorDisplayStatus(showError);
                            Category.ChangeErrorDisplayStatus(showError);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Метод вывода списка продуктов
        /// </summary>
        /// <param name="products">Список продуктов</param>
        /// <param name="DisplayType">Тип вывода:\nsimple - простой вывод\nadvanced - полный вывод.</param>
        private static void DisplayProductList(List<Product> products, string DisplayType)
        {
            if (DisplayType == "simple")
            {
                foreach (Product product in products)
                {
                    Console.WriteLine("+---------------------------------------+");
                    Console.WriteLine($"{product.ProductId} - {product.ProductName}");
                    Console.WriteLine("+---------------------------------------+");
                }
            }
            else if (DisplayType == "advanced")
            {
                foreach (Product product in products)
                {
                    Console.WriteLine("+---------------------------------------+");
                    Console.WriteLine($"Идентификатор: {product.ProductId}");
                    Console.WriteLine($"Название: {product.ProductName}");
                    Console.WriteLine($"Описание: {product.ProductDescription}");
                    Console.WriteLine($"Картинка: {product.ProductImagePath}");
                    Console.Write("Количество: ");
                    if (product.ProductQuantity == 0 ) Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{product.ProductQuantity}");
                    Console.ResetColor();
                    Console.WriteLine($"Цена: {product.ProductPrice}");
                    Console.WriteLine($"Скидка: {product.ProductDiscount}%");
                    Console.WriteLine("+---------------------------------------+");
                }
            }
        }

        private static void DisplayCategoryList(List<Category> categories)
        {
            foreach (Category category in categories)
            {
                Console.WriteLine("+---------------------------------------+");
                Console.WriteLine($"\t{category.CategoryId} - {category.CategoryName}");
                Console.WriteLine("+---------------------------------------+");
            }
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

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            int action = 0;

            do
            {
                action = GetActions();
                ExecuteAction(action);

            } while (!exit);

            Environment.Exit(0);
        }
    }
}
