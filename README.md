[![Website](https://img.shields.io/website-up-down-green-red/http/shields.io.svg?label=elky-essay)](https://elky84.github.io)
![Made with](https://img.shields.io/badge/made%20with-.NET8-blue.svg)

[![Publish Nuget Github Package](https://github.com/elky84/EzDotNetty/actions/workflows/publish_github.yml/badge.svg)](https://github.com/elky84/EzDotNetty/actions/workflows/publish_github.yml)
[![Publish Nuget Package](https://github.com/elky84/EzDotNetty/actions/workflows/publish_nuget.yml/badge.svg)](https://github.com/elky84/EzDotNetty/actions/workflows/publish_nuget.yml)

![GitHub forks](https://img.shields.io/github/forks/elky84/EzDotNetty.svg?style=social&label=Fork)
![GitHub stars](https://img.shields.io/github/stars/elky84/EzDotNetty.svg?style=social&label=Stars)
![GitHub watchers](https://img.shields.io/github/watchers/elky84/EzDotNetty.svg?style=social&label=Watch)
![GitHub followers](https://img.shields.io/github/followers/elky84.svg?style=social&label=Follow)

![GitHub](https://img.shields.io/github/license/mashape/apistatus.svg)
![GitHub repo size in bytes](https://img.shields.io/github/repo-size/elky84/EzDotNetty.svg)
![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/elky84/EzDotNetty.svg)


# EzDotNetty

## introduce

Easily usable with [DotNetty](https://github.com/Azure/DotNetty).

Implemented by C# .NET 8

The purpose of this project is to easily build and operate a server using DotNetty.

## nuget

<https://www.nuget.org/packages/EzDotNetty/>

## Example

### TestClient

<https://github.com/elky84/EzDotNetty/TestClient>

### TestServer

<https://github.com/elky84/EzDotNetty/TestServer>

## structure review

### 한국어
- BootstrapHelper
    - 손쉽게 DotNetty 기본 옵션으로 가동해주는 Helper Class
    - 생성자에 넘기는 THandler Type에 따라, 소켓 커넥션마다 Handler를 생성해줌
        - 이는 DotNetty의 구조이자 규칙이며, Handler는 커넥션마다 생성된다는 점을 감안해서 코딩 해주어야 함.
- Config
    - 말그대로 설정 파일
        - AppSettings.json 으로 부터 읽어옴
        - 설정 파일에 property가 있을 경우 이곳에서 확장해도 되고, 기능은 별개의 설정 파일을 쓰거나, 별개의 class를 생성해서 읽어도 무방하긴 함
            - IConfigurationRoot 자체가 string 형태로 설정 값에 대한 접근을 허용하기 때문
- Handler
    - 기본적인 DotNetty 이벤트를 받아서 처리해주는 ChannelHandlerAdapter의 상속받은 클래스 이자, 일부 메소드를 abstract로 지정한 추상 클래스
        - BootstrapHelper에서 THandler의 명시 타입을 규칙화 하기 위해 존재하는 클래스
- Logging
    - Serilog에 대한 설정 기능
    - LoggerId를 상속 받아 구현함으로써, LoggerId에 대한 동적 추가 가능하게 구현
- Service
    - SchedulerService
        - 특정 주기마다 불려지는 기능을 지원하는 서비스
- Util
    - JsonUtil
        - Json의 ExtensionData 부분을 Populate 해주는 Extend 기능을 위한 클래스

### english
- BootstrapHelper
    - Helper Class that easily operates with DotNetty basic options
    - Depending on the THandler Type passed to the constructor, a Handler is created for each socket connection.
        - This is the structure and rule of DotNetty, and the Handler should be coded considering that it is created for each connection.
- Config
    - Literally config file
        - Read from AppSettings.json
        - If there is a property in the configuration file, it can be extended here, and it is okay to write a separate configuration file or create and read a separate class for functions.
            - Because IConfigurationRoot itself allows access to the setting value in the form of a string
- Handler
    - A class inherited from ChannelHandlerAdapter that receives and handles basic DotNetty events, and an abstract class with some methods designated as abstract.
        - A class that exists to regularize the explicit type of THandler in BootstrapHelper.
- Logging
    - Setting function for Serilog
    - By inheriting and implementing LoggerId, it is possible to dynamically add LoggerId.
- Service
    - SchedulerService
        - A service that supports a function called every specific cycle
- Util
    - JsonUtil
        - A class for the extend function that populates the ExtensionData part of Json.

## version history

### v1.0.20

use .NET 8

### v1.0.19

improve logging interface.
changed namespace LogConfigration -> Loggers

### v1.0.18

Arranged to use serilog default options

### v1.0.17

Added file logging configuration from serilog.json.

### v1.0.16

added file logging to default option.

### v1.0.15

Change private set of Enumeration to set for json deserialize

### v1.0.14

Changed Newtonsoft.json to use ZeroFormatter as Protocol function.
Changed to a more explicit example where the test client and test server send and receive Protocols directly.

### v1.0.13

Remove some logger.
Initialize all logger in Loggin.Collection.
Need register LoggerId. (Per LoggerId type. Include inheritance)

### v1.0.12

Fix invalid namespace. (wrong namespace, sln, csproj word issue)

### v1.0.11

Added log level switch function. (SeriLog)

### v1.0.10

Added Enumeration Get method.

### v1.0.9

Improve Logging Class. (change to use LoggerId)
