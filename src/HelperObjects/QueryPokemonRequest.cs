using API.Dtos;

namespace API.HelperObjects
{
    public class QueryPokemonRequest
    {
        public string AbilityType {get; set;} = string.Empty;
        public int? Height {get; set;}
        public AbilityFilterDto? ability { get; set; }
    }
}