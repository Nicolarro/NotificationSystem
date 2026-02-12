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

-
-



 ### How to run the tests 
 -
 -


## Techs

- .NET: 10.0
- REact: 19.2.0
- Entity Framwework :
- Postgres
- Testing: xUnit.net 2.9.3

## Decisions made

- Clean Architecture: To be able to handle further changes in the future in a proper way.
- EntityFramework: Because it is the already integrated ORM in the .NET Framework and it is the most popular ORM so it is easy to find fixes and people that know how to use it
- Docker: To make portable
- xUnit/Testing/E2E: xUnit is the most used testing framework of .NET. Same argument as above. E2E testing was done because it is useless to always test every single part. That's why if the controller provide the proper answer the test has passed.
- Migrations are useds over Sincronize 


## Route

- Local: [API Swagger](http://localhost:5062/api)

## Env vars should be defined

To find an example of the values you can use .env.example
