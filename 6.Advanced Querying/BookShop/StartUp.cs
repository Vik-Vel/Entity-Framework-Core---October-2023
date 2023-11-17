using System.Globalization;
using System.Text;
using BookShop.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookShop
{
    using BookShop.Models;
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            //string input = Console.ReadLine();

            //2.	Age Restriction
            //Console.WriteLine(GetBooksByAgeRestriction(db, input));

            //3.	Golden Books
            //Console.WriteLine(GetGoldenBooks(db));

            //4.	Books by Price
            //Console.WriteLine(GetBooksByPrice(db));

            //5.	Not Released In
            //string input = Console.ReadLine();
            //int inputIn = Int32.Parse(input);
            //Console.WriteLine(GetBooksNotReleasedIn(db, inputIn));

            //6.	Book Titles by Category
            //string input = Console.ReadLine();
            //Console.WriteLine(GetBooksByCategory(db, input));

            //7.	Released Before Date
            //string date = Console.ReadLine();
            //Console.WriteLine(GetBooksReleasedBefore(db, date));

            //8.	Author Search
            //string input = Console.ReadLine();
            //Console.WriteLine(GetAuthorNamesEndingIn(db, input));

            //9.	Book Search
            //string input = Console.ReadLine();
            //Console.WriteLine(GetBookTitlesContaining(db, input));

            //10.	Book Search by Author
            //string input = Console.ReadLine();
            //Console.WriteLine(GetBooksByAuthor(db, input));

            //11.	Count Books
            //int input = int.Parse(Console.ReadLine());
            //Console.WriteLine(CountBooks(db, input));

            // 12.Total Book Copies
            //Console.WriteLine(CountCopiesByAuthor(db));

            //13.	Profit by Category
            //Console.WriteLine(GetTotalProfitByCategory(db));

            // 14.Most Recent Books
            // Console.WriteLine(GetMostRecentBooks(db));

            //15.	Increase Prices
            Console.WriteLine(GetMostRecentBooks(db));

        }


        //2.	Age Restriction
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            //command should be either Teen, Minor or Adult
            if (!Enum.TryParse<AgeRestriction>(command, true, out var ageRestriction))
            {
                return $"{command} is not valid are restriction";
            }

            var books = context.Books
                .Where(b => b.AgeRestriction == ageRestriction)
                .Select(b => new
                {
                    b.Title
                })
                .OrderBy(b => b.Title)
                .ToList();

            return string.Join(Environment.NewLine, books.Select(b => b.Title));

        }

        //3.	Golden Books
        public static string GetGoldenBooks(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
                .ToList();

            return string.Join(Environment.NewLine, books.Select(b => b.Title));

        }

        //4.	Books by Price
        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.Price > 40)
                .Select(b => new
                {
                    b.Title,
                    b.Price
                })
                .OrderByDescending(b => b.Price)
                .ToList();


            return string.Join(Environment.NewLine, books.Select(b => $"{b.Title} - ${b.Price:f2}"));


        }

        //5.	Not Released In
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                .Select(b => new
                {
                    b.Title,
                    b.ReleaseDate
                })
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year != year)
                .ToList();

            return string.Join(Environment.NewLine, books.Select(b => b.Title));
        }

        //6.	Book Titles by Category

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categories = input.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLower()).ToArray();

            var books = context.Books
                .Select(b => new
                {
                    b.Title,
                    b.BookCategories
                })
                .Where(b => b.BookCategories.Any(bc => categories.Contains(bc.Category.Name.ToLower())))
                .OrderBy(b => b.Title)
                .ToList();


            return string.Join(Environment.NewLine, books.Select(b => b.Title));
        }

        //7.	Released Before Date
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var parsedDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);


            var books = context.Books
                .Where(b => b.ReleaseDate < parsedDate)
                .Select(b => new
                {
                    b.Title,
                    b.EditionType,
                    b.Price,
                    b.ReleaseDate
                })
                .OrderByDescending(b => b.ReleaseDate);

            return string.Join(Environment.NewLine, books.Select(b => $"{b.Title} - {b.EditionType} - ${b.Price:f2}"));
        }

        //8.	Author Search
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a=>new
                {
                    FullName = a.FirstName + " " + a.LastName
                })
                .OrderBy(n => n.FullName);


            return string.Join(Environment.NewLine, authors.Select(a => a.FullName));

        }

        //9.	Book Search
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            //var books = context.Books
            //    .Where(b => b.Title.ToLower().Contains(input))
            //    .Select(b => new
            //    {
            //       FullName =  b.Title

            //    })
            //    .OrderBy(b => b.FullName);


            //return string.Join(Environment.NewLine, books.Select(b => b.FullName));


            // Option 2
            var books = context.Books
                .Where(b => EF.Functions.Like(b.Title, $"%{input}%"))
                .Select(b => new
                {
                    b.Title
                })
                .OrderBy(b => b.Title);

            return string.Join(Environment.NewLine, books.Select(b => b.Title));
        }

        //10.	Book Search by Author

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            //var books = context.Books
            //    .Where(a => a.Author.LastName.StartsWith(input.ToLower()))
            //    .Select(a => new
            //    {
            //        TitleAndAuthor = $"{a.Title} ({a.Author.FirstName} {a.Author.LastName})",
            //        a.BookId
            //    })
            //    .OrderBy(a => a.BookId);

            //return string.Join(Environment.NewLine, books.Select(b => b.TitleAndAuthor));

            var books = context.Books
                .Where(b => EF.Functions.Like(b.Author.LastName, $"%{input}%"))
                .Select(b => new
                {
                    BookTitle = b.Title,
                    AuthorName = b.Author.FirstName + " " + b.Author.LastName
                });

            return string.Join(Environment.NewLine,
                books.Select(b => $"{b.BookTitle} ({b.AuthorName})"));



        }

        //11.	Count Books
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var books = context.Books
                .Where(b => b.Title.Length > lengthCheck)
                .ToList();

            int countBooks = books.Count;

            return countBooks;

            //Option 2
            // return context.Books.Count(b => b.Title.Length > lengthCheck);
        }

        //12.	Total Book Copies
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context.Authors
                .Select(a => new
                {
                    // AuhtorName = string.Join(" ", a.FirstName, a.LastName),
                    AuthorName = $"{a.FirstName} {a.LastName}",
                    Copies = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(a => a.Copies)
                .ToList();

            return string.Join(Environment.NewLine, authors.Select(a => $"{a.AuthorName} - {a.Copies}"));
        }

        //13.	Profit by Category
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var profits = context.Categories
                .Select(p => new
                {
                    Profit = p.CategoryBooks
                        .Sum(p => p.Book.Copies * p.Book.Price),
                    CategoryName = p.Name
                })
                .OrderByDescending(p => p.Profit)
                .ThenBy(c => c.CategoryName);

            //foreach (var prof in profits)
            //{
            //    Console.WriteLine(prof.CategoryName);
            //}

            return string.Join(Environment.NewLine, profits.Select(p => $"{p.CategoryName} ${p.Profit:f2}"));
        }

        //14.	Most Recent Books
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categories = context.Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    MostRecentBooks = c.CategoryBooks.OrderByDescending(bc => bc.Book.ReleaseDate)
                        .Take(3)
                        .Select(cb => new
                        {
                            BookTitle = cb.Book.Title,
                            cb.Book.ReleaseDate.Value.Year
                        })
                })
                .OrderBy(c => c.CategoryName);

            StringBuilder sb = new StringBuilder();

            foreach (var cat in categories)
            {
                sb.AppendLine($"--{cat.CategoryName}");
                foreach (var most in cat.MostRecentBooks)
                {
                    sb.AppendLine($"{most.BookTitle} ({most.Year})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //15.	Increase Prices
        public static void IncreasePrices(BookShopContext context)
        {

            var books = context.Books
                .Where(b => b.ReleaseDate!.Value.Year < 2010);

            foreach (var book in books)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        //16.	Remove Books
        public static int RemoveBooks(BookShopContext context)
        {
            context.ChangeTracker.Clear();

            var books = context.Books
                .Where(b => b.Copies < 4200);

            context.RemoveRange(books);

            return context.SaveChanges();
        }

    }
}


