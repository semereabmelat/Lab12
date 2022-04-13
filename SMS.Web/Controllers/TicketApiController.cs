using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

using SMS.Data.Models;
using SMS.Data.Services;
using SMS.Web.Models;

namespace SMS.Web.Controllers
{
    [ApiController]  
    [Route("api/ticket")]
    public class TicketApiController : BaseController
    {
        private readonly IStudentService svc;

        public TicketApiController(IStudentService ss)
        {
            svc = ss; // initialise via dependency injection
        } 

        // GET /api/ticket
        [HttpGet]
        public IActionResult GetAll()
        {
            // return open tickets
            var response = svc.GetAllTickets()
                             //.Select(t => ConvertToCustomTicketObject(t))
                             .Select( t => t.ToDto())
                             .ToList(); 

            return Ok(response); 
        }

        [HttpGet("open")]
        public IActionResult GetOpen()
        {
            var response = svc.GetOpenTickets()
                               .Select(t => ConvertToCustomTicketObject(t)
                            ).ToList(); 
            return Ok( response ); 
        }

        // GET /api/ticket/{id}
        [HttpGet("{id}")]
        public IActionResult GetOne(int id)
        {
            var ticket = svc.GetTicket(id);
            if (ticket == null)
            {
                return NotFound();             
            }

            // return custom ticket object
            return Ok(ConvertToCustomTicketObject(ticket));  //local utility method
            //return Ok(ticket.ToDto());                     //or extension method
        }
     
        [HttpGet("search")]
        public IActionResult Search(string query = "", TicketRange range = TicketRange.ALL)
        {                             
            var tickets = svc.SearchTickets(range, query);
            var results = tickets.Select( t => ConvertToCustomTicketObject(t) )
                                 .ToList();
            
            // return custom results list
            return Ok(results);
        }        

        // PATCH /api/ticket/{id}
        // [HttpPatch("{id}")]       
        // public IActionResult Close([Bind("Id, Resolution")] Ticket t)
        // {
        //     // close ticket via service
        //     var ticket = svc.CloseTicket(t.Id, t.Resolution);           
        //     if (ticket == null) {
        //         // ticket could not be closed
        //         return BadRequest();
        //     }

        //     // return updated ticket
        //     return Ok(ticket);    
        // }
       
        // POST /api/ticket
        // [HttpPost]
        // public IActionResult Create(TicketCreateViewModel tvm)
        // {
        //     var ticket = svc.CreateTicket(tvm.StudentId, tvm.Issue);
        //     if (ticket != null) {
        //         return Ok(ticket);
        //     }    
             
        //     // ticket could not be created
        //     return BadRequest();
        // }


        private object ConvertToCustomTicketObject(Ticket t)
        {
            return new {   
                Id = t.Id,
                Issue = t.Issue, 
                CreatedOn = t.CreatedOn.ToShortDateString(),
                Active = t.Active,
                Resolution = t.Resolution,
                Student = t.Student?.Name,
                Email = t.Student?.Email
            };
        }

    }
}
