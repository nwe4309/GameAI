using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone.Enums
{
    public class Inverter : Node
    {
        // Child node to evaluate
        private Node childNode;

        public Node Node
        {
            get { return childNode; }
        }

        public Inverter(Node node)
        {
            childNode = node;
        }

        /// <summary>
        /// Reports a success if the child fails and a failure if the child succeeds
        /// Running will report as running
        /// </summary>
        /// <returns></returns>
        public override NodeStates Evaluate()
        {
            switch(childNode.Evaluate())
            {
                case NodeStates.FAILURE:
                    currentNodeState = NodeStates.SUCCESS;
                    return currentNodeState;

                case NodeStates.SUCCESS:
                    currentNodeState = NodeStates.FAILURE;
                    return currentNodeState;

                case NodeStates.RUNNING:
                    currentNodeState = NodeStates.RUNNING;
                    return currentNodeState;
            }

            currentNodeState = NodeStates.SUCCESS;
            return currentNodeState;
        }
    }
}
