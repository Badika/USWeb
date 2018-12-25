using System;
using System.Collections.Generic;

namespace UpsCoolWeb.Components.Extensions
{
    public class MvcTreeNode
    {
        public Int32? Id { get; set; }
        public String Title { get; set; }
        public List<MvcTreeNode> Children { get; set; }

        public MvcTreeNode(Int32? id, String title)
        {
            Id = id;
            Title = title;
            Children = new List<MvcTreeNode>();
        }
        public MvcTreeNode(String title)
            : this(null, title)
        {
        }
    }
}
