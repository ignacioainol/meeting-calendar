using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using TCS.EF.Entidades;
using System.Web.Mvc;
using System.Collections.Generic;
using System.ComponentModel;

namespace TCS.WebUI.Interface
{
    public interface IMeetingService
    {
        // CREAR tareas para AGREGAR, ACTUALIZAR Y BORRAR, --"ASIGNAR"

        Task<bool> SaveMeeting(Meeting data);

        Task<List<Meeting>> GetMeetingsByUser(int userId);

        //Task<Meeting> DeleteMeeting(int id);
        Task<dynamic> DeleteMetting(Meeting data);
    }



}
