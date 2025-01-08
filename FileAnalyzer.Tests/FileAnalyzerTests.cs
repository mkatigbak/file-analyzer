/* FILE ANALYZER - MARK KATIGBAK */

namespace FileAnalyzer.Tests
{
    // THIS CLASS CONTAINS TESTS FOR THE FILEANALYZER FUNCTIONALITY
    public class FileAnalyzerTests : IDisposable
    {
        // PATH FOR THE TEST FILE THAT WILL BE CREATED AND USED IN TESTS
        private const string TestFilePath = "TestSales.txt";

        // TEST TO CHECK IF READING SALES DATA FROM A VALID FILE WORKS CORRECTLY
        [Fact]
        public void ReadSalesData_ValidFile_ReturnsSalesList()
        {
            // ARRANGE: SET UP THE TEST DATA BY WRITING TO THE TEST FILE
            File.WriteAllLines(TestFilePath, new[]
            {
                "Product1, 01/01/2022, 100.00", // SAMPLE SALES DATA
                "Product2, 01/02/2022, 200.00"  // SAMPLE SALES DATA
            });

            // ACT: CALL THE METHOD TO READ SALES DATA FROM THE FILE
            var sales = FileAnalyzer.ReadSalesData(TestFilePath);

            // ASSERT: VERIFY THAT THE DATA READ MATCHES THE EXPECTED VALUES
            Assert.Equal(2, sales.Count); // CHECK IF TWO SALES RECORDS WERE READ
            Assert.Equal("Product1", sales[0].ProductName); // CHECK THE FIRST PRODUCT NAME
            Assert.Equal(new DateTime(2022, 1, 1), sales[0].DateOfSale); // CHECK THE SALE DATE
            Assert.Equal(100.00m, sales[0].SalesAmount); // CHECK THE SALES AMOUNT
        }

        // TEST TO CHECK IF FILTERING SALES DATA WORKS CORRECTLY
        [Fact]
        public void FilterSalesData_ValidSearchStrings_ReturnsFilteredSales()
        {
            // ARRANGE: CREATE A LIST OF SALES TO FILTER
            var sales = new List<Sale>
            {
                new Sale("Product1", DateTime.Now, 100), // SAMPLE SALE
                new Sale("Product2", DateTime.Now, 200)  // SAMPLE SALE
            };
            var searchStrings = new[] { "Product1" }; // SEARCH FOR PRODUCT1

            // ACT: CALL THE METHOD TO FILTER SALES BASED ON SEARCH STRINGS
            var filteredSales = FileAnalyzer.FilterSalesData(sales, searchStrings);

            // ASSERT: VERIFY THAT THE FILTERED RESULTS ARE CORRECT
            Assert.Single(filteredSales); // CHECK THAT ONLY ONE SALE IS RETURNED
            Assert.Equal("Product1", filteredSales[0].ProductName); // CHECK THE PRODUCT NAME
        }

        // TEST TO CHECK IF WRITING SALES DATA TO A FILE WORKS CORRECTLY
        [Fact]
        public void WriteSalesData_ValidData_WritesToFile()
        {
            // ARRANGE: CREATE A LIST OF SALES TO WRITE TO THE FILE
            var sales = new List<Sale>
            {
                new Sale("Product1", DateTime.Now, 100), // SAMPLE SALE
                new Sale("Product2", DateTime.Now, 200)  // SAMPLE SALE
            };
            var searchStrings = new[] { "Product1", "Product2" }; // PRODUCTS TO FILTER

            // ACT: CALL THE METHOD TO WRITE SALES DATA TO THE FILE
            FileAnalyzer.WriteSalesData(TestFilePath, sales, searchStrings);

            // ASSERT: VERIFY THAT THE CORRECT DATA WAS WRITTEN TO THE FILE
            var lines = File.ReadAllLines(TestFilePath); // READ ALL LINES FROM THE FILE
            Assert.Contains("Filtered product(s): Product1, Product2", lines[0]); // CHECK HEADER LINE
            Assert.Contains($"Product1, {DateTime.Now:MM/dd/yyyy}, {100.00:N2}", lines); // CHECK PRODUCT1 LINE
            Assert.Contains($"Product2, {DateTime.Now:MM/dd/yyyy}, {200.00:N2}", lines); // CHECK PRODUCT2 LINE
        }

        // CLEAN UP TEST FILE AFTER TESTS ARE DONE
        public void Dispose()
        {
            // CHECK IF THE TEST FILE EXISTS AND DELETE IT
            if (File.Exists(TestFilePath))
            {
                File.Delete(TestFilePath); // REMOVE THE TEST FILE
            }
        }
    }
}