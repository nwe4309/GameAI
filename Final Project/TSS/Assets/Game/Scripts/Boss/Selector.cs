using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone.Enums
{
    public class Selector : Node
    {
        // Child nodes for this selector
        protected List<Node> childNodes = new List<Node>();

        public Selector(List<Node> nodes)
        {
            childNodes = nodes;
        }

        /// <summary>
        /// If any of the children report success, then the selector will immediately send a success upwards in the tree
        /// If all children fail, it will report a failure instead
        /// </summary>
        /// <returns></returns>
        public override NodeStates Evaluate()
        {
            foreach(Node node in childNodes)
            {
                switch(node.Evaluate())
                {
                    case NodeStates.FAILURE:
                        continue;

                    case NodeStates.SUCCESS:
                        currentNodeState = NodeStates.SUCCESS;
                        return currentNodeState;

                    case NodeStates.RUNNING:
                        currentNodeState = NodeStates.RUNNING;
                        return currentNodeState;

                    default:
                        continue;
                }
            }
            currentNodeState = NodeStates.FAILURE;
            return currentNodeState;
        }
    }
}
