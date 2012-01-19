using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fallen8.API;
using System.Threading.Tasks;
using Fallen8.API.Helper;
using Fallen8.Model;

namespace Intro
{
    class IntroProvider
    {
        private Fallen8.API.Fallen8 _fallen8;
        private Int32 _edgePropertyId;

        public IntroProvider(Fallen8.API.Fallen8 fallen8)
        {
            _fallen8 = fallen8;
            _edgePropertyId = 0;
        }

        internal void CreateScaleFreeNetwork(int nodeCound, int edgeCount)
        {
            DateTime creationDate = DateTime.Now;
            List<Int32> vertexIDs = new List<Int32>();
            Random prng = new Random();

            for (int i = 0; i < nodeCound; i++)
            {
                vertexIDs.Add(
                    _fallen8.CreateVertex(creationDate, new Dictionary<int,object> 
                    {
                        {23, "Name" + i}
                    }).Id);
                        
            }

            foreach (var aVertexId in vertexIDs)
            {
                HashSet<Int32> targetVertices = new HashSet<Int32>();

                do
                {
                    targetVertices.Add(vertexIDs[prng.Next(0, vertexIDs.Count)]);
                } while (targetVertices.Count < edgeCount);

                foreach (var aTargetVertex in targetVertices)
                {
                    _fallen8.CreateEdge(aVertexId, _edgePropertyId, new EdgeModelDefinition(aTargetVertex, creationDate, new Dictionary<int, object> { { 42, aTargetVertex % 42 } }));
                }
            }
        }

        internal void TraverseABit()
        {
            var vertex = _fallen8.GetVertices().First();


            #region out edges

            EdgePropertyModel edgeProperty;
            if (vertex.TryGetOutEdge(out edgeProperty, _edgePropertyId))
            {
                foreach (var aTargetVertex in edgeProperty.Select(_ => _.TargetVertex))
                {
                    // target vertices
                }

                foreach (var aEdge in edgeProperty)
                {
                    // edge
                }
            } 
            
            #endregion

            #region inc edges

            IEnumerable<EdgeModel> incomingEdges;
            if (vertex.TryGetInEdges(out incomingEdges, _edgePropertyId))
            {
                foreach (var aIncomingVertex in incomingEdges.Select(_ => _.SourceEdgeProperty.SourceVertex))
                {
                    // incoming vertices
                }

                foreach (var aIncomingVertex in incomingEdges.Select(_ => _.SourceEdgeProperty).SelectMany(__ => __))
                {
                    // outgoing vertices of the incoming vertices
                }
            }
            
            #endregion
        }
    }
}
