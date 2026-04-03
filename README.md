# NotificationSystem
Notification Management System For Authenticated Users

### Badges

[![CircleCI](https://dl.circleci.com/status-badge/img/gh/Nicolarro/NotificationSystem/tree/master.svg?style=svg)](https://dl.circleci.com/status-badge/redirect/gh/Nicolarro/NotificationSystem/tree/master)

[![Coverage Status](https://coveralls.io/repos/github/Nicolarro/NotificationSystem/badge.svg)](https://coveralls.io/github/Nicolarro/NotificationSystem)

 ### Features

- Create new Users with their Pokemon Ids
- Get User list
- Get User by Id and also gathering Pokemon Names from Poke API
- Update User
- Delete User

## Pre-Requisites

- Docker installed without SUDO Permission
- Docker compose installed without SUDO
- Ports free: 3000 and 5432

 ### How to run the APP

1. Create or review the `.env` file in the project root. This repository already includes one with the database values used by Docker Compose.
2. Start the backend and database with Docker Compose:

```bash
docker compose -f docker-compose.yaml up -d --build
```

3. Wait until both containers are running:

```bash
docker compose -f docker-compose.yaml ps
```

4. Open the API in your browser:

- Swagger UI: `http://localhost:5062/swagger`
- Health/sample endpoint: `http://localhost:5062/hello`
- Users endpoint: `http://localhost:5062/api/User`

5. Start the React client locally in a second terminal:

```bash
cd src/client
npm install
npm run dev
```

6. Open the frontend at `http://localhost:5173`.

Notes:

- The frontend calls the API on `http://localhost:5062` by default.
- The API applies Entity Framework migrations and seeds the database automatically on startup.
- Required ports for the default setup are `5173`, `5062`, and `5432`.

 ### How to run the tests 
1. Make sure PostgreSQL is available on `localhost:5432`.

If you want to use Docker for the database only:

```bash
docker compose -f docker-compose.yaml up -d postgres
```

2. Run the integration test project from the repository root:

```bash
dotnet test tests/TakeHomeChallenge.IntegrationTests/TakeHomeChallenge.IntegrationTests.csproj
```

Notes:

- The tests use a separate database named `TakeHomeChallengeDB_Test`.
- The test suite recreates that database automatically and mocks the external Pokemon API.
- `dotnet run` is not required before running tests.

## Techs

- .NET: 8.0
- React: 19.2.0
- Entity Framwework : 
- Postgres> 
- Testing: xUnit.net 2.9.3

## Decisions made

- Clean Architecture: To be able to handle further changes in the future in a proper way.
- EntityFramework: Because it is the already integrated ORM in the .NET Framework and it is the most popular ORM so it is easy to find fixes and people that know how to use it
- Docker: To make portable
- xUnit/Testing/E2E: xUnit is the most used testing framework of .NET. Same argument as above. E2E testing was done because it is useless to always test every single part. That's why if the controller provide the proper answer the test has passed.
- Migrations are useds over Sincronize 


## Route

- Local: [API Swagger](http://localhost:5062/swagger)

## Env vars should be defined

To find an example of the values you can use .env.example
