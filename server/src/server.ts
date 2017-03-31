'use strict';

import {
	IPCMessageReader, IPCMessageWriter,
	createConnection, IConnection, TextDocumentSyncKind,
	TextDocuments, TextDocument, Diagnostic, DiagnosticSeverity,
	InitializeParams, InitializeResult, TextDocumentPositionParams,
	CompletionItem, CompletionItemKind,
	Hover, Files
} from 'vscode-languageserver';

import * as fs from 'fs';
import * as path from 'path';

// Create a connection for the server. The connection uses Node's IPC as a transport
let connection: IConnection = createConnection(new IPCMessageReader(process), new IPCMessageWriter(process));

// Listen on the connection
connection.listen();

// Create a simple text document manager. The text document manager
// supports full document sync only
let documents: TextDocuments = new TextDocuments();
// Make the text document manager listen on the connection
// for open, change and close text document events
documents.listen(connection);

// After the server has started the client sends an initialize request. The server receives
// in the passed params the rootPath of the workspace plus the client capabilities. 
let workspaceRoot: string;

// The settings interface describe the server relevant settings part
interface Settings {
	dotLanguageServer: DotSettings;
}

// These are the example settings we defined in the client's package.json
// file
interface DotSettings {
	maxNumberOfProblems: number;
}

// hold the maxNumberOfProblems setting
let maxNumberOfProblems: number;

// hold a list of colors and shapes for the completion provider
let colors: Array<string>;
let shapes: Array<string>;

connection.onInitialize((params): InitializeResult => {	
	workspaceRoot = params.rootPath;
	colors = new Array<string>();
	shapes = new Array<string>();

	return {		
		capabilities: {
			// Tell the client that the server works in FULL text document sync mode
			textDocumentSync: documents.syncKind,
			// Tell the client that the server support code complete
			completionProvider: {
				resolveProvider: true,
				"triggerCharacters": [ '=' ]
			},
			hoverProvider: true			
		}
	}
});

// The settings have changed. Is send on server activation
// as well.
connection.onDidChangeConfiguration((change) => {
	let settings = <Settings>change.settings;
	maxNumberOfProblems = settings.dotLanguageServer.maxNumberOfProblems || 100;
	// Revalidate any open text documents
	documents.all().forEach(validateDotDocument);
})

// The content of a text document has changed. This event is emitted
// when the text document first opened or when its content has changed.
documents.onDidChangeContent((change) => {
	validateDotDocument(change.document);
});;

var request = require('request');

let names: Array<any>;

function validateDotDocument(textDocument: TextDocument): void {
	let diagnostics: Diagnostic[] = [];	
    	
	request.post({url:'http://localhost:3000/parse', body: textDocument.getText()}, function optionalCallback(err, httpResponse, body) {  		  		
		let messages = JSON.parse(body).errors;
		names = JSON.parse(body).names;
		
		let lines = textDocument.getText().split(/\r?\n/g);
		let problems = 0;		

		for (var i = 0; i < messages.length && problems < maxNumberOfProblems; i++) {		
			problems++;
			
			if(messages[i].length == 0)
				messages[i].length = lines[i].length - messages[i].character;

			diagnostics.push({
				severity: DiagnosticSeverity.Error,
				range: {
					start: { line: messages[i].line, character: messages[i].character},
					end: { line: messages[i].line, character: messages[i].character + messages[i].length }
				},
				message: messages[i].message,
				source: 'ex'
			});			
		}
		// Send the computed diagnostics to VSCode.
		connection.sendDiagnostics({ uri: textDocument.uri, diagnostics });
	});	
}

connection.onCompletion((textDocumentPosition: TextDocumentPositionParams): CompletionItem[] => {
	let text = documents.get(textDocumentPosition.textDocument.uri).getText();	
	let lines = text.split(/\r?\n/g);	
	let position = textDocumentPosition.position;		

	if(colors.length == 0)
		colors=loadColors();

	if(shapes.length == 0)
		shapes=loadShapes();

	let start = 0;

	for (var i = position.character; i >= 0; i--) {
		if(lines[position.line][i] == '=')
		{			
			start = i;
			i = 0;
		}
	}		

	if(start >= 5
	&& lines[position.line].substr(start-5,5) == "color")
	 {
		let results = new Array<CompletionItem>();
		for(var a = 0; a < colors.length; a++)
		{
			results.push({ 
				label: colors[a],
				kind: CompletionItemKind.Color,
				data: 'color-' + a
			})
		}
		
		return results;
	}

	if(start >= 5
	&& lines[position.line].substr(start-5,5) == "shape")
	 {
		let results = new Array<CompletionItem>();
		for(var a = 0; a < shapes.length; a++)
		{
			results.push({ 
				label: shapes[a],
				kind: CompletionItemKind.Text,
				data: 'shape-' + a
			})
		}
		
		return results;
	}
});

connection.onCompletionResolve((item: CompletionItem): CompletionItem => {
	if(item.data.startsWith('color-'))
	{
		item.detail = 'X11 Color';
		item.documentation = 'http://www.graphviz.org/doc/info/colors.html';		
	}

	if(item.data.startsWith('shape-'))
	{
		item.detail = 'Shape';
		item.documentation = 'http://www.graphviz.org/doc/info/shapes.html';
	}
	
	return item;
});

connection.onHover(({ textDocument, position }): Hover => {    		
	for(var i = 0; i < names.length; i++)
	{		
		if(names[i].line == position.line
		   && (names[i].start <= position.character && names[i].end >= position.character) )
		{
			// we return an answer only if we find something
			// otherwise no hover information is given
			return {
				contents: names[i].text
			};
		}
	}	
});

function fromUri(document: { uri: string; }) {
    return Files.uriToFilePath(document.uri);
}

function loadColors() : Array<string>
{	
	let colorsFile = fs.readFileSync(path.join(__dirname, '..', '..', 'data', 'colors')).toString();
	let colors = colorsFile.split(/\r?\n/g);		

	return colors;
}

function loadShapes() : Array<string>
{			
	let shapesFile = fs.readFileSync(path.join(__dirname, '..',  '..', 'data', 'shapes')).toString();
	let shapes = shapesFile.split(/\r?\n/g);		

	return shapes;
}