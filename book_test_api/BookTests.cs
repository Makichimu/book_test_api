using Newtonsoft.Json;
using System.Text;
using System.Net;

namespace book_test_api
{
    class Book
    {
        public string author { get; set; }
        public int id { get; set; }
        public bool isElectronicBook { get; set; }
        public string name { get; set; }
        public int year { get; set; }
    }

    [TestClass]
    public class BookTests
    {
        private readonly HttpClient client = new HttpClient();

        [TestMethod]
        public async Task GetAllBooks_ReturnsSuccessStatusCode()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, $"http://localhost:5000/api/books");
            // Act
            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            var books = JsonConvert.DeserializeObject(responseContent);
            Console.WriteLine(JsonConvert.SerializeObject(books, Formatting.Indented));

            // Assert
            Console.WriteLine(response.StatusCode);
            response.EnsureSuccessStatusCode();
            
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task GetBookById_ValidId_ReturnsSuccessStatusCode()
        {
            for (int i = 3; i <= 74; i++)
            {
                // Arrange
                int id = i;
                var request = new HttpRequestMessage(HttpMethod.Get, $"http://localhost:5000/api/books/{id}");

                // Act
                var response = await client.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();
                var books = JsonConvert.DeserializeObject(responseContent);
                Console.WriteLine(JsonConvert.SerializeObject(books, Formatting.Indented));

                // Assert
                Console.WriteLine(response.StatusCode);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task GetBookById_InvalidId_ReturnsNotFoundStatusCode()
        {
            // Define an array of invalid IDs
            int[] invalidIds = { -1, 0, -100, -99, -101, 1000000000 };

            // Loop through the array of invalid IDs
            foreach (int id in invalidIds)
            {
                // Arrange
                var request = new HttpRequestMessage(HttpMethod.Get, $"http://localhost:5000/api/books/{id}");

                // Act
                var response = await client.SendAsync(request);


                // Assert
                // Check that all invalid IDs return the expected status code
                Console.WriteLine(response.StatusCode);
                Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            }

            
        }

        [TestMethod]
        public async Task AddBook_ValidBook_ReturnsCreatedStatusCode()
        {
            // Arrange
            var book = new Book
            {
                author = "Test Author",
                name = "Test Book",
                year = 2022,
                isElectronicBook = false
            };
            var content = new StringContent(JsonConvert.SerializeObject(book), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5000/api/books")
            {
                Content = content
            };

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Console.WriteLine(response.StatusCode);
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        [TestMethod]
        public async Task AddBook_ValidOnlyNameBook_ReturnsCreatedStatusCode()
        {
            // Arrange
            var book = new Book
            {
                name = "Test Book"


            };
            var content = new StringContent(JsonConvert.SerializeObject(book), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5000/api/books")
            {
                Content = content
            };

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Console.WriteLine(response.StatusCode);
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        [TestMethod]
        public async Task AddBook_InvalidNoNameBook_ReturnsBadRequestStatusCode()
        {
            // Arrange
            var book = new Book
            {
                author = "Test Author",
                year = 2022,
                isElectronicBook = false

            };
            var content = new StringContent(JsonConvert.SerializeObject(book), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5000/api/books")
            {
                Content = content
            };

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Console.WriteLine(response.StatusCode);
            Console.WriteLine("Book have no name!");
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task AddBook_InvalidNameBook_ReturnsBadRequestStatusCode()
        {
            // Arrange
            var book = new Book
            {
                name = "",
                author = "",
                year = 0,
                isElectronicBook = false

            };
            var content = new StringContent(JsonConvert.SerializeObject(book), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5000/api/books")
            {
                Content = content
            };

            // Act
            var response = await client.SendAsync(request);

            // Assert

            object addedbook = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
            Console.WriteLine("Response: " + addedbook);
            Console.WriteLine(response.StatusCode);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task UpdateBook_ValidIdAndBook_ReturnsOKStatusCode()
        {
            // Arrange
            int id = 3;
            var book = new Book
            {
                author = "Updated Author",
                name = "Updated Book",
                year = 2022,
                isElectronicBook = true
            };
            var content = new StringContent(JsonConvert.SerializeObject(book), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, $"http://localhost:5000/api/books/{id}")
            {
                Content = content
            };
            // Act
            var response = await client.SendAsync(request);

            // Assert
            Console.WriteLine(response.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }


        [TestMethod]
        public async Task UpdateBook_InvalidBook_ReturnsNotFoundStatusCode()
        {
            // Arrange
            int id = -1;
            var book = new Book
            {
                author = "",
                name = "",
                year = 0,
                isElectronicBook = false
            };
            var content = new StringContent(JsonConvert.SerializeObject(book), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, $"http://localhost:5000/api/books/{id}")
            {
                Content = content
            };

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Console.WriteLine(response.StatusCode);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task DeleteBook_ValidId_ReturnsOKStatusCode()
        {
            // Arrange
            int id = 1;
            var request = new HttpRequestMessage(HttpMethod.Delete, $"http://localhost:5000/api/books/{id}");

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Console.WriteLine(response.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task DeleteBook_ValidId_ReturnsNotFoundStatusCode()
        {
            // Arrange
            int id = 1;
            var request = new HttpRequestMessage(HttpMethod.Delete, $"http://localhost:5000/api/books/{id}");

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Console.WriteLine(response.StatusCode);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task DeleteBook_InvalidId_ReturnsNotFoundStatusCode()
        {
            // Arrange
            int id = -1;
            var request = new HttpRequestMessage(HttpMethod.Delete, $"http://localhost:5000/api/books/{id}");

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Console.WriteLine(response.StatusCode);
            Console.WriteLine("Book not found!");
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
        
    }

}