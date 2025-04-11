using StrateZone_Repository.Entities;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Interfaces
{
    public interface ILikeService
    {
        Task<LikeModel> CreateLike(LikeRequest like);
        Task<LikeModel> DeleteLike(int id);
    }
}
