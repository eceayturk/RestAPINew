using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Text;


namespace RestAPI_
{
    [TestFixture]
    public class UserControllerIntegrationTests
    {
        private TestServer server;
        private HttpClient client;

        [OneTimeSetUp] //runs once, does the preconfigurations.
        public void Setup()
        {

            server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            client = server.CreateClient();
            
    }

        [OneTimeTearDown] //releasing memory and resources after running all tests.
        public void TearDown()
        {
            server.Dispose();
            client.Dispose();
        }

        [Test]
        public async Task GetUsers()
        {
            // Act part of the code. Here we try to connect related part of the application.
            HttpResponseMessage response = await client.GetAsync("/api/users");
            string responseBody = await response.Content.ReadAsStringAsync(); 
             //after sending get request, API's answer as string.

            // Assert part of the code. 
            response.EnsureSuccessStatusCode(); // to be sure http request succeed or not.
            Assert.AreEqual("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
            // to verify content type is json or not.
            Assert.IsNotEmpty(responseBody); // to ensure response is not empty.

            List<User> users = JsonConvert.DeserializeObject<List<User>>(responseBody);
            // convert data from json format to c# object(List<User>).
            Assert.IsNotNull(users);
            //users variable is empty or not.
        }

        [Test]
        public async Task GetUserWithValidId()
        {
            // Arrange part of the code. Here we assign a id which must be in database. 
            int validId = 1;

            // Act part. We try to connect : api/users/1.
            HttpResponseMessage response = await client.GetAsync($"/api/users/{validId}");
            string responseBody = await response.Content.ReadAsStringAsync();

            // Assert part for verification.
            response.EnsureSuccessStatusCode();
            Assert.AreEqual("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
            Assert.IsNotEmpty(responseBody);

            User user = JsonConvert.DeserializeObject<User>(responseBody); 
            //convert user which has id number 1, to the c# object.  
            Assert.IsNotNull(user);
        }

        [Test]
        public async Task GetUserWithInvalidId()
        {
            // Arrange invalid id number -1.
            int invalidId = -1;

            // Act part.
            HttpResponseMessage response = await client.GetAsync($"/api/users/{invalidId}");

            // Assert part. Here -1 should be not found in database. 
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public async Task AddUser()
        {
            // Arrange new user to add database.
            User newUser = new User
            {
                Name = "Ayse",
                Surname = "Akin",
                Email = "ayseakin@outlook.com"
            };
            string jsonContent = JsonConvert.SerializeObject(newUser);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            //convert c# objest to the json.

            // Act part.
            HttpResponseMessage response = await client.PostAsync("/api/users", httpContent);
            string responseBody = await response.Content.ReadAsStringAsync();

            // Assert part.
            response.EnsureSuccessStatusCode();
            Assert.AreEqual("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
            Assert.IsNotEmpty(responseBody);

            User addedUser = JsonConvert.DeserializeObject<User>(responseBody);
            Assert.IsNotNull(addedUser); 
        }

        [Test]
        public async Task UpdateUserWithValidId()
        {
            // Arrange updated information about user 1.
            int validId = 1;
            User updatedUser = new User
            {
                Name = "Ece",
                Surname = "Aytürk",
                Email = "ece_ayturk@outlook.com"
            };
            string jsonContent = JsonConvert.SerializeObject(updatedUser);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act part for connect related user and update values--> httpContent.
            HttpResponseMessage response = await client.PutAsync($"/api/users/{validId}", httpContent);
            string responseBody = await response.Content.ReadAsStringAsync();
            //After sending the put request, it keeps the content returned from the api as a string.

            // Assert part.
            response.EnsureSuccessStatusCode();
            Assert.AreEqual("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
            Assert.IsNotEmpty(responseBody);

            User updatedUserResult = JsonConvert.DeserializeObject<User>(responseBody);
            Assert.IsNotNull(updatedUserResult);
        }

        [Test]
        public async Task UpdateUserWithInvalidId()
        {
            // Arrange infos for invalid user.
            int invalidId = -1;
            User updatedUser = new User
            {
                Name = "UpdatedName",
                Surname = "UpdatedSurname",
                Email = "updated.email@example.com"
            };
            string jsonContent = JsonConvert.SerializeObject(updatedUser);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act part. 
            HttpResponseMessage response = await client.PutAsync($"/api/users/{invalidId}", httpContent);

            // Assert part.
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public async Task DeleteUserWithValidId()
        {
            // Arrange id for user which we want to delete.
            int validId = 1;

            // Act part.
            HttpResponseMessage response = await client.DeleteAsync($"/api/users/{validId}");
            string responseBody = await response.Content.ReadAsStringAsync();

            // Assert part.
            response.EnsureSuccessStatusCode();
            Assert.AreEqual("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
            Assert.IsNotEmpty(responseBody);

            List<User> remainingUsers = JsonConvert.DeserializeObject<List<User>>(responseBody);
            Assert.IsNotNull(remainingUsers); 
        }

        [Test]
        public async Task DeleteUserWithInvalidId()
        {
            // Arrange invalid user id for deletion.
            int invalidId = -1;

            // Act part.
            HttpResponseMessage response = await client.DeleteAsync($"/api/users/{invalidId}");

            // Assert part.
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
