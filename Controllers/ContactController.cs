using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using hsapp.Models;

namespace hsapp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly ILogger<ContactController> _logger;
        private List<Contact> _contacts = new List<Contact> {
                new Contact {
                    ID = 4,
                    UserID = 7,
                    Name = "m",
                    Address = "111 main st, hometown, usa",
                    Email = "m@example.com",
                    Phone = "503-555-1234"
                }
        };

        public ContactController(ILogger<ContactController> logger)
        {
            _logger = logger;
        }

        [HttpGet("User/{id}")]
        public IEnumerable<Contact> GetByUser(int id)
        {
            //Console.WriteLine($"in contact get with id:{id}"); //DEBUG
            //needs paging, maybe user mandatory
            var contacts = _contacts.Where(c => c.UserID == id).ToArray();
            return contacts;
        }
    }
}
