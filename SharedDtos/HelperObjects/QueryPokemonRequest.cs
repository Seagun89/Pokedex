using SharedDtos.Dtos;

namespace SharedDtos.HelperObjects
{
    // changing query requests to string type to accom. frontend URLSearchParam obj
    public class QueryPokemonRequest
    {
        public string AbilityType {get; set;} = string.Empty;
        public int? Height {get; set;}
        public AbilityFilterDto? Ability { get; set; }
        public string SortBy { get; set; } = string.Empty;
        public bool IsDescending {get; set; } = false;
        public string PageNumber { get; set; } = "1";
        public string PageSize { get; set; } = "10";
        public string Username { get; set; } = string.Empty;
    }
}