using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TCS.EF.Entidades;
using TCS.WebUI.Interface;

namespace TCS.WebUI.Controllers.SIG
{
    public class MeetingsController : Controller
    {
        private readonly IMeetingService _meetingService;


        public MeetingsController(IMeetingService meetingService)
        {
            _meetingService = meetingService;
        }

        // GET: Meetings
        public async Task<IActionResult> Index()
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var response = await _meetingService.GetMeetingsByUser(Convert.ToInt32(userId));
            //return response;
            //var tCSContext = _context.Meetings.Include(m => m.User);
            //TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            //var namemonth = DateTime.Now.ToString("MMMM");
            //ViewBag.monthheaderdate = textInfo.ToTitleCase(namemonth).ToLower();
            //ViewBag.yearheader = DateTime.Now.Year;
            //return View();
            ViewBag.Meetings = response;
            return View();

        }

        // GET: Meetings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //if (id == null || _context.Meetings == null)
            //{
            //    return NotFound();
            //}

            //var meeting = await _context.Meetings
            //    .Include(m => m.User)
            //    .FirstOrDefaultAsync(m => m.MeetingId == id);
            //if (meeting == null)
            //{
            //    return NotFound();
            //}

            //return View(meeting);
            return View();
        }

        // GET: Meetings/Create
        public IActionResult Create()
        {
            //ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Culture");
            //TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            //var namemonth = DateTime.Now.ToString("MMMM");
            //ViewBag.monthheaderdate = textInfo.ToTitleCase(namemonth).ToLower();
            //return View();
            return View();
        }

        // POST: Meetings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Meeting meetingRequest)
        {


            //if (ModelState.IsValid)
            //{
            //        string userid = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            //        meetingRequest.UserId = Convert.ToInt32(userid);
            //        _context.Add(meetingRequest);
            //        await _context.SaveChangesAsync();
            //        return RedirectToAction(nameof(Index));

            //}
            //ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Culture", meetingRequest.UserId);
            //return View(meetingRequest);
            var response = false;
            if (ModelState.IsValid)
            {
                string userid = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                meetingRequest.UserId = Convert.ToInt32(userid);
                response = await _meetingService.SaveMeeting(meetingRequest);
            }

            return RedirectToAction(nameof(Index));

        }


        // GET: Meetings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //if (id == null || _context.Meetings == null)
            //{
            //    return NotFound();
            //}

            //var meeting = await _context.Meetings.FindAsync(id);
            //if (meeting == null)
            //{
            //    return NotFound();
            //}
            //ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Culture", meeting.UserId);



            //return View(meeting);
            return View();
        }

        // POST: Meetings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Meeting meeting)
        {
            //if (id != meeting.MeetingId)
            //{
            //    return NotFound();
            //}

            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        _context.Update(meeting);
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!MeetingExists(meeting.MeetingId))
            //        {
            //            return NotFound();
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }
            //    return RedirectToAction(nameof(Index));
            //}
            //ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Culture", meeting.UserId);
            //return View(meeting);

            return View();
        }

        // GET: Meetings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            //if (id == null || _context.Meetings == null)
            //{
            //    return NotFound();
            //}

            //var meeting = await _context.Meetings
            //    .Include(m => m.User)
            //    .FirstOrDefaultAsync(m => m.MeetingId == id);
            //if (meeting == null)
            //{
            //    return NotFound();
            //}

            //return View(meeting);
            return View();
        }

        // POST: Meetings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //if (_meetingService.Meetings == null)
            //{
            //    return Problem("Entity set 'TCSContext.Meetings'  is null.");
            //}
            //var meeting = await _meetingService.Meetings.FindAsync(id);
            //if (meeting != null)
            //{
            //    _meetingService.Meetings.Remove(meeting);
            //}

            //await _meetingService.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
            return View();
        }

        private bool MeetingExists(int id)
        {
            //return _context.Meetings.Any(e => e.MeetingId == id);
            return false;
        }
    }
}
