using API.Dtos;

namespace API.HelperObjects
{
    public class QueryPokemonRequest
    {
        public string AbilityType {get; set;} = string.Empty;
        public int? Height {get; set;}
        public AbilityFilterDto? Ability { get; set; }
        public string SortBy { get; set; } = string.Empty;
        public bool IsDescending {get; set; } = false;
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
    }
}