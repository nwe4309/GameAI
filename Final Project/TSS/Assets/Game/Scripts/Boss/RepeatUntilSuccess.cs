using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone.Enums
{
    public class RepeatUntilSuccess : Node
    {
        // Child node to evaluate
        private Node childNode;

        public Node Node
        {
            get { return childNode; }
        }

        public RepeatUntilSuccess(Node node)
        {
            childNode = node;
        }

        /// <summary>
        /// Continues to run child evaluate until success is returned
        /// </summary>
        /// <returns></returns>
        public override NodeStates Evaluate()
        {
            //while (currentNodeState != NodeStates.SUCCESS)
            //{
                switch (childNode.Evaluate())
                {
                    case NodeStates.FAILURE:
                        currentNodeState = NodeStates.FAILURE;
                        return currentNodeState;
                        break;

                    case NodeStates.SUCCESS:
                        currentNodeState = NodeStates.SUCCESS;
                        return currentNodeState;
                        break;

                    case NodeStates.RUNNING:
                        currentNodeState = NodeStates.RUNNING;
                        return currentNodeState;
                        break;
                }
            //}
            currentNodeState = NodeStates.SUCCESS;
            return currentNodeState;
        }
    }
}
