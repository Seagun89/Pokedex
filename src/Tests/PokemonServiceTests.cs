using API.Dtos;
using API.Models;
using API.Repos;
using API.Services;
using Moq;
using Xunit;

namespace API.Tests
{
    public class PokemonServiceTests
    {
        private readonly Mock<IPokemonRepository> _pokemonRepositoryMock;
        private readonly IPokemonService _pokemonServiceMock;

        public PokemonServiceTests()
        {
            _pokemonRepositoryMock = new Mock<IPokemonRepository>();
            _pokemonServiceMock = new PokemonService(_pokemonRepositoryMock.Object);
        }

        [Fact]
        public void GetPokemonOrThrowAsync_ShouldThrowKeyNotFoundException_WhenPokemonNotExists()
        {
            // Arrange
            int id = -1;

            // Act 
            var actual = Assert.ThrowsAsync<KeyNotFoundException>(() => _pokemonServiceMock.GetPokemonOrThrowAsync(id));
            
            // Assert
            Assert.Equal("Pokemon with this ID does not exist.", actual?.Result.Message);
        }

        [Fact]
        public async Task GetPokemonOrThrowAsync_ShouldReturnPokemon()
        {
            // Arrange
            var expected = "Pikachu";
            var pokemon = _pokemonRepositoryMock.Setup(x => x.GetPokemonAsync(It.IsAny<int>()))
                .ReturnsAsync( 
                    new Pokemon {
                        Id = 1,
                        Name = "Pikachu",
                        Height = 4,
                        Weight = 60,
                        Abilities = new List<Ability>
                        {
                            new Ability
                            {
                                Id = 1,
                                Name = "Static",
                                AbilityType = "Electric",
                                Damage = 10,
                                PokemonId = 1,
                                Description = "Has a 30% chance of paralyzing attacking Pokémon that make contact with Pikachu."
                            }
                        }
                    });

            // Act 
            var actual = await _pokemonServiceMock.GetPokemonOrThrowAsync(1);

            //Assert
            Assert.Equal(expected, actual.Name);
        }

        [Fact]
        public void AddPokemonAsync_ShouldThrowArgumentNullException_WhenPokemonIsNull()
        {
            // Arrange
            PokemonRequestDto? pokemon = null;
            // Act
            var actual = Assert.ThrowsAsync<ArgumentNullException>(() => _pokemonServiceMock.AddPokemonAsync(pokemon ?? throw new ArgumentNullException(nameof(pokemon))))?.Result.Message;
            // Assert
            Assert.Equal("Value cannot be null. (Parameter 'pokemon')", actual);
        }

        [Fact]
        public void AddPokemonAsync_ShouldThrowInvalidOperationException_WhenPokemonExists()
        {
            // Arrange
            var pokemon = new PokemonRequestDto
            {
                Name = "Pikachu",
                Height = 4,
                Weight = 60,
                Abilities = new List<AbilityRequestDto>
                {
                    new AbilityRequestDto
                    {
                        Name = "Static",
                        AbilityType = "Electric",
                        Damage = 10,
                        Description = "Has a 30% chance of paralyzing attacking Pokémon that make contact with Pikachu."
                    }
                }
            };

            _pokemonRepositoryMock.Setup(x => x.PokemonExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

            // Act
            var actual = Assert.ThrowsAsync<InvalidOperationException>(() => _pokemonServiceMock.AddPokemonAsync(pokemon))?.Result.Message;

            // Assert
            Assert.Equal("Pokemon already exists", actual);
        }

        [Fact]
        public void AddPokemonAsync_ShouldAddPokemon()
        {
            // Arrange
            var pokemon = new PokemonRequestDto
            {
                Name = "Pikachu",
                Height = 4,
                Weight = 60,
                Abilities = new List<AbilityRequestDto>
                {
                    new AbilityRequestDto
                    {
                        Name = "Static",
                        AbilityType = "Electric",
                        Damage = 10,
                        Description = "Has a 30% chance of paralyzing attacking Pokémon that make contact with Pikachu."
                    }
                }
            };

            _pokemonRepositoryMock.Setup(x => x.PokemonExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            _pokemonRepositoryMock.Setup(x => x.AddPokemonAsync(It.IsAny<PokemonRequestDto>())).Returns(Task.CompletedTask);

            // Act 
            var actual = _pokemonServiceMock.AddPokemonAsync(pokemon);

            _pokemonRepositoryMock.Verify(x => x.AddPokemonAsync(It.IsAny<PokemonRequestDto>()), Times.Once());
            // Assert
            Assert.True(actual.IsCompletedSuccessfully);
        }

        [Theory]
        [InlineData(0), InlineData(-1)]
        public void DeletePokemonAsync_ShouldReturnArgumentOutOfRangeException_WhenPokemonIdIsZero(int id)
        {
            // Assert and Act
            var actual = Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _pokemonServiceMock.DeletePokemonAsync(id));

            // Assert
            Assert.Equal("Id must be greater than 0. (Parameter 'id')", actual?.Result.Message);
            
        }

        [Fact]
        public void DeletePokemonAsync_ShouldDeletePokemon()
        {
            // Arrange
            _pokemonRepositoryMock.Setup(x => x.GetPokemonAsync(It.IsAny<int>())).ReturnsAsync( new Pokemon
            {
                Id = 1,
                Name = "Pikachu",
                Height = 4,
                Weight = 60,
                Abilities = new List<Ability>
                {}
            });
            
            // Act
            var result = _pokemonServiceMock.DeletePokemonAsync(1).IsCompletedSuccessfully;

            // Assert
            Assert.True(result);
        }
    }
}