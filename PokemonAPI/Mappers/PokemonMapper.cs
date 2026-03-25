using PokemonAPI.Dtos;
using PokemonAPI.Models;

namespace PokemonAPI.Mappers
{
    public static class PokemonMapper // used static for pokemon mapper since we don't need to maintain any state and can directly call the mapping methods without instantiating the class
    {
        public static PokemonResponseDto MapToPokemonResponseDto(this Pokemon pokemonModel)
        {
            return new PokemonResponseDto // mapping properties from pokemon entity to response dto, only including properties that we want to expose in the PokemonAPI response
            {
                Id = pokemonModel.Id,
                Name = pokemonModel.Name,
                AbilityType = pokemonModel.AbilityType
            };
        }
        public static Pokemon MapToPokemonModel(this PokemonRequestDto pokemonRequestDto)
        {
            return new Pokemon 
            {
                Name = pokemonRequestDto.Name,
                Height = pokemonRequestDto.Height,
                Weight = pokemonRequestDto.Weight,
                AbilityType = pokemonRequestDto.AbilityType,
                Abilities = pokemonRequestDto.Abilities.Select( a => new Ability
                    {
                        Name = a.Name,
                        Description = a.Description,
                        AbilityType = a.AbilityType,
                        Damage = a.Damage
                    }
                ).ToList()
            };
        }
    }
}