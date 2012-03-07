//
//   Copyright 2012 Eric Sadit Tellez <sadit@dep.fie.umich.mx>
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//
//   Original filename: natix/natix-simsearch/Main.cs
// 
using System;
using System.IO;
using natix;
using natix.SimilaritySearch;

using System.Collections;
using System.Collections.Generic;
using NDesk.Options;

namespace natixsimsearch
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			if (Directory.Exists ("plugins")) {
				PluginManager plugins = new PluginManager ();
				plugins.LoadPluginDirectory ("plugins");
			}
			List<string> argsList = new List<string> ();
			foreach (string arg in args) {
				if (arg == "--") {
					RunCommand (argsList);
					argsList.Clear ();
				} else {
					argsList.Add (arg);
				}
			}
			RunCommand (argsList);
		}
		
		public static void RunCommand(List<string> args)
		{
			Console.WriteLine("============== Argument list: ");
			foreach (string s in args) {
				Console.Write(s+" ");
			}
			Console.WriteLine();
			string command = null;
			OptionSet op;
			op = new OptionSet () {
				{ "subspace|ss", "Extracts a subspace", v => command = "subspace" },
				{ "search|s", "Searching in an index", v => command = "search" },
				{ "build|b", "Build an index", v => command = "build" },
				{ "check|c", "Verify results using a basis result set", v => command = "check"},
				{ "hist|H", "Display the histogram of distances for each given result-file", v => command = "hist"},
				{ "histknr", "Save the histogram of distances of KNR", v => command = "histknr"},
				{ "pass", "Do nothing", v => command = "pass"},
				{ "help|h", "Show this help", v => command = "help"}
			};
			List<string> argsList = op.Parse(args);
			if (command == null || command == "help") {
				Console.WriteLine("Usage: {0} --command|-short-command --options", Environment.GetCommandLineArgs()[0]);
				Console.WriteLine("Where --command|-short-command could be");
				op.WriteOptionDescriptions(Console.Out);
				return;
			}
			switch (command) {
			case "histknr":
				HistKnr.FromKnrCosine(argsList);
				break;
			case "hist":
				Commands.Hist(argsList);
				break;
			case "build":
				Commands.Build (argsList);
				break;
			case "search":
				Commands.Search (argsList);
				break;
			case "subspace":
				Commands.SubSpace(argsList);
				break;
			case "check":
				Commands.Check(argsList);
				break;
			case "pass":
				break;
			default:
				throw new ArgumentException(String.Format("Unknown command '{0}'", command));
			}
		}
	}
}
