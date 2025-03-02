using Microsoft.EntityFrameworkCore;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;

namespace StrateZone_Repository.Implements
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly StrateZoneDbContext _context;

        public AppointmentRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        public async Task<List<Appointment>> GetAppointmentsAsync()
        {
            try
            {
                return await _context.Appointments.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Appointment> GetAppointmentByIdAsync(int id)
        {
            try
            {
                return await _context.Appointments.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Appointment> CreateAppointmentAsync(Appointment appointment)
        {
            try
            {
                await _context.Appointments.AddAsync(appointment);
                await _context.SaveChangesAsync();

                return appointment;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Appointment> UpdateAppointmentAsync(Appointment appointment, int id)
        {
            try
            {
                if (await _context.Appointments.FindAsync(id) == null) throw new Exception("Appointment with this ID does not exist");

                _context.Appointments.Update(appointment);
                await _context.SaveChangesAsync();

                return appointment;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Appointment> DeleteAppointmentAsync(int id)
        {
            try
            {
                Appointment toRemove = await _context.Appointments.FindAsync(id) ?? throw new Exception("Appointment with this ID does not exist");

                _context.Appointments.Remove(toRemove);
                await _context.SaveChangesAsync();

                return toRemove;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
