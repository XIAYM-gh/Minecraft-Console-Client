## 这是一个MCC的汉化版本 如果你有更好的意见，请fork此项目.
## 状态
Translated(已翻译) : 61%<br>
已更新到官方Commit : 0c88c18ea060853b32d5b23684d9323bfd3840ae (正在更新..)<br>
**Change color of "feature not handled" from gray to red (#1265)**<br>
下载汉化版本请前往"Releases"页面<br>
提交bug请去:<br>
[ORelio issues](https://github.com/ORelio/Minecraft-Console-Client/issues)

我的世界控制台客户端(MCC)
========================

[![Appveyor build status](https://ci.appveyor.com/api/projects/status/github/ORelio/Minecraft-Console-Client?branch=Indev)](https://ci.appveyor.com/project/ORelio/minecraft-console-client)

我的世界控制台客户端(MCC)是一个轻量级的程序，它允许你连接至任何我的世界服务器，
简单快速地发送指令和接收聊天信息而不需要开启游戏。它也提供了多种自动化管理服务器和进行其他操作的可能性。

**注意！** MCC仅可以连接到**我的世界Java版**，而**不能连接到我的世界基岩版/中国版！**

## 正在寻找维护者

由于不再有足够的时间来为新的我的世界版本提供升级和修复错误，开发者正在寻找有开发动力的人来接手该项目。如果您认为您可以接手该项目，请查看 [issues](https://github.com/ORelio/Minecraft-Console-Client/issues?q=is%3Aissue+is%3Aopen+label%3Awaiting-for%3Acontributor) 部分 :)

## 下载

从这里获得最新的exe文件[开发构建](https://ci.appveyor.com/project/ORelio/minecraft-console-client/build/artifacts)。
这是个.NET可执行文件，它也能运行于Mac OS和Linux系统。

## 如何使用


在此查看[示例配置文件](MinecraftClient/config/) ，其中有基础使用教程 README 文件。<br>
更多帮助和信息可以从[我的世界官方论坛](http://www.minecraftforum.net/topic/1314800-/)中查询。

## 贡献代码

如果您希望为我的世界控制台客户端出一份力的话，我们不胜感激，您可以fork此repo并提交合并请求。 *Indev* 分支将不会继续被使用, 我们将只会把MCC作为测试版软件发布。

## 许可证

除非有特殊说明，此项目代码全部来自MCC开发者，并以CDDL-1.0协议发布。
在其他情况下，许可证和原作者会被提及于源码文件的顶部。
CDDL-1.0许可证的主要条件基本上在列明于下列：

- 你可以在任何一个程序使用许可证编码不管是使用完整的或一部分，程序的许可证是处于完整（或者相当的，不包括你借用的编码）。程序本身可以使开放来源或是封闭来源，自由的或商业的。
- 无论如何，在CDDL编码（在CDDl编码里被任何编码引用直接修改会被认为是增建部分于CDDL编码里，所以是被限制于这需求；列子：对math fuction的改进使用快速查阅资料表会让资料表被认为是个增建部分，不管这是否在自己本身的来源编码之中）里，所有案列例如任何修改，改进，或者是增建部分必须使其公开的和自由的在来源中，当然也被限制于CDDL许可证里。
- 在任何程序（来源或二进制）使用CDDL编码，确认必须要被给于CDDl编码的来源（任何一个项目或作者）。同样的，对CDDL编码（必须分布作为来源）的改进不得移除作为指引来源编码的通知。

更多资讯在 http://qstuff.blogspot.fr/2007/04/why-cddl.html<br>
完整许可证在 http://opensource.org/licenses/CDDL-1.0
## English ver.
Minecraft Console Client
========================

[![Appveyor build status](https://ci.appveyor.com/api/projects/status/github/ORelio/Minecraft-Console-Client?branch=Indev)](https://ci.appveyor.com/project/ORelio/minecraft-console-client)

Minecraft Console Client(MCC) is a lightweight app allowing you to connect to any Minecraft server,
send commands and receive text messages in a fast and easy way without having to open the main Minecraft game. It also provides various automation for administration and other purposes.

## Looking for maintainers

Due to no longer having time to implement upgrades for new Minecraft versions and fixing bugs, I'm looking for motivated people to take over the project. If you feel like it could be you, please have a look at the [issues](https://github.com/ORelio/Minecraft-Console-Client/issues?q=is%3Aissue+is%3Aopen+label%3Awaiting-for%3Acontributor) section :)

## Download

Get exe file from the latest [development build](https://ci.appveyor.com/project/ORelio/minecraft-console-client/build/artifacts).
This exe file is a .NET binary which also works on Mac and Linux.

## How to use

Check out the [sample configuration files](MinecraftClient/config/) which includes the how-to-use README.
Help and more info is also available on the [Minecraft Forum thread](http://www.minecraftforum.net/topic/1314800-/).<br/>

## Building from source

First of all, get a [zip of source code](https://github.com/ORelio/Minecraft-Console-Client/archive/master.zip), extract it and navigate to the `MinecraftClient` folder.

Edit `MinecraftClient.csproj` to set the Build target to `Release` on [line 4](https://github.com/ORelio/Minecraft-Console-Client/blob/master/MinecraftClient/MinecraftClient.csproj#L4):

```xml
<Configuration Condition=" '$(Configuration)' == '' ">Release</Configuration>
```

### On Windows

1. Locate `MSBuild.exe` for .NET 4 inside `C:\Windows\Microsoft.NET\Framework\v4.X.XXXXX`
2. Drag and drop `MinecraftClient.csproj` over `MSBuild.exe` to launch the build
3. If the build succeeds, you can find `MinecraftClient.exe` under `MinecraftClient\bin\Release`

### On Mac and Linux

1. Install the [Mono Framework](https://www.mono-project.com/download/stable/#download-lin) if not already installed
2. Run `msbuild MinecraftClient.csproj` in a terminal
3. If the build succeeds, you can find `MinecraftClient.exe` under `MinecraftClient\bin\Release`

## How to contribute

If you'd like to contribute to Minecraft Console Client, great, just fork the repository and submit a pull request. The *Indev* branch for contributions to future stable versions is no longer used as MCC is currently distributed as development builds only.

## License

Unless specifically stated, the code is from the MCC developers, and available under CDDL-1.0.
Else, the license and original author are mentioned in source file headers.
The main terms of the CDDL-1.0 license are basically the following:

- You may use the licensed code in whole or in part in any program you desire, regardless of the license of the program as a whole (or rather, as excluding the code you are borrowing). The program itself may be open or closed source, free or commercial.
- However, in all cases, any modifications, improvements, or additions to the CDDL code (any code that is referenced in direct modifications to the CDDL code is considered an addition to the CDDL code, and so is bound by this requirement; e.g. a modification of a math function to use a fast lookup table makes that table itself an addition to the CDDL code, regardless of whether it's in a source code file of its own) must be made publicly and freely available in source, under the CDDL license itself.
- In any program (source or binary) that uses CDDL code, recognition must be given to the source (either project or author) of the CDDL code. As well, modifications to the CDDL code (which must be distributed as source) may not remove notices indicating the ancestry of the code.

More info at http://qstuff.blogspot.fr/2007/04/why-cddl.html
Full license at http://opensource.org/licenses/CDDL-1.0
