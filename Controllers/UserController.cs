using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using hsapp.Models;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.Sqlite;

namespace hsapp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        //Dummy data for testing
        private List<User> _users = new List<User> {
            new User {
                ID = 1,
                Name = "alice",
                Contacts = new List<Contact>(),
            },
            new User {
                ID = 2,
                Name = "bob",
                Contacts = new List<Contact>(),
            },
            new User {
                ID = 3,
                Name = "carol",
                Contacts = new List<Contact>(),
            },
        };

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        //Not working - routing?
        [HttpGet]
        [Route("")]
        [Route("search/{search=search}")]
        public IEnumerable<User> Get(string search=null)
        {
            //Console.WriteLine($"in search:{search}"); //DEBUG
            //Should be case insensitive, sorted
            //Need to add paging, and/or don't return results if there are too many
            return _users.Where(u => search == null || u.Name.Contains(search)); //Replace with GetUsers
        }
        
        [HttpGet("{id}")]
        public ActionResult<User> Get(int id)
        {
            Console.WriteLine($"in id:{id}");
            //Contact should be reference?
            var user = _users.FirstOrDefault(u => u.ID == id); //Replace with GetUsers with ID
            if(user == null)
            {
                return NotFound();
            }
            return user;
        }

        [HttpPost]
        [Route("")]
        public ActionResult<User> PostUser(User user)
        {
            var ID = _users.Any() ? _users.Max(u => u.ID) + 1 : 1;
            user.ID = ID;
            _users.Add(user); //Replace with AddUser below
            
            return CreatedAtAction("User", new { id = user.ID }, user);
        }

        private List<User> GetUsers(string search)
        {
            var result = new List<User>();

            using (var connection = new SqliteConnection("" +
                new SqliteConnectionStringBuilder
                {
                    DataSource = "hsapp.db"
                }))
            {
                connection.Open();

                var selectCommand = connection.CreateCommand();
                selectCommand.CommandText = "SELECT ID, Name FROM Users";
                using (var reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new User {
                            ID = reader.GetInt32(0),
                            Name = reader.GetString(1),
                        });
                    }
                }
            }

            return result;
        }

        private int AddUser(User user)
        {
            int ID;

            using (var connection = new SqliteConnection("" +
                new SqliteConnectionStringBuilder
                {
                    DataSource = "hsapp.db"
                }))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var insertCommand = connection.CreateCommand();
                    insertCommand.Transaction = transaction;
                    insertCommand.CommandText = "INSERT INTO User ( Name ) VALUES ( $name ); SELECT SCOPE_IDENTITY();";
                    insertCommand.Parameters.AddWithValue("$name", user.Name);

                    ID = Convert.ToInt32(insertCommand.ExecuteScalar());

                    transaction.Commit();
                }
            }

            return ID;
        }
    }
}
