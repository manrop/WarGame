# RESTful Service WarGame

Implementation of an application that plays the card game of War for Aspen Capital's Software Engineer challenge.

## Running the application

This application was build in a Docker container and made available in the Docker Hub repository.
In order to run it you should have [Docker](https://www.docker.com/products/docker) installed in your system and run the following on a command prompt:

``` console
docker pull manualex09/wargameapi:latest
docker run -d -p 41080:80 manualex09/wargameapi:latest
```

## Testing the application

Once the docker container is running, you can now start testing the application.
There are two main functions available for this service: 

### Start a new game (and simulate it).
GET  method with the following form:
``` console
http://localhost:41080/api/startgame?p1={player1}&p2={player2}
```
Where

- {player1} = Name of player 1
- {player2} = Name of player 2

Example:
``` console
http://localhost:41080/api/startgame?p1=william&p2=robert
```

### Consulting the lifetime wins for each player
GET method with the following form:
``` console
http://localhost:41080/api/playerstats
```