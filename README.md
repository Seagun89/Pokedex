# Pokemon API – (Frontend Repo: https://github.com/Seagun89/my-pokedex-app)

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
✅ Phase 1–6 complete \
🔄 Phase 7 React UI scaffolding \
TODO:\
Creating the Add pokemon page:\
make a form which accepts input for the pokemonrequestDto schema\
"createdBy": "string", // this is retrieved from sessiondata storage\

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
  + [x] JWT authentication
  + [x] Policy-based authorization
  #### *Phase 5*
  + [x] Messaging system
  + [x] Background worker services 
  #### *Phase 6*
  + [x] Microservices architecture
  + [x] Docker
  #### *Phase 7*
  + [ ] React Frontend UI
  + [ ] Azure
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
  - RabbitMQ 

 **Database** 
  - Microsoft SQL Server
 
 **Container** 
  - Docker

**Future FrontEnd** 
  - React Library
  - TypeScript
  
**Future Stack Additions**
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

## Example RegisterRequestDto Schema (for register and login to create JWT)
  ```
{
  "username": "ashketchum",
  "password": "Pikachu123!",
  "email": "ash.ketchum@pokemon.com",
  "claims": [
    {
      "type": "role",
      "value": "User"
    },
    {
      "type": "region",
      "value": "Kanto"
    },
    {
      "type": "subscription",
      "value": "Free"
    }
  ]
}
  ```
  
## Personal Notes
  - IOC: 
    + Inversion of control software design principle. With IoC, the management of object instances within the application is handled in container, aiming to minimize dependencies.
  - JWT: 
    + Json Web Token. Used for security where a user is granted a token containing a header(algo+tokenType), payload(Claims), and signature(signingkey+algo)
    + Need to add Authorize button on swagger to login user with JWT by config AddSwaggerGen()
  - Policy > Roles:
    + Enterprises typically use policy-based rather than role-based Access. Built on top of roles + claims. Allows for checking roles, claims, conditions and scales better
  - Claims & Roles: 
    + Claims are data about the user including roles. Roles are types of claims for categorizing the user and actions they can perform
  - DI lifeTimes:
    + Singleton - same instance whole application lifetime, Scoped - one instance per HTTP request, Transient - new instance for every request or injection
    + Dependent process - transient => Scoped => Singleton
    + If need to use different lifetime in in singleton call serviceProviders then get required scope
  - Identity Services:
    + Creates user, password checking, and manages claims/roles for user authentication and authorization
  - StackExchange Redis:
    + Distributed caching using Redis, allowing for improved performance and reducing db load by caching frequently
    + Need to use JsonSerializer and JsonDeserializer to cache string or uncache 
    + Requires Docker to run redis server: 
      + docker pull redis
      + docker run --name my-redis -p 6379:6379 -d redis (localHost 6379 is default)
      + docker exec -it my-redis redis-cli (Connect to redis CLI in container)
  - Query Optimization techniques:
    + Indexing (uses index seek instead of full table search)
    + Caching
    + Pagination
    + Filtering/Sorting
  - RabbitMQ:
    + Message broker (it accepts and forwards messages) using publishers => queue => consumers
    + Await/Async = Not blocking thread and able to handle I/O operations without block
    + synchronous = sequential start to end
    + background worker = handle work later after response to request 
    + Message = binary blobs of data (use JsonSerializer to convert object into string then Encoding.UTF8.GetBytes into bytes)
    + Can instantiate publisher (singleton service) connection and channel within a consumer (backgroundservice) execute method hosted using AddHostedService
    + Can survey queues and channels/connections by putting localhost:port into browser
- Docker:
    + For containers to communicate with one another you need to create a network then connect containers to it
      > docker network create -d bridge my-bridge-network \
      > docker network connect [OPTIONS] NETWORK CONTAINER
    + Docker-Compose is a tool for defining and running multi-container applications. Acts as an orchestrator for single-host machines.
    + Docker Files uses layers to set up images
- React: 
    + UseState
    + UseEffect
    + React.FC
    + Routes






