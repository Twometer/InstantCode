# InstantCode
[![Build status](https://ci.appveyor.com/api/projects/status/lf1dc3g9is1k9evf?svg=true)](https://ci.appveyor.com/project/Twometer/instantcode)

_Two coders, one project, in realtime._ InstantCode makes working together
on a Visual Studio project in realtime easy.

## Introduction
Create a session, invite people and everyone participating will be able to modify the
code, with everyone being able to see the changes immediately. It not only synchronizes
code changes, but also metadata such as cursor positions.

## Installation
All required software can be downloaded from the latest release on GitHub

### Client
The client is a Visual Studio extension all participants have to install before using
InstantCode.

### Server
The server is a .NET Core console application. On first startup, it will generate a
configuration file `config.json` with the following contents:
```json
{
    "Port" : 49374,
    "Password" : ""
}
```

#### `Port`
The network port the server should listen on.

#### `Password`
This password is used for AES encryption between server and clients. It is like a PSK, so
you have to tell all participants the password for your server before you can start coding.
> **WARNING**: Do not leave this on the default setting!

## Building from source
Prerequisites: 
 - Visual Studio Extension SDK
 - .NET Core SDK

The easiest way to build from source is to clone the repository and build the solution using
Visual Studio.
