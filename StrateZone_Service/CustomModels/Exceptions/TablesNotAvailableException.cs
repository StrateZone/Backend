using StrateZone_Service.CustomModels.RequestModels;
using System.Runtime.Serialization;
using System.Text.Json;

namespace StrateZone_Service.CustomModels.Exceptions
{
    [Serializable]
    public class TablesNotAvailableException : Exception
    {
        public object ErrorResponse { get; }

        public TablesNotAvailableException(List<TablesAppointmentRequest> unavailableTables)
        {
            ErrorResponse = new
            {
                error = new
                {
                    code = "TABLE_NOT_AVAILABLE",
                    message = "Some tables are not available",
                    unavailable_tables = unavailableTables.Select(t => new
                    {
                        table_id = t.TableId,
                        start_time = t.ScheduleTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                        end_time = t.EndTime.ToString("yyyy-MM-ddTHH:mm:ss")
                    })
                }
            };
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(ErrorResponse);
        }
    }
}