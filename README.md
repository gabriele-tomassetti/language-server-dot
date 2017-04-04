# A Language Server For DOT With Visual Studio Code

This is the companion repository of the [A Language Server For DOT With Visual Studio Code](https://tomassetti.me/a-language-server-for-dot-with-visual-studio-code/), a tutorial that will explain how to create a server and a client for the Language Server Protocol.

The client will be created for Visual Studio Code, while the server will be created with Visual Studio Code, but could work with any client that supports the Language Server Protocol.

To use the project you have to enter in the folders `client` and `server` and install node packages:

```bash
npm install
```

and you have to enter `csharp` and restore the nuget packages:

```bash
dotnet restore
```

Before launching the client (which is the proper VS Code extension) you have to compile the server. The server will be outputted under a `server` folder inside the client. You also have to leave running the .NET Core project under the `csharp` folder.

```bash
dotnet run
```

So while you are developing you have to open three Visual Studio Code instances:

- one for the folder `client`
- one for the folder `server`
- one for the folder `csharp`

If you want to simply run the project you can just start the .NET Core one and the client. You can start the .NET Core project outside Visual Studio Code, using the command line. But the client is an extension, so it has to be run inside Visual Studio Code.

This code is based upon the example provided by Microsoft: [Sample language server implemented in Node](https://github.com/Microsoft/vscode-languageserver-node-example).
