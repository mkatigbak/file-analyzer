/* FILE ANALYZER - MARK KATIGBAK */


using System.Globalization;

namespace FileAnalyzer
{
    // CLASS REPRESENTING A SALE WITH PRODUCT NAME, DATE, AND AMOUNT
    public class Sale
    {
        public string ProductName { get; set; }     // NAME OF THE PRODUCT SOLD
        public DateTime DateOfSale { get; set; }    // DATE WHEN THE SALE OCCURRED
        public decimal SalesAmount { get; set; }    // AMOUNT OF THE SALE

        // CONSTRUCTOR TO INITIALIZE A SALE OBJECT
        public Sale(string productName, DateTime dateOfSale, decimal salesAmount)
        {
            ProductName = productName;  // SET PRODUCT NAME
            DateOfSale = dateOfSale;    // SET DATE OF SALE
            SalesAmount = salesAmount;  // SET SALES AMOUNT
        }
    }

    // CLASS FOR ANALYZING SALES DATA
    public class FileAnalyzer
    {
        // METHOD TO READ SALES DATA FROM A FILE
        public static List<Sale> ReadSalesData(string filePath)
        {
            var sales = new List<Sale>();   // LIST TO STORE SALES DATA
            try
            {
                // READ ALL LINES FROM THE SPECIFIED FILE
                foreach (var line in File.ReadAllLines(filePath))
                {
                    var parts = line.Split(',');    // SPLIT THE LINE INTO PARTS
                    // CHECK IF THE LINE HAS EXACTLY 3 PARTS AND PARSE DATE AND AMOUNT
                    if (parts.Length == 3 &&
                        DateTime.TryParseExact(parts[1].Trim(), "MM/dd/yyyy",
                                               CultureInfo.InvariantCulture,
                                               DateTimeStyles.None,
                                               out var date) &&
                        decimal.TryParse(parts[2].Trim(), out var amount))
                    {
                        // CREATE A NEW SALE OBJECT AND ADD IT TO THE LIST
                        sales.Add(new Sale(parts[0].Trim(), date, amount));
                    }
                }
            }
            catch (Exception ex)
            {
                // THROW AN EXCEPTION IF THERE IS AN ERROR READING THE FILE
                throw new Exception($"Error reading the sales data file: {ex.Message}");
            }
            return sales;   // RETURN THE LIST OF SALES
        }

        // METHOD TO DISPLAY TOTAL SALES GROUPED BY PRODUCT
        public static void DisplayTotalSalesByProduct(IEnumerable<Sale> sales, string title)
        {
            Console.WriteLine($"\n{title}");    // PRINT THE TITLE
            // GROUP SALES BY PRODUCT NAME AND CALCULATE TOTAL SALES
            var grouped = sales.GroupBy(s => s.ProductName)
                               .Select(g => new { Product = g.Key, TotalSales = g.Sum(s => s.SalesAmount) })
                               .OrderByDescending(g => g.TotalSales);   // ORDER BY TOTAL SALES

            // PRINT EACH PRODUCT AND ITS TOTAL SALES
            foreach (var item in grouped)
            {
                Console.WriteLine($"{item.Product}: ${item.TotalSales:N2}");
            }
        }

        // METHOD TO DISPLAY TOTAL SALES GROUPED BY MONTH
        public static void DisplayTotalSalesByMonth(IEnumerable<Sale> sales, string title)
        {
            Console.WriteLine($"\n{title}");    // PRINT THE TITLE
            // GROUP SALES BY MONTH AND CALCULATE TOTAL SALES
            var grouped = sales.GroupBy(s => s.DateOfSale.ToString("MMMM"))
                               .Select(g => new { Month = g.Key, TotalSales = g.Sum(s => s.SalesAmount) })
                               .OrderByDescending(g => g.TotalSales);   // ORDER BY TOTAL SALES

            // PRINT EACH MONTH AND ITS TOTAL SALES
            foreach (var item in grouped)
            {
                Console.WriteLine($"{item.Month}: ${item.TotalSales:N2}");
            }
        }

        // METHOD TO FILTER SALES DATA BASED ON SEARCH STRINGS
        public static List<Sale> FilterSalesData(IEnumerable<Sale> sales, IEnumerable<string> searchStrings)
        {
            // RETURN SALES THAT MATCH ANY OF THE SEARCH STRINGS (CASE-INSENSITIVE)
            return sales.Where(s => searchStrings.Any(str =>
                                  s.ProductName.Equals(str.Trim(), StringComparison.OrdinalIgnoreCase)))
                        .ToList();
        }

