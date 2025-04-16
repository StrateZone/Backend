using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.CustomModels.ResponseModels
{
    public class SearchedOpponentsResponse
    {
        public List<OpponentResponse> MatchingOpponents = new();
        public List<OpponentResponse> Friends = new();
    }
}
