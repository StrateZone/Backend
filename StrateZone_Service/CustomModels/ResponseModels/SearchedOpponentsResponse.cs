using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.ResponseModels
{
    public class SearchedOpponentsResponse
    {
        public HashSet<int> ExcludedIds = new HashSet<int>();
        public List<OpponentResponse> MatchingOpponents = new List<OpponentResponse>();
    }
}
