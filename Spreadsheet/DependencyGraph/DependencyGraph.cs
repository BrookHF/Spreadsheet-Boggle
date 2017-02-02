// Skeleton implementation written by Joe Zachary for CS 3500, January 2017.

using System;
using System.Collections.Generic;

namespace Dependencies
{
    /// <summary>
    /// A DependencyGraph can be modeled as a set of dependencies, where a dependency is an ordered 
    /// pair of strings.  Two dependencies (s1,t1) and (s2,t2) are considered equal if and only if 
    /// s1 equals s2 and t1 equals t2.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that the dependency (s,t) is in DG 
    ///    is called the dependents of s, which we will denote as dependents(s).
    ///        
    ///    (2) If t is a string, the set of all strings s such that the dependency (s,t) is in DG 
    ///    is called the dependees of t, which we will denote as dependees(t).
    ///    
    /// The notations dependents(s) and dependees(s) are used in the specification of the methods of this class.
    ///
    /// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    ///     dependents("a") = {"b", "c"}
    ///     dependents("b") = {"d"}
    ///     dependents("c") = {}
    ///     dependents("d") = {"d"}
    ///     dependees("a") = {}
    ///     dependees("b") = {"a"}
    ///     dependees("c") = {"a"}
    ///     dependees("d") = {"b", "d"}
    ///     
    /// All of the methods below require their string parameters to be non-null.  This means that 
    /// the behavior of the method is undefined when a string parameter is null.  
    ///
    /// IMPORTANT IMPLEMENTATION NOTE
    /// 
    /// The simplest way to describe a DependencyGraph and its methods is as a set of dependencies, 
    /// as discussed above.
    /// 
    /// However, physically representing a DependencyGraph as, say, a set of ordered pairs will not
    /// yield an acceptably efficient representation.  DO NOT USE SUCH A REPRESENTATION.
    /// 
    /// You'll need to be more clever than that.  Design a representation that is both easy to work
    /// with as well acceptably efficient according to the guidelines in the PS3 writeup. Some of
    /// the test cases with which you will be graded will create massive DependencyGraphs.  If you
    /// build an inefficient DependencyGraph this week, you will be regretting it for the next month.
    /// </summary>
    public class DependencyGraph
    {

        /// <summary>
        /// The DependencyGraphNode is a vertax class that represent one variable. 
        /// It contains one verable's dependents and dependees.
        /// The dependents and dependees are represented by points inorder to save memery space, and 
        /// making it easier to modify dependees and dependents because they are stored in different
        /// node.
        /// </summary>

        private class DependencyGraphNode
        {
            // variable's name
            public string name { get;}

            // variable's dependents
            public List<DependencyGraphNode> dependent { get; }

            // vairable's dependees
            public List<DependencyGraphNode> dependee { get; }

            // default constructor
            public DependencyGraphNode(string name)
            {
                this.name = name;
                dependee = new List<DependencyGraphNode>();
                dependent = new List<DependencyGraphNode>();
            }

        }

        /// <summary>
        /// DependencyGraphNodes are stored in dirctonary dependencyGraph in order to have constant
        /// access time when the number of dependency pairs is large.
        /// </summary>
        private Dictionary<string, DependencyGraphNode> dependencyGraph;

        public int size { get; set; }

        /// <summary>
        /// Creates a DependencyGraph containing no dependencies.
        /// </summary>
        public DependencyGraph()
        {
            dependencyGraph = new Dictionary<string, DependencyGraphNode>();
        }

        /// <summary>
        /// The number of dependencies in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return this.size; }
        }

        /// <summary>
        /// Reports whether dependents(s) is non-empty.  Requires s != null.
        /// </summary>
        public bool HasDependents(string s)
        {
            // return false if s does not exist or it does not have any dependent
            return dependencyGraph.ContainsKey(s) && dependencyGraph[s].dependent.Count != 0;
        }

        /// <summary>
        /// Reports whether dependees(s) is non-empty.  Requires s != null.
        /// </summary>
        public bool HasDependees(string s)
        {
            // return false if s does not exist or it does not have any dependee
            return dependencyGraph.ContainsKey(s) && dependencyGraph[s].dependee.Count != 0;
        }

        /// <summary>
        /// Enumerates dependents(s).  Requires s != null.
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            if (!dependencyGraph.ContainsKey(s)) { yield break; }
            foreach(DependencyGraphNode node in dependencyGraph[s].dependent)
            {
                yield return node.name;
            }
        }

        /// <summary>
        /// Enumerates dependees(s).  Requires s != null.
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            if (!dependencyGraph.ContainsKey(s)) { yield break; }
            foreach (DependencyGraphNode node in dependencyGraph[s].dependee)
            {
                yield return node.name;
            }
        }

        /// <summary>
        /// Adds the dependency (s,t) to this DependencyGraph.
        /// This has no effect if (s,t) already belongs to this DependencyGraph.
        /// Requires s != null and t != null.
        /// </summary>
        public void AddDependency(string s, string t)
        {
            if (!dependencyGraph.ContainsKey(s)) { dependencyGraph.Add(s, new DependencyGraphNode(s)); }
            if (!dependencyGraph.ContainsKey(t)) { dependencyGraph.Add(t, new DependencyGraphNode(t)); }
            dependencyGraph[s].dependent.Add(dependencyGraph[t]);
            dependencyGraph[t].dependee.Add(dependencyGraph[s]);
            // increaement size
            this.size++;
        }

        /// <summary>
        /// Removes the dependency (s,t) from this DependencyGraph.
        /// Does nothing if (s,t) doesn't belong to this DependencyGraph.
        /// Requires s != null and t != null.
        /// </summary>
        public void RemoveDependency(string s, string t)
        {
            if(dependencyGraph.ContainsKey(s) && dependencyGraph.ContainsKey(t))
            {
                dependencyGraph[t].dependee.Remove(dependencyGraph[s]);
                dependencyGraph[s].dependent.Remove(dependencyGraph[t]);
                // decreaement of size
                this.size--;
            }
        }

        /// <summary>
        /// Removes all existing dependencies of the form (s,r).  Then, for each
        /// t in newDependents, adds the dependency (s,t).
        /// Requires s != null and t != null.
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            // delete dependency
            foreach (DependencyGraphNode node in dependencyGraph[s].dependent)
            {
                this.RemoveDependency(s, node.name);
            }

            // add new dependency (s,t)
            foreach(string str in newDependents)
            {
                AddDependency(s,str);
            }
        }

        /// <summary>
        /// Removes all existing dependencies of the form (r,t).  Then, for each 
        /// s in newDependees, adds the dependency (s,t).
        /// Requires s != null and t != null.
        /// </summary>
        public void ReplaceDependees(string t, IEnumerable<string> newDependees)
        {
            // delete dependency from r
            foreach (DependencyGraphNode node in dependencyGraph[t].dependee)
            {
                this.RemoveDependency(node.name, t);
            }

            // add new dependency (s,t)
            foreach (string str in newDependees)
            {
                AddDependency(str, t);
            }
        }
    }
}
