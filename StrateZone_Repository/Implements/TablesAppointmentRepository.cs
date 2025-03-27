using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.OpenApi.Any;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;

namespace StrateZone_Repository.Implements
{
    public class TablesAppointmentRepository : ITablesAppointmentRepository
    {
        private readonly StrateZoneDbContext _context;

        public TablesAppointmentRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        public async Task<List<TablesAppointment>> GetAllTablesAppointmentAsync()
        {
            try
            {
                return await _context.TablesAppointments
                                    .Include(ta => ta.Table)
                                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TablesAppointment> GetByIdAsync(int id)
        {
            try
            {
                return await _context.TablesAppointments
                                    .Include(ta => ta.Table)
                                    .SingleOrDefaultAsync(ta => ta.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<List<TablesAppointment>> GetAllTablesAppointmentByTableIdAsync(int id)
        {
            try
            {
                return await _context.TablesAppointments
                                    .Where(ta => ta.TableId == id)
                                    .Include(ta => ta.Table)
                                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<List<TablesAppointment>> GetAllTablesAppointmentByAppointmentIdAsync(int id)
        {
            try
            {
                return await _context.TablesAppointments
                                    .Where(ta => ta.AppointmentId == id)
                                    .Include(ta => ta.Table)
                                        .ThenInclude(t => t.GameType)
                                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TablesAppointment> GetTablesAppointmentByTableIdAndAppointmentIdAsync(int tableId, int appointmentId)
        {
            try
            {
                return await _context.TablesAppointments
                                    .Include(ta => ta.Table)
                                        .ThenInclude(t => t.GameType)
                                    .SingleOrDefaultAsync(ta => ta.TableId == tableId && ta.AppointmentId == appointmentId);
                                    
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TablesAppointment> CreateTablesAppointmentAsync(TablesAppointment tablesAppointment)
        {
            try
            {
                await _context.TablesAppointments.AddAsync(tablesAppointment);
                await _context.SaveChangesAsync();

                return tablesAppointment;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<TablesAppointment>> CreateTablesAppointmentsFromAppointmentAsync(Appointment appointment)
        {
            try
            {
                List<TablesAppointment> tablesAppointments = [.. appointment.TablesAppointments];

                await _context.TablesAppointments.AddRangeAsync(tablesAppointments);
                await _context.SaveChangesAsync();

                return tablesAppointments;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TablesAppointment> UpdateTablesAppointmentAsync(TablesAppointment tablesAppointment, int id)
        {
            try
            {
                if (!await _context.TablesAppointments.AnyAsync(ta => ta.Id == id))
                    throw new Exception("Table appointment with this ID does not exist");

                tablesAppointment.Id = id;

                _context.TablesAppointments.Update(tablesAppointment);
                await _context.SaveChangesAsync();

                return tablesAppointment;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TablesAppointment> DeleteTablesAppointmentAsync(int id)
        {
            try
            {
                var toDelete = await _context.TablesAppointments.FindAsync(id) ?? throw new Exception("No tables_appointment with this ID was found.");

                _context.TablesAppointments.Remove(toDelete);
                await _context.SaveChangesAsync();

                return toDelete;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
