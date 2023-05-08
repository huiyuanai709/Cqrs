# CQRS tools

Create directory, commands, queries in order to implement CQRS pattern

## Installation

```C#
dotnet tool install -g huiyuanai709.cqrs
```

## Usage

> Create Directory

Resources/

Resources/Commands/

Resources/Queries/

```C#
cqrs init
```

> Create command, CommandHandler and optional return type

```C#
cqrs command --name LoginCommand --type User
```

> Create Query, QueryHandler and optional return type

```C#
cqrs query --name UserQuery --type User
```