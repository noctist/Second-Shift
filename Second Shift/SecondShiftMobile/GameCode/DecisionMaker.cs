using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile
{
    public class Decision
    {
        public Action DecisionAction;
        public Func<bool> NecessaryCondition;
        static Random rand = new Random();
        float timer = 0, decisionTime;
        List<Func<bool>> decisionHelpers = new List<Func<bool>>();
        List<float> decisionHelpersWeights = new List<float>();
        float probability;
        public Decision(float decisionTime, float probability)
        {
            this.decisionTime = decisionTime;
            this.probability = MathHelper.Clamp(probability, 0, 1);
        }
        public void Update(float playSpeed)
        {
            timer += playSpeed;
            if (timer >= decisionTime)
                Decide();
        }

        public void AddDecisionHelper(float weight, Func<bool> decision)
        {
            decisionHelpersWeights.Add(weight);
            decisionHelpers.Add(decision);
        }

        public bool Decide()
        {
            if (timer < decisionTime)
                return false;
            if (DecisionAction == null)
                return false;
            if (decisionHelpers.Count == 0)
                return false;
            if (NecessaryCondition != null && !NecessaryCondition())
            {
                return false;
            }
            float totalWeight = 0, currentWeight = 0;
            for (int i = 0; i < decisionHelpers.Count; i++)
            {
                var dweight = decisionHelpersWeights[i];
                var d = decisionHelpers[i];
                totalWeight += dweight;
                if (d())
                {
                    currentWeight += dweight;
                }
            }
            float finalWeight = currentWeight / totalWeight;
            float decision = (float)rand.NextDouble();
            float prob = 1 - probability;
            if (finalWeight > 0.5)
            {
                if (decision >= prob)
                {
                    timer -= decisionTime;
                    DecisionAction();
                    return true;
                }
                else
                {
                    timer -= decisionTime * finalWeight;
                    return false;
                }
            }
            else
            {
                timer -= decisionTime * (1 - finalWeight);
                return false;
            }
        }
    }
}
