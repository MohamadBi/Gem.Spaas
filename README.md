# powerplant-coding-challenge


## Content 

- Solution file : Gem.Spaas.sln
- Service project: Gem.Spaas.Service
- Shared models and DTO project: Gem.Spaas.Shared
- Unit test project (XUnit): Gem.Spaas.Xunit.Tests
- Front end project to test websocket broadcasting: Gem.Spaas.Client

## Build and run

The implementation uses .net v5.0, 
- To build: dotnet build
- To run the client application: dotnet run --project Gem.Spaas.Service
- The application will be exposed on: https://localhost:8888
- The endpoint /productionplan can be called by a POST to: https://localhost:8888/productionplan with a valid payload
- It is possible to test with swagger: https://localhost:8888/swagger in development mode. 


## Optional
- The websocket server is exposed with this endpoint: ws://localhost:5555/pp
- To view the websocket broadcasting, run the client application: dotnet run --project Gem.Spaas.Client, go to https://localhost:5001/productionPlans, then execute a POST in the backend, the payload request + generated plan will be displayed.
- The docker is added via the docker file in the project Gem.Spaas.Service





