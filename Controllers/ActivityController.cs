using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using exam.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Routing;

namespace exam.Controllers {
    public class ActivityController : Controller {
        private ExamContext _context;
        public ActivityController(ExamContext context) {
            _context = context;
        }
        // GET: /Home/
        [HttpGet]
        [Route("activity")]
        public IActionResult Activity() {
            if (HttpContext.Session.GetInt32("UserId") == 0 || HttpContext.Session.GetInt32("UserId") == null) {
                TempData["message"] = "You have to be logged in to see this page!";
                return RedirectToAction("Index", "User");
            }
            List<Activity> AllActivities = _context.activities.Where(t => t.date > DateTime.Now).Include(User => User.User).Include(Part => Part.People).OrderBy(a => a.date).ToList();
            ViewBag.AllActivities = AllActivities;
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            ViewBag.Message = TempData["message"];
            return View();
        }

        [Route("addpage")]
        public IActionResult AddPage()  {
            if (HttpContext.Session.GetInt32("UserId") == 0 || HttpContext.Session.GetInt32("UserId") == null) {
                TempData["message"] = "You have to be logged in to see this page!";
                return RedirectToAction("Index", "User");
            }
            ViewBag.Message = TempData["message"];
            return View("AddPage");
        }

        [Route("page/{ActId}")]
        public IActionResult Page(int ActId)  {
            if (HttpContext.Session.GetInt32("UserId") == 0 || HttpContext.Session.GetInt32("UserId") == null) {
                TempData["message"] = "You have to be logged in to see this page!";
                return RedirectToAction("Index", "User");
            }
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            ViewBag.Activity = _context.activities.Where(activity => activity.id == ActId).Include(User => User.User).SingleOrDefault();
            ViewBag.Part = _context.part.Where(g => g.activityId == ActId).Include(u => u.User).ToList();
            // Where(y => y.weddingId == WedId)
            return View();
        }

        [HttpPost]
        [Route("add")]
        public IActionResult Add(Activity Act) {
            
            if(ModelState.IsValid == false) {
                ViewBag.Errors = ModelState.Values;
                ViewBag.Status = true;
                return View("AddPage"); 
            } else {
                if (Act.date < DateTime.Now.Date) {
                    TempData["message"] = "Date cant be in the past";
                    return RedirectToAction("AddPage");
                } else {
                    Act.created_at = DateTime.Now;
                    Act.updated_at = DateTime.Now;
                    Act.userId = (int)HttpContext.Session.GetInt32("UserId");
                    _context.Add(Act);
                    _context.SaveChanges();
                    TempData["message"] = "Sucessfully added a new activity";
                }
            }
            Activity Activity = new Activity();
            Activity = _context.activities.Where(activity => activity.created_at.Ticks == Act.created_at.Ticks).SingleOrDefault();
            int ID = Activity.id;
            return RedirectToAction("Page", new { @ActId = ID });
        }

        [Route("join/{ActID}")]
        public IActionResult Join(int ActId) {
            if(ModelState.IsValid) {
                int sessionID = (int)HttpContext.Session.GetInt32("UserId");
                Activity Adding = _context.activities.Where(a => a.id == ActId).SingleOrDefault();
                List<Part> All = _context.part.Where(u => u.userId == sessionID).Include(a => a.Activity).ToList();
                DateTime end = new DateTime();
                DateTime newEnd = new DateTime();
                foreach (var date in All) {
                    DateTime begin = date.Activity.date;
                    TimeSpan time = TimeSpan.Parse(date.Activity.time);
                    DateTime start = begin.Add(time);

                    DateTime newBegin = Adding.date;
                    TimeSpan newTime = TimeSpan.Parse(Adding.time);
                    DateTime newStart = newBegin.Add(newTime);
                    if (date.Activity.type == "Hours") {
                        Double value = Convert.ToDouble(date.Activity.duration);
                        end = start.AddHours(value);
                    } else if (date.Activity.type == "Minutes") {
                        Double value = Convert.ToDouble(date.Activity.duration);
                        end = start.AddMinutes(value);
                    } else {
                        Double value = Convert.ToDouble(date.Activity.duration);
                        end = start.AddDays(value);
                    }

                    if (Adding.type == "Hours") {
                        Double newValue = Convert.ToDouble(Adding.duration);
                        newEnd = newStart.AddHours(newValue);
                    } else if (Adding.type == "Days") {
                        Double newValue = Convert.ToDouble(Adding.duration);
                        newEnd = newStart.AddDays(newValue);
                    } else {
                        Double newValue = Convert.ToDouble(Adding.duration);
                        newEnd = newStart.AddMinutes(newValue);
                    }

                    if ((newStart > start) && (newStart < end)) {
                        TempData["message"] = "The start of this activity falls within one of your other activities!";
                        return RedirectToAction("Activity");
                    } else if ((newEnd > start) && (newEnd < end)) {
                        TempData["message"] = "The end of this activity falls within one of your other activities!";
                        return RedirectToAction("Activity");
                    }
                }
                Part New = new Part();
                New.activityId = ActId;
                New.userId = sessionID;
                _context.Add(New);
                _context.SaveChanges();
                TempData["message"] = "Sucessfully joined an activity";
            } else {
                TempData["message"] = "Unsucessful in joining";
            }
            return RedirectToAction("Activity");
        }

        [Route("leave/{ActID}")]
        public IActionResult Leave(int ActId) {
            int CurrId = (int)HttpContext.Session.GetInt32("UserId");
            Part remove = _context.part.Where(user => user.activityId == ActId && user.userId == CurrId).SingleOrDefault();
            _context.part.Remove(remove);
            _context.SaveChanges();
            TempData["message"] = "Sucessfully left an activity";
            return RedirectToAction("Activity");
        }

        [Route("delete/{ActID}")]
        public IActionResult Delete(int ActId) {
            if(ModelState.IsValid) {
                Activity remove = _context.activities.Where(x => x.id == ActId).SingleOrDefault();
                List<Part> guest = _context.part.Where(g => g.activityId == ActId).ToList();
                foreach (var persom in guest) {
                    _context.part.Remove(persom);
                    _context.SaveChanges();
                }
                _context.activities.Remove(remove);
                _context.SaveChanges();
                TempData["message"] = "Sucessfully deleted an activity";
            } else {
                TempData["message"] = "Unsucessful in deleting";
            }
            return RedirectToAction("Activity");
        }
    }
}
