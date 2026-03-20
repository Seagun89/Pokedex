# Pokemon API – (Frontend will begin after completion of API Backend)

## Description 
  A production-style RESTful API built with ASP.NET Core designed to 
  demonstrate backend engineering concepts such as API architecture,
  security, caching, distributed systems, and cloud deployment.
  
  The project evolves through multiple phases simulating how a
  real system grows from a basic API into a scalable cloud
  application.
  
  > When I started this project in February, it was just a simple CRUD project but I wanted to expand on my skills. \
  > I added the project to Git after base CRUD creation.

## Current Status
✅ Phase 1–3 complete \
🔄 Phase 4 in progress (JWT Authentication & Role-based Authorization)

## Architecture Path

  #### *Phase 1*
  + [x] DTO mapping
  + [x] Database setup
  + [x] Controller setup
  #### *Phase 2*
  + [x] Service layer
  + [x] Repository layer
  + [x] Unit tests
  #### *Phase 3*
  + [x] Redis caching
  + [x] Pagination/Filtering/Sorting
  + [x] Logging/ExceptionHandling
  #### *Phase 4*
  + [ ] JWT authentication
  + [ ] Role-based authorization
  #### *Phase 5*
  + [ ] Messaging system
  + [ ] Background worker services 
  #### *Phase 6*
  + [ ] Docker
  + [ ] Azure
  #### *Phase 7*
  + [ ] Angular Frontend UI
  #### *Phase 8*
  + [ ] AI integration

### Tech Stack
  **Backend** 
  - ASP.NET Core Web API Framework
  - C#
  - Entity FrameWork Core
  - Redis Caching
  - xUnit
  - Moq
    
 **Database** 
  - Microsoft SQL Server

**Future FrontEnd** 
  - Angular Framework
  - TypeScript
    
**Future Stack Additions**
  - Kafka or RabbitMQ
  - Docker
  - Microsoft Azure
  - OpenAI

## Example HTTP API endpoints
  - GET api/Pokemon/PokeDex/All  **Gets all pokemon*
  - GET api/Pokemon/PokeDex/All?AbilityType=fighting&Ability.AbilityType=fighting **Gets pokemon by filtering*
  - GET api/Pokemon/PokeDex/All?SortBy=Name&IsDescending=true **Gets pokemon by sorting*
  - GET api/Pokemon/PokeDex/GetPokemon/2  **Gets pokemon by ID*
  - POST api/Pokemon/PokeDex/AddPokemon **Adds a pokemon*
  - PUT api/Pokemon/PokeDex/UpdatePokemon/24 **Updates a pokemon by ID and body*
  - DELETE api/Pokemon/PokeDex/DeletePokemon/11 **deletes a pokemon by ID*

## Example PokemonRequestDto Schema
Example payload for creating a Pokémon with abilities.
  ```
{
  "name": "Dialga",
  "height": 5.4,
  "weight": 683.0,
  "abilityType": "Steel/Dragon",
  "abilities":
[{
      "name": "Roar of Time",
      "description": "Dialga unleashes a powerful temporal roar. Cannot be used consecutively.",
      "abilityType": "Dragon",
      "damage": 150
    },
    {
      "name": "Flash Cannon",
      "description": "Shoots a steel beam that may lower the target's special defense.",
      "abilityType": "Steel",
      "damage": 80
    },
    {
      "name": "Dragon Claw",
      "description": "Slashes the target with sharp claws.",
      "abilityType": "Dragon",
      "damage": 80
    },
    {
      "name": "Aura Sphere",
      "description": "Launches a sphere of energy that never misses the target.",
      "abilityType": "Fighting",
      "damage": 80
    }]
}
```
## Personal Notes
  - IOC: 
    + inversion of control software design principle. With IoC, the management of object instances within the application is handled in container, aiming to minimize dependencies.
  - JWT: 
    + Json Web Token. Used for security where a user is granted a token containing a header(algo+tokenType), payload(Claims), and signature(signingkey+algo)
  - Claims > Only Roles: 
    + Claims are data about the user including roles. Roles are types of claims for categorizing the user and actions they can perform
  - DI lifeTimes 
    + Singleton - same instance whole application lifetime, Scoped - one instance per HTTP request, Transient - new instance for every request or injection
  - Identity Services 
    + creates user, password checking, and manages claims/roles for user authentication and authorization
  - StackExchange Redis 
    + distributed caching using Redis, allowing for improved performance and reducing db load by caching frequently






