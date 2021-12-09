using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone.Enums
{
    public class Sequence : Node
    {
        // Child nodes for this sequence
        protected List<Node> childNodes = new List<Node>();

        public Sequence(List<Node> nodes)
        {
            childNodes = nodes;
        }

        /// <summary>
        /// If any of the child nodes return a failure, the entire node fails
        /// Once all nodes return a success, the nodes reports a success
        /// </summary>
        /// <returns></returns>
        public override NodeStates Evaluate()
        {
            bool anyChildRunning = false;

            foreach (Node node in childNodes)
            {
                switch (node.Evaluate())
                {
                    case NodeStates.FAILURE:
                        currentNodeState = NodeStates.FAILURE;
                        return currentNodeState;

                    case NodeStates.SUCCESS:
                        continue;

                    case NodeStates.RUNNING:
                        anyChildRunning = true;
                        continue;

                    default:
                        currentNodeState = NodeStates.SUCCESS;
                        return currentNodeState;
                }
            }
            currentNodeState = anyChildRunning ? NodeStates.RUNNING : NodeStates.SUCCESS;
            return currentNodeState;
        }
    }
}
