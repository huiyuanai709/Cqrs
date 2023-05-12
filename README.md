# CQRS tools

Create directory, commands, queries in order to implement CQRS pattern

## Installation

```C#
dotnet tool install -g huiyuanai709.cqrs
```

## Help

```shell
cqrs help
```

## Usage

> Create Application, Domain, Infrastructure and add this projects to specified sln. By default, it will traverse the folders and find the first sln

```shell
cqrs project --name Identity
```

> Create Directory

Resources/

Resources/Commands/

Resources/Queries/

```shell
cqrs init
```

> Create command, CommandHandler and optional return type

```shell
cqrs command --name LoginCommand --type User
```

> Create Query, QueryHandler and optional return type

```shell
cqrs query --name UserQuery --type User
```