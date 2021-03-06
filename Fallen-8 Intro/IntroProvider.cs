﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq;
using NoSQL.GraphDB;
using NoSQL.GraphDB.Helper;
using NoSQL.GraphDB.Model;

namespace Intro
{
	public class IntroProvider
	{
		private List<VertexModel> _toBeBenchenVertices = null;
		private int _numberOfToBeTestedVertices = 10000000;
		private Fallen8 _f8;

		public IntroProvider (Fallen8 fallen8)
		{
			_f8 = fallen8;
		}

		/// <summary>
		/// Creates a scale free network
		/// </summary>
		/// <param name="nodeCound"></param>
		/// <param name="edgeCount"></param>
		/// <param name="fallen8"></param>
		public void CreateScaleFreeNetwork (int nodeCound, int edgeCount)
		{
			var creationDate = DateHelper.ConvertDateTime (DateTime.Now);
			var vertexIDs = new List<Int32> ();
			var prng = new Random ();
			if (nodeCound < _numberOfToBeTestedVertices) {
				_numberOfToBeTestedVertices = nodeCound;
			}

			_toBeBenchenVertices = new List<VertexModel> (_numberOfToBeTestedVertices);

			for (var i = 0; i < nodeCound; i++) {
//                vertexIDs.Add(
//                    fallen8.CreateVertex(creationDate, new PropertyContainer[4]
//                                                           {
//                                                               new PropertyContainer { PropertyId = 23, Value = 4344 },
//                                                               new PropertyContainer { PropertyId = 24, Value = "Ein gaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaanz langes Property" },
//                                                               new PropertyContainer { PropertyId = 25, Value = "Ein kurzes Property" },
//                                                               new PropertyContainer { PropertyId = 26, Value = "Ein gaaaaaaaanz langes Property" },
//                                                           }).Id);
				vertexIDs.Add (_f8.CreateVertex (creationDate).Id);
                        
			}

			if (edgeCount != 0) {
				foreach (var aVertexId in vertexIDs) {
					var targetVertices = new HashSet<Int32> ();

					do {
						targetVertices.Add (vertexIDs [prng.Next (0, vertexIDs.Count)]);
					} while (targetVertices.Count < edgeCount);

					foreach (var aTargetVertex in targetVertices) {
//                    fallen8.CreateEdge(aVertexId, 0, aTargetVertex, creationDate, new PropertyContainer[2]
//                                                           {
//                                                               new PropertyContainer { PropertyId = 29, Value = 23.4 },
//                                                               new PropertyContainer { PropertyId = 1, Value = 2 },
//                                                           });
//
						_f8.CreateEdge (aVertexId, 0, aTargetVertex, creationDate);
					}
				}

				_toBeBenchenVertices.AddRange (PickInterestingIDs (vertexIDs, prng)
				.Select (aId => {
					VertexModel v = null;

					_f8.TryGetVertex (out v, aId);

					return v;
				}));
			}
		}

		IEnumerable<int> PickInterestingIDs (List<int> vertexIDs, Random prng)
		{
			for (int i = 0; i < _numberOfToBeTestedVertices; i++) {
				yield return vertexIDs [prng.Next (0, vertexIDs.Count)];
			}
		}

		/// <summary>
		/// Benchmark
		/// </summary>
		/// <param name="fallen8"></param>
		/// <param name="myIterations"></param>
		/// <returns></returns>
		public String Bench (int myIterations = 1000)
		{
			if (_toBeBenchenVertices == null) {
				return "No vertices available";
			}

			List<VertexModel> vertices = _toBeBenchenVertices;
			var tps = new List<double> ();
			long edgeCount = 0;
			var sb = new StringBuilder ();
			
			Int32 range = ((vertices.Count / Environment.ProcessorCount) * 3) / 2;
			
			for (var i = 0; i < myIterations; i++) {
				var sw = Stopwatch.StartNew ();

				edgeCount = CountAllEdgesParallelPartitioner (vertices, range);

				sw.Stop ();

				tps.Add (edgeCount / sw.Elapsed.TotalSeconds);
			}

			sb.AppendLine (String.Format ("Traversed {0} edges. Average: {1}TPS Median: {2}TPS StandardDeviation {3}TPS ", edgeCount, Statistics.Average (tps), Statistics.Median (tps), Statistics.StandardDeviation (tps)));

			return sb.ToString ();
		}

		/// <summary>
		/// Counter
		/// </summary>
		/// <param name="vertices"></param>
		/// <param name="vertexRange"></param>
		/// <returns></returns>
		private static long CountAllEdgesParallelPartitioner (List<VertexModel> vertices, Int32 vertexRange)
		{
			var lockObject = new object ();
			var edgeCount = 0L;
			var rangePartitioner = Partitioner.Create (0, vertices.Count, vertexRange);

			Parallel.ForEach (
				rangePartitioner,
				() => 0L,
				delegate(Tuple<int, int> range, ParallelLoopState loopstate, long initialValue) {
					var localCount = initialValue;

					for (var i = range.Item1; i < range.Item2; i++) {
						ReadOnlyCollection<EdgeModel> outEdge;
						if (vertices [i].TryGetOutEdge (out outEdge, 0)) {
							for (int j = 0; j < outEdge.Count; j++) {
								var vertex = outEdge [j].TargetVertex;
								localCount++;
							}
						}
					}

					return localCount;
				},
				delegate(long localSum) {
					lock (lockObject) {
						edgeCount += localSum;
					}
				});

			return edgeCount;
		}
	}
}
