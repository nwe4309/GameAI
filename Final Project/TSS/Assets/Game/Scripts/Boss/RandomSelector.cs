using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone.Enums
{
    public class RandomSelector : Node
    {
        // Child nodes for this selector
        protected List<Node> childNodes = new List<Node>();

        private int randomIndex;

        public RandomSelector(List<Node> nodes)
        {
            childNodes = nodes;
            randomIndex = -1;
        }

        /// <summary>
        /// Pick a random node in the list and evaluate it
        /// Only pick a new random node if the children return success
        /// </summary>
        /// <returns></returns>
        public override NodeStates Evaluate()
        {
            if(randomIndex == -1)
            {
                randomIndex = Random.Range(0, childNodes.Count);
            }

            switch (childNodes[randomIndex].Evaluate())
            {
                case NodeStates.FAILURE:
                    currentNodeState = NodeStates.FAILURE;
                    return currentNodeState;

                case NodeStates.SUCCESS:
                    currentNodeState = NodeStates.SUCCESS;
                    randomIndex = -1;
                    return currentNodeState;

                case NodeStates.RUNNING:
                    currentNodeState = NodeStates.RUNNING;
                    return currentNodeState;
            }

            currentNodeState = NodeStates.FAILURE;
            return currentNodeState;
        }
    }
}
