# Project Title

One Paragraph of project description goes here

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites
```
https://www.postgresql.org/download/
https://dotnet.microsoft.com/download/dotnet/5.0
```

### Installing

A step by step series of examples that tell you how to get a development env running

1. Create the folder ```AspNetServerData\Cache``` in your home folder
2. Go to [appsettings.json](https://github.com/JayArrowz/NetScape/blob/master/NetScape/appsettings.json) and ensure the ConnectionString to your database is correct
3. Go to your Terminal (Make sure its current directory is matching the root of this repo) or VS Console then type:
```
dotnet ef database update --project NetScape
```
This should generate the tables in the postgresDB

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
