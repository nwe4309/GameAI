using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone.Enums
{
    [System.Serializable]
    public abstract class Node
    {
        // Delegate that returns the state of the node
        public delegate NodeStates NodeReturn();

        // The current state of the node
        protected NodeStates currentNodeState;

        public NodeStates NodeState
        {
            get { return currentNodeState; }
        }

        public Node()
        {

        }

        // Implementing classes use this method to evaluate the desired set of conditions
        public abstract NodeStates Evaluate();
    }
}
