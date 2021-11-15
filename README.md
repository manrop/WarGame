# RESTful Service WarGame

Implementation of an application that plays the card game of War.
Aspen Capital's Software Engineer challenge.

## Running the application

This application was build in a Docker container and made available in the Docker Hub repository.

In order to run it you should have [Docker](https://www.docker.com/products/docker) installed and run the following commands on command prompt:

'''console
docker pull manualex09/wargameapi:latest
docker run -d -p 41080:80 manualex09/wargameapi:latest
'''
