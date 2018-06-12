using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReboProject
{
    public class Node
    {
        public string Value;
        public int Level;
        public List<Node> Children = new List<Node>();
    }
}