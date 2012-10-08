using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Trigger
{
    public class Trigger : ICondition
    {
        public List<ICondition> Conditions = new List<ICondition>();
        public List<IAction> Actions = new List<IAction>();

        private int type;
        private bool inverted;
        private bool isOr;

        private bool isSatisfied;
        private bool hasBeenSatisfied;
        private bool isActivated;

        public void SetAndOr(bool isOr)
        {
            this.isOr = isOr;
        }

        public bool IsOr()
        {
            return isOr;
        }

        public bool IsInverted()
        {
            return inverted;
        }

        public void Update()
        {
            if (checkConditions())
            {
                isSatisfied = true;
                hasBeenSatisfied = true;
            }
            else
                isSatisfied = false;


            if (type == ConditionType.ONCE && hasBeenSatisfied && !isActivated)
            {
                performActions();
                isActivated = true;
            }
            if (type == ConditionType.SWITCH)
            {
                if (isSatisfied && !isActivated)
                {
                    performActions();
                    isActivated = true;
                }
                if (!isSatisfied && isActivated)
                {
                    resetActions();
                    isActivated = false;
                }

            }
        }

        private bool checkConditions()
        {
            if (!isOr) //AND
            {
                foreach (ICondition c in Conditions)
                {
                    if (!c.IsSatisfied())
                    {
                        return false;
                    }
                }
                return true;
            }
            
            //OR
            foreach (ICondition c in Conditions)
            {
                if (c.IsSatisfied())
                {
                    return true;
                }
            }
            return false;

        }

        private void performActions()
        {
            foreach (IAction a in Actions)
            {
                if (!inverted)
                    a.Activate();
                else
                    a.Reset();
            }
        }

        private void resetActions()
        {
            foreach (IAction a in Actions)
            {
                if (!inverted)
                    a.Reset();
                else
                    a.Activate();
            }
        }

        public bool IsSatisfied()
        {
            Update();

            if (type == ConditionType.ONCE && hasBeenSatisfied)
                return !inverted; //true in not-inverted case
            if (type == ConditionType.SWITCH && isSatisfied)
                return !inverted;

            return inverted; //false in not-inverted case
        }

        public void SetType(int conditionType)
        {
            type = conditionType;
        }

        public void Invert()
        {
            inverted = true;
        }
    
    }
}
