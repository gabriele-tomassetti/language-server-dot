'use strict';

import * as path from 'path';
import * as fs from 'fs';

import { workspace, Disposable, ExtensionContext } from 'vscode';
import { LanguageClient, LanguageClientOptions, SettingMonitor, ServerOptions, TransportKind, TextEdit,
RequestType, TextDocumentIdentifier, ResponseError, InitializeError, State as ClientState, NotificationType } from 'vscode-languageclient';

export function activate(context: ExtensionContext) {

	// The server is implemented in another project and outputted there
	let serverModule = context.asAbsolutePath(path.join('server', 'server.js'));
	// The debug options for the server
	let debugOptions = { execArgv: ["--nolazy", "--debug=6009"] };
	
	// If the extension is launched in debug mode then the debug server options are used
	// Otherwise the  normal ones are used
	let serverOptions: ServerOptions = {
		run : { module: serverModule, transport: TransportKind.ipc },
		debug: { module: serverModule, transport: TransportKind.ipc, options: debugOptions }
	}
	
	// Options of the language client
	let clientOptions: LanguageClientOptions = {
		// Activate the server for DOT files
		documentSelector: ['dot'],
		synchronize: {
			// Synchronize the section 'dotLanguageServer' of the settings to the server
			configurationSection: 'dotLanguageServer',
			// Notify the server about file changes to '.clientrc files contained in the workspace
			fileEvents: workspace.createFileSystemWatcher('**/.clientrc')
		}
	}
	
	// Create the language client and start the client.
	let disposable = new LanguageClient('dotLanguageServer', 'Language Server', serverOptions, clientOptions).start();
	
	// Push the disposable to the context's subscriptions so that the 
	// client can be deactivated on extension deactivation
	context.subscriptions.push(disposable);
}