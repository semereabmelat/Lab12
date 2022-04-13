
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Data.Models;
using SMS.Data.Services;

using SMS.Web.Models;

namespace SMS.Web.Controllers
{
    [Authorize]
    public class StudentController : BaseController
    {
        private IStudentService svc;

        public StudentController()
        {
            svc = new StudentServiceDb();
        }

        // GET /student
        public IActionResult Index()
        {
            // complete this method
            var students = svc.GetStudents();
            
            return View(students);
        }

        // GET /student/details/{id}
        public IActionResult Details(int id)
        {  
            // retrieve the student with specifed id from the service
            var s = svc.GetStudent(id);

            // TBC - replace NotFound with Alert and redirection
            if (s == null)
            {
                // TBC - Display suitable warning alert and redirect
                Alert($"Student {id} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            // pass student as parameter to the view
            return View(s);
        }

        // GET: /student/create
        [Authorize(Roles="admin")]
        public IActionResult Create()
        {   
            // display blank form to create a student
            return View();
        }

        // POST /student/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="admin")]
        public IActionResult Create([Bind("Name, Email, Course, Age, Grade, PhotoUrl")]  Student s)
        {
            // check email is unique for this student
            if (svc.IsDuplicateStudentEmail(s.Email, s.Id))
            {
                // add manual validation error
                ModelState.AddModelError(nameof(s.Email),"The email address is already in use");
            }

            // complete POST action to add student
            if (ModelState.IsValid)
            {
                // pass data to service to store 
                s = svc.AddStudent(s.Name, s.Course, s.Email, s.Age, s.Grade, s.PhotoUrl);
                Alert($"Student created successfully", AlertType.success);

                return RedirectToAction(nameof(Details), new { Id = s.Id});
            }
            
            // redisplay the form for editing as there are validation errors
            return View(s);
        }

        // GET /student/edit/{id}
        [Authorize(Roles="admin,manager")]
        public IActionResult Edit(int id)
        {        
            // load the student using the service
            var s = svc.GetStudent(id);

            // check if s is null and if so alert
            if (s == null)
            {
                Alert($"Student {id} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }   

            // pass student to view for editing
            return View(s);
        }

        // POST /student/edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="admin,manager")]
        public IActionResult Edit(int id, [Bind("Id, Name, Email, Course, Age, Grade, PhotoUrl")] Student s)
        {
            // check email is unique for this student  
            if (svc.IsDuplicateStudentEmail(s.Email,s.Id)) {
                // add manual validation error
                ModelState.AddModelError("Email", "This email is already registered");
            }

            // validate and complete POST action to save student changes
            if (ModelState.IsValid)
            {
                // pass data to service to update
                svc.UpdateStudent(s);      
                Alert("Student updated successfully", AlertType.info);

                return RedirectToAction(nameof(Details), new { Id = s.Id });
            }

            // redisplay the form for editing as validation errors
            return View(s);
        }

        // GET / student/delete/{id}
        [Authorize(Roles="admin")]      
        public IActionResult Delete(int id)
        {       
            // load the student using the service
            var s = svc.GetStudent(id);
            // check the returned student is not null and if so return NotFound()
            if (s == null)
            {
                // TBC - Display suitable warning alert and redirect
                Alert($"Student {id} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }     
            
            // pass student to view for deletion confirmation
            return View(s);
        }

        // POST /student/delete/{id}
        [HttpPost]
        [Authorize(Roles="admin")]
        [ValidateAntiForgeryToken]              
        public IActionResult DeleteConfirm(int id)
        {
            // TBC delete student via service
            svc.DeleteStudent(id);

            Alert("Student deleted successfully", AlertType.info);
            // redirect to the index view
            return RedirectToAction(nameof(Index));
        }


        // ============== Student ticket management ==============

        // GET /student/createticket/{id}
        public IActionResult TicketCreate(int id)
        {     
            var s = svc.GetStudent(id);
            // check the returned student is not null and if so alert
            if (s == null)
            {
                Alert($"Student {id} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            // create a ticket view model and set StudentId (foreign key)
            var ticket = new Ticket { StudentId = id }; 

            return View( ticket );
        }

        // POST /student/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TicketCreate([Bind("StudentId, Issue")] Ticket t)
        {
            if (ModelState.IsValid)
            {                
                var ticket = svc.CreateTicket(t.StudentId, t.Issue);
                Alert($"Ticket created successfully for student {t.StudentId}", AlertType.info);
                return RedirectToAction(nameof(Details), new { Id = ticket.StudentId });
            }
            // redisplay the form for editing
            return View(t);
        }

        // GET /student/ticketdelete/{id}
        public IActionResult TicketDelete(int id)
        {
            // load the ticket using the service
            var ticket = svc.GetTicket(id);
            // check the returned Ticket is not null and if so return NotFound()
            if (ticket == null)
            {
                Alert($"Ticket {id} not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }     
            
            // pass ticket to view for deletion confirmation
            return View(ticket);
        }

        // POST /student/ticketdeleteconfirm/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TicketDeleteConfirm(int id, int studentId)
        {
            // delete student via service
            svc.DeleteTicket(id);
            Alert($"Ticket deleted successfully for student {studentId}", AlertType.info);
            
            // redirect to the ticket index view
            return RedirectToAction(nameof(Details), new { Id = studentId });
        }


        // ============ Student Module Management ===============
    
        // GET /student/moduleupdate/{id}
        [Authorize(Roles = "admin,manager")]
        public IActionResult ModuleUpdate(int id)
        {
            var sm = svc.GetStudentModule(id);  
            if (sm == null)
            {
                Alert("Student Module Not Found", AlertType.warning);
                return RedirectToAction(nameof(Details));
            }
            var vm = new StudentModuleViewModel {
                StudentId = sm.StudentId,
                ModuleId = sm.ModuleId,
                Mark = sm.Mark
            };            
            return View( vm );
        }

        [HttpPost]
        [Authorize(Roles = "admin,manager")]
        [ValidateAntiForgeryToken]
        public IActionResult ModuleUpdate([Bind("StudentId, ModuleId, Mark")] StudentModuleViewModel sm)
        {
            if (ModelState.IsValid)
            {                
                svc.UpdateStudentModuleMark(sm.StudentId, sm.ModuleId, sm.Mark);

                return RedirectToAction(nameof(Details), new { Id = sm.StudentId });
            }
            // redisplay the form for editing
            return View(sm);
        }

        // GET /student/createmodule/{id}
        [Authorize(Roles = "admin,manager")]
        public IActionResult ModuleCreate(int id)
        {
            var sm = svc.GetStudent(id);  
            if (sm == null)
            {
                Alert("StudentNot Found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }  
            var modules = svc.GetAvailableModulesForStudent(id);  
            var vm = new StudentModuleViewModel {
                Modules = new SelectList(modules,"Id","Title"),
                StudentId = id
            };  
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "admin,manager")]
        [ValidateAntiForgeryToken]
        public IActionResult ModuleCreate([Bind("StudentId, ModuleId, Mark")] StudentModuleViewModel sm)
        {
            if (ModelState.IsValid)
            {                
                svc.AddStudentToModule(sm.StudentId, sm.ModuleId, sm.Mark);
                svc.UpdateStudentModuleMark(sm.StudentId, sm.ModuleId, sm.Mark);
                return RedirectToAction(nameof(Details), new { Id = sm.StudentId });
            }
            // redisplay the form for editing  
            // note - we must re-create the selectlist and update view model Modules property
            //        this is because the form does not retain the select list values when posted to server
            var modules = svc.GetAvailableModulesForStudent(sm.StudentId);
            sm.Modules = new SelectList(modules,"Id","Title"); 
            return View(sm);
        }

        [HttpPost]
        [Authorize(Roles = "admin,manager")]
        [ValidateAntiForgeryToken]
        public IActionResult ModuleDelete(int id)
        {
            var sm = svc.GetStudentModule(id);
            if (sm == null)
            {
                Alert("Student Modulet Found", AlertType.warning);
                return RedirectToAction(nameof(Index));   
            }   
            
            svc.RemoveStudentFromModule(sm.StudentId,sm.ModuleId);
            return RedirectToAction(nameof(Details), new { Id = sm.StudentId });
        }
        
    }
}
