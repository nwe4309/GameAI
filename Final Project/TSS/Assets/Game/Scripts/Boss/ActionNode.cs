using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone.Enums
{
    public class ActionNode : Node
    {
        // Method signature for the action
        public delegate NodeStates ActionNodeDelegate();

        // The delegate that is called to evaluate this node
        private ActionNodeDelegate actionDelegate;

        /// <summary>
        /// This node contains no logic itself, so the logic must be passed in in the form of a delegate
        /// The action needs to return a NodeState enum
        /// </summary>
        /// <param name="action"></param>
        public ActionNode(ActionNodeDelegate action)
        {
            actionDelegate = action;
        }

        /// <summary>
        /// Evaluates the node using the passed in delegate and returns the resulting state as appropriate
        /// </summary>
        /// <returns></returns>
        public override NodeStates Evaluate()
        {
            switch (actionDelegate())
            {
                case NodeStates.FAILURE:
                    currentNodeState = NodeStates.FAILURE;
                    return currentNodeState;

                case NodeStates.SUCCESS:
                    currentNodeState = NodeStates.SUCCESS;
                    return currentNodeState;

                case NodeStates.RUNNING:
                    currentNodeState = NodeStates.RUNNING;
                    return currentNodeState;

                default:
                    currentNodeState = NodeStates.FAILURE;
                    return currentNodeState;
            }
        }
    }
}
