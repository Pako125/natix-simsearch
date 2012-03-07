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
//   Original filename: natix/natix-simsearch/HistKnr.cs
// 
using System;
using natix;
using natix.SimilaritySearch;
using NDesk.Options;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace natixsimsearch
{
	public class HistKnr
	{
		public HistKnr ()
		{
		}
	
		public static void FromKnrCosine (IEnumerable<string> args)
		{
			OptionSet op;
			string indexname = null;
			int round = 3;
			string basename = null;
			op = new OptionSet () {
				{ "index=", "KnrCosine index (Full knr with distances)", v => indexname = v},
				{ "round=", "Round distances to this number of decimal digits", v => round = int.Parse(v)},
				{ "outbase|basename=", "Base name to output files", v =>  basename = v}
			};
			
			/*List<string> argsList = */
			op.Parse(args);
			if (indexname == null || basename == null) {
				Console.WriteLine("Values are mandatory => indexname: {0}, basename: {1}",
					indexname, basename);
				op.WriteOptionDescriptions(Console.Out);
			}
			
			if (File.Exists(String.Format("{0}-pos0", basename))) {
				Console.WriteLine("Skipping {0}, already exists", basename);
				return;
			}
			var histogram_by_knr_position = new Dictionary<int, Dictionary<float, int>>();
			var index = (KnrCosineBase) IndexLoader.Load(indexname);
			var docspace = index.GetDocumentSpace();
			var L = new List<Tvoc>();
			var percent = docspace.Count / 100 + 1;
			for (int docid = 0, len = docspace.Count; docid < len; docid++) {
				L.Clear();
				var doc = docspace[docid].doc;
				if (docid % percent == 0) {
					Console.WriteLine("*** docid: {0}, advance: {1:0.00}%", docid, docid * 100.0 / len);
				}
				for (int i = 0; i < doc.Length; i++) {
					L.Add(doc[i]);
				}
				L.Sort( (Tvoc x, Tvoc y) => x.weight.CompareTo(y.weight) );
				for (int i = 0; i < L.Count; i++) {
					Dictionary<float, int> V; 
					var D = L[i];
					if (!histogram_by_knr_position.TryGetValue(i, out V)) {
						V = new Dictionary<float, int>();
						histogram_by_knr_position[i] = V;
					}
					int counter;
					var w = (float)Math.Round(D.weight, round);
					if (!V.TryGetValue(w, out counter)) {
						counter = 0;
					}
					counter++;
					V[w] = counter;
				}
			}
			for (int histid = 0; histid < histogram_by_knr_position.Count; histid++) {
				string name = String.Format("{0}-pos{1}", basename, histid);
				Console.WriteLine("==> Processing: {0}", name);
				using (var F = new StreamWriter(File.Create(name))) {
					var H = histogram_by_knr_position[histid];
					var K = new List<float>(H.Keys);
					K.Sort();
					for (int ik = 0; ik < K.Count; ik++) {
						var key = K[ik];
						F.WriteLine("{0} {1}", key, H[key]);
					}
				}
			}
			Console.WriteLine("done");
		}
	}
}
