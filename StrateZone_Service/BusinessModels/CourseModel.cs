using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.BusinessModels
{
    public class CourseModel
    {
        public int CourseId { get; set; }

        public string? CourseName { get; set; }

        public string? Description { get; set; }

        public int? InstructorId { get; set; }

        public StrateZone_Repository.Parameters.PostgreEnums.GameTypeEnum GameType { get; set; }

        public SkillLevel SkillLevel { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public int? MaxParticipants { get; set; }

        public CourseStatus CourseStatus { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual UserModel? Instructor { get; set; }

        public virtual ICollection<PriceModel> Prices { get; set; } = new List<PriceModel>();
    }
}
