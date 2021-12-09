using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone.Enums
{
    public class RandomWeighted : Node
    {
        // Child nodes for this selector
        protected List<Node> childNodes = new List<Node>();
        private List<Vector2> weights = new List<Vector2>();

        private int randomIndex;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="weightRanges">Vectors must be lower bounds, upper bounds. As well, the list must add up to 100</param>
        public RandomWeighted(List<Node> nodes, List<Vector2> weightRanges)
        {
            childNodes = nodes;
            weights = weightRanges;
            randomIndex = -1;
        }

        /// <summary>
        /// Pick a random node in the list and evaluate it
        /// Only pick a new random node if the children return success
        /// </summary>
        /// <returns></returns>
        public override NodeStates Evaluate()
        {
            if (randomIndex == -1)
            {
                // Pick a random number between 1 and 100 and find what node has weights that belong to it
                // Example, RandNum = 31, Ranges = {1-25, 26-50, 51-100}, then index/node to run = 1
                float randNum = Random.Range(1f, 101f);

                for(int i = 0; i < weights.Count; i ++)
                {
                    if(randNum >= weights[i].x && randNum < weights[i].y)
                    {
                        randomIndex = i;
                        break;
                    }
                }

                Debug.Log(randNum + " : " + randomIndex);
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
