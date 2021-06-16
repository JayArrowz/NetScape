# NetScape
Modular Runescape Private Server in C# for learning purposes & fun!

# Badges 
[![wakatime](https://wakatime.com/badge/github/JayArrowz/NetScape.svg)](https://wakatime.com/badge/github/JayArrowz/NetScape) [![.NET](https://github.com/JayArrowz/NetScape/actions/workflows/docs.yml/badge.svg?branch=master)](https://github.com/JayArrowz/NetScape/actions/workflows/docs.yml)

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites
* [PostgresSQL](https://www.postgresql.org/download/)
* [Net5.0](https://dotnet.microsoft.com/download/dotnet/5.0)

### Code Docs
* [Code Docs](https://jayarrowz.github.io/NetScape/api/)

### Installing
1. Create the folder ```AspNetServerData\Cache``` in your users home folder and add the cache
2. Go to [appsettings.json](https://github.com/JayArrowz/NetScape/blob/master/NetScape/appsettings.json) and ensure the ConnectionString to your database is correct
3. Go to your Terminal (Make sure its current directory is matching the root of this repo) or VS Console then type:
```
dotnet tool install -g dotnet-ef
dotnet build
dotnet ef database update --project NetScape
```

To Run in Terminal: 
```
dotnet run --project netscape
```

## Contributing

Please read [CONTRIBUTING.md](https://gist.github.com/PurpleBooth/b24679402957c63ec426) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/JayArrowz/NetScape/tags). 

## Authors

* **JayArrowz** - [JayArrowz](https://github.com/JayArrowz)

See also the list of [contributors](https://github.com/JayArrowz/NetScape/contributors) who participated in this project.

## Acknowledgments
* JayArrowz
* Graham
* Major
* Scu11
* https://github.com/villermen/runescape-cache-tools
* https://github.com/apollo-rsps/apollo
