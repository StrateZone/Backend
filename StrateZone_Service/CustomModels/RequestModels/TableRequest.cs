using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.RequestModels
{
    public class TableRequest
    {
        [Required]
        public int Room_Id { get; set; }

        [Required]
        public int GameType_Id { get; set; }
    }
}
