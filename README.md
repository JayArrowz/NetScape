# NetScape
Modular Runescape Private Server

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites
[PostgresSQL](https://www.postgresql.org/download/)
[Net5.0](https://dotnet.microsoft.com/download/dotnet/5.0)


### Installing
1. Create the folder ```AspNetServerData\Cache``` in your users home folder and add the 317 cache (Currently NetScape only supports the 317 protocol)
2. Go to [appsettings.json](https://github.com/JayArrowz/NetScape/blob/master/NetScape/appsettings.json) and ensure the ConnectionString to your database is correct
3. Go to your Terminal (Make sure its current directory is matching the root of this repo) or VS Console then type:
```
dotnet ef database update --project NetScape
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
