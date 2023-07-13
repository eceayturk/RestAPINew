using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.SQLite;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using System.Data;



namespace RestAPI
{

    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly SQLiteConnection _connection;

        public UserController()
        {
            _connection = new SQLiteConnection("Data Source=users.db;Version=3;");
            _connection.Open();
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            var command = new SQLiteCommand(
                "CREATE TABLE IF NOT EXISTS Users (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, Surname TEXT, Email TEXT)",
                _connection);
            command.ExecuteNonQuery();
        }

        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            var command = new SQLiteCommand("SELECT * FROM Users", _connection);
            var reader = command.ExecuteReader();

            var users = new List<User>();
            while (reader.Read())
            {
                var user = new User
                {
                    Id = (int)(long)reader["Id"],
                    Name = (string)reader["Name"],
                    Surname = (string)reader["Surname"],
                    Email = (string)reader["Email"]
                };
                users.Add(user);
            }

            return Ok(users);
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetUser(int id)
        {
            var command = new SQLiteCommand("SELECT * FROM Users WHERE Id = @Id", _connection);
            command.Parameters.AddWithValue("@Id", id);

            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var user = new User
                {
                    Id = (int)(long)reader["Id"],
                    Name = (string)reader["Name"],
                    Surname = (string)reader["Surname"],
                    Email = (string)reader["Email"]
                };

                return Ok(user);
            }

            return NotFound();
        }

        [HttpPost]

        public ActionResult<User> AddUser([FromBody] User user)
        {
            var command = new SQLiteCommand("INSERT INTO Users (Name, Surname, Email) VALUES (@Name, @Surname, @Email)", _connection);
            command.Parameters.AddWithValue("@Name", user.Name);
            command.Parameters.AddWithValue("@Surname", user.Surname);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.ExecuteNonQuery();

            user.Id = (int)_connection.LastInsertRowId;
            return Ok(user);
        }

        [HttpPut("{id}")]
        public ActionResult<User> UpdateUser(int id, [FromBody] User updatedUser)
        {
            var command = new SQLiteCommand("UPDATE Users SET Name = @Name, Surname = @Surname, Email = @Email WHERE Id = @Id", _connection);
            command.Parameters.AddWithValue("@Name", updatedUser.Name);
            command.Parameters.AddWithValue("@Surname", updatedUser.Surname);
            command.Parameters.AddWithValue("@Email", updatedUser.Email);
            command.Parameters.AddWithValue("@Id", id);
            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                updatedUser.Id = id;
                return Ok(updatedUser);
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)

        {
            var command = new SQLiteCommand("DELETE FROM Users WHERE Id = @Id", _connection);
            command.Parameters.AddWithValue("@Id", id);
            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {

                var rest = new SQLiteCommand("SELECT * FROM Users", _connection);
                var reader = rest.ExecuteReader();

                var users = new List<User>();
                while (reader.Read())
                {
                    var user = new User
                    {
                        Id = (int)(long)reader["Id"],
                        Name = (string)reader["Name"],
                        Surname = (string)reader["Surname"],
                        Email = (string)reader["Email"]
                    };
                    users.Add(user);
                }

                return Ok(users);
                //return Ok();
            }

            return NotFound();
        }
    }
}

