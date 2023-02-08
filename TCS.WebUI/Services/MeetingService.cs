using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TCS.WebUI.Interface;
using TCS.EF.Entidades;
using TCS.EF;
using System.Collections.Generic;
using System.Linq;
using Org.BouncyCastle.Asn1.Ocsp;

namespace TCS.WebUI.Services
{
    public class MeetingService : IMeetingService
    {
        private readonly TCSContext _context;

        public MeetingService(TCSContext context)
        {
            _context = context;
        }

        public async Task<List<Meeting>> GetMeetingsByUser(int userId)
        {

            var response = await _context.Meetings.Where(m => m.UserId == userId).ToListAsync();
            //var newRecord = _mapper.Map<User>(request);
            return response;

        }

        public async Task<bool> SaveMeeting(Meeting meetingRequest)
        {
            //string userid = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            //meetingRequest.UserId = Convert.ToInt32(userid);
            _context.Add(meetingRequest);
            var response = await _context.SaveChangesAsync();

            if (response > 0)
            {
                return true;
            }
            return false;
            
            
        }

        //Task<Meeting> IMeetingService.DeleteMeeting(int id)
        //{
        //    throw new NotImplementedException();
        //}

        public Task<dynamic> DeleteMetting(Meeting data)
        {
            throw new NotImplementedException();
        }
    }
    }
