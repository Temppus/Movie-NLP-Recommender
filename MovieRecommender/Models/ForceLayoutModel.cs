using MovieRecommender.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace MovieRecommender.Models
{
    public class ForceLayoutModel
    {
        public ForceLayoutModel()
        {
            nodes = new List<Node>();
            edges = new List<Edge>();
        }

        public List<Node> nodes { get; set; }
        public List<Edge> edges { get; set; }

        public ForceLayoutModel Compose(IEnumerable<MovieMention> movieMentions)
        {
            string labelSeenText = "liked movie";
            string labelMentionText = "contains positive mention to";

            int nodeNum = 0;

            var coreNode = new Node() { name = "You" , OrderId = nodeNum++};
            nodes.Add(coreNode);

            var fromGroupings = movieMentions.GroupBy(m => m.FromIMDBId);

            foreach (var grouping in fromGroupings)
            {
                MovieMention firstMention = grouping.First();

                var seenNode = new Node()
                {
                    OrderId = nodeNum++,
                    name = firstMention.MovieName
                };

                nodes.Add(seenNode);

                var mentionedNode = new Node()
                {
                    OrderId = nodeNum++,
                    name = firstMention.MovieName
                };

                nodes.Add(mentionedNode);

                var coreToSeenLink = new Edge() { source = coreNode.OrderId, target = seenNode.OrderId, label = labelSeenText };
                var seenToMentionedLink = new Edge() { source = seenNode.OrderId, target = mentionedNode.OrderId, label = labelMentionText };

                edges.Add(coreToSeenLink);
                edges.Add(seenToMentionedLink);

                foreach (var item in grouping.Skip(1))
                {
                    var gmentioneNode = new Node()
                    {
                        OrderId = nodeNum++,
                        name = item.MovieName
                    };

                    var gSeenToMentionLink = new Edge() { source = seenNode.OrderId, target = gmentioneNode.OrderId, label = labelMentionText };

                    nodes.Add(gmentioneNode);
                    edges.Add(gSeenToMentionLink);
                }
            }

            return this;
        }

        public string ToJson()
        {
            return new JavaScriptSerializer().Serialize(this);
        }
    }

    public class Node
    {
        public string name { get; set; }

        [ScriptIgnore]
        public int OrderId { get; set; }
    }

    public class Edge
    {
        public int source { get; set; }

        public int target { get; set; }

        public string label { get; set; }
    }
}