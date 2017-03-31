# A Language Server For DOT With Visual Studio Code

This is the companion repository of the [A Language Server For DOT With Visual Studio Code](https://tomassetti.me/a-language-server-for-dot-with-visual-studio-code/), a tutorial that will explain how to create a server and a client for the Language Server Protocol.

The client will be created for Visual Studio Code, while the server will be created with Visual Studio Code, but could work with any client that supports the Language Server Protocol.

To use the code you have to enter in the folders ´client´ and ´server´ and install node packages:

´´´bash
npm install
´´´

and you have to enter ´csharp´ and restore the nuget packages:

´´´bash
dotnet restore
´´´

This code is based upon the example provided by Microsoft: [Sample language server implemented in Node](https://github.com/Microsoft/vscode-languageserver-node-example).