        // METHOD TO WRITE FILTERED SALES DATA TO A FILE
        public static void WriteSalesData(string filePath, List<Sale> filteredSales, string[] searchStrings)
        {
            try
            {
                // CREATE A STREAMWRITER TO WRITE TO THE SPECIFIED FILE
                using (var writer = new StreamWriter(filePath))
                {
                    // WRITE THE FILTERED PRODUCTS HEADER
                    writer.WriteLine($"Filtered product(s): {string.Join(", ", searchStrings)}\n");

                    writer.WriteLine("Products Information:");  // WRITE SECTION HEADER
                    // WRITE EACH SALE'S INFORMATION TO THE FILE
                    foreach (var sale in filteredSales)
                    {
                        writer.WriteLine($"{sale.ProductName}, {sale.DateOfSale:MM/dd/yyyy}, {sale.SalesAmount:N2}");
                    }

                    writer.WriteLine("\nTotal sales by Filtered product:"); // WRITE SECTION HEADER
                    // GROUP AND WRITE TOTAL SALES BY PRODUCT
                    foreach (var group in filteredSales.GroupBy(s => s.ProductName)
                                                       .Select(g => new { Product = g.Key, TotalSales = g.Sum(s => s.SalesAmount) })
                                                       .OrderByDescending(g => g.TotalSales))
                    {
                        writer.WriteLine($"{group.Product}: ${group.TotalSales:N2}");
                    }

                    writer.WriteLine("\nTotal sales by Filtered product group by Month:");  // WRITE SECTION HEADER
                    // GROUP AND WRITE TOTAL SALES BY MONTH
                    foreach (var group in filteredSales.GroupBy(s => s.DateOfSale.ToString("MMMM"))
                                                       .Select(g => new { Month = g.Key, TotalSales = g.Sum(s => s.SalesAmount) })
                                                       .OrderByDescending(g => g.TotalSales))
                    {
                        writer.WriteLine($"{group.Month}: ${group.TotalSales:N2}");
                    }
                }
            }
            catch (Exception ex)
            {
                // THROW AN EXCEPTION IF THERE IS AN ERROR WRITING TO THE FILE
                throw new Exception($"Error writing the sales data file: {ex.Message}");
            }
        }
    }

    // MAIN PROGRAM CLASS
    internal class Program
    {
        // MAIN METHOD TO RUN THE PROGRAM
        static void Main(string[] args)
        {
            try
            {
                // PROMPT USER FOR INPUT FILE PATH
                Console.Write("Enter the input file path: ");
                var inputFilePath = Console.ReadLine();

                // PROMPT USER FOR OUTPUT FILE PATH
                Console.Write("Enter the output file path: ");
                var outputFilePath = Console.ReadLine();

                // READ SALES DATA FROM THE INPUT FILE
                var sales = FileAnalyzer.ReadSalesData(inputFilePath);

                // DISPLAY TOTAL SALES BY PRODUCT AND MONTH
                FileAnalyzer.DisplayTotalSalesByProduct(sales, "Total sales by Product:");
                FileAnalyzer.DisplayTotalSalesByMonth(sales, "Total sales by Month:");

                // PROMPT USER FOR SEARCH STRINGS
                Console.Write("\nEnter search strings separated by commas: ");
                var searchStrings = Console.ReadLine()?.Split(',');

                // CHECK IF ANY SEARCH STRINGS WERE ENTERED
                if (searchStrings == null || searchStrings.Length == 0)
                {
                    Console.WriteLine("No search strings entered. Exiting.");   // EXIT IF NONE ENTERED
                    return;
                }

                // FILTER SALES DATA BASED ON SEARCH STRINGS
                var filteredSales = FileAnalyzer.FilterSalesData(sales, searchStrings);

                // DISPLAY TOTAL SALES FOR FILTERED PRODUCTS
                FileAnalyzer.DisplayTotalSalesByProduct(filteredSales, "Total sales by Filtered product:");
                FileAnalyzer.DisplayTotalSalesByMonth(filteredSales, "Total sales by Filtered product group by Month:");

                // WRITE FILTERED SALES DATA TO THE OUTPUT FILE
                FileAnalyzer.WriteSalesData(outputFilePath, filteredSales, searchStrings);

                // INFORM USER THAT OUTPUT WAS SAVED SUCCESSFULLY
                Console.WriteLine($"\nThe output is successfully saved to {outputFilePath}.");
            }
            catch (Exception ex)
            {
                // PRINT ANY ERROR MESSAGES
                Console.WriteLine(ex.Message);
            }
        }
    }
}