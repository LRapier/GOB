using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal
{
    public string name;
    public float value;

    public Goal(string goalName, float goalValue)
    {
        name = goalName;
        value = goalValue;
    }

    public float GetDiscontentment(float newValue)
    {
        return newValue * newValue;
    }
}

public class Action
{
    public string name;
    public List<Goal> targetGoals;

    public Action(string actionName)
    {
        name = actionName;
        targetGoals = new List<Goal>();
    }

    public float GetGoalChange(Goal goal)
    {
        foreach (Goal target in targetGoals)
        {
            if (target.name == goal.name)
            {
                return target.value;
            }
        }
        return 0f;
    }
}

public class Player : MonoBehaviour
{
    Goal[] mGoals;
    Action[] mActions;
    Action mChangeOverTime;
    float ticks = 0f;
    const float TICK_LENGTH = 5.0f;

    void Start()
    {
        mGoals = new Goal[3];
        mGoals[0] = new Goal("Eat", 4);
        mGoals[1] = new Goal("Sleep", 3);
        mGoals[2] = new Goal("Pee", 3);

        mActions = new Action[6];
        mActions[0] = new Action("eat some raw food");
        mActions[0].targetGoals.Add(new Goal("Eat", -3f));
        mActions[0].targetGoals.Add(new Goal("Sleep", +2f));
        mActions[0].targetGoals.Add(new Goal("Pee", +1f));

        mActions[1] = new Action("eat a snack");
        mActions[1].targetGoals.Add(new Goal("Eat", -2f));
        mActions[1].targetGoals.Add(new Goal("Sleep", -1f));
        mActions[1].targetGoals.Add(new Goal("Pee", +1f));

        mActions[2] = new Action("sleep in the bed");
        mActions[2].targetGoals.Add(new Goal("Eat", +2f));
        mActions[2].targetGoals.Add(new Goal("Sleep", -4f));
        mActions[2].targetGoals.Add(new Goal("Pee", +2f));

        mActions[3] = new Action("sleep on the sofa");
        mActions[3].targetGoals.Add(new Goal("Eat", +1f));
        mActions[3].targetGoals.Add(new Goal("Sleep", -2f));
        mActions[3].targetGoals.Add(new Goal("Pee", +1f));

        mActions[4] = new Action("drink a soda");
        mActions[4].targetGoals.Add(new Goal("Eat", -1f));
        mActions[4].targetGoals.Add(new Goal("Sleep", -2f));
        mActions[4].targetGoals.Add(new Goal("Pee", +3f));

        mActions[5] = new Action("visit the bathroom");
        mActions[5].targetGoals.Add(new Goal("Eat", 0f));
        mActions[5].targetGoals.Add(new Goal("Sleep", 0f));
        mActions[5].targetGoals.Add(new Goal("Pee", -4f));

        mChangeOverTime = new Action("tick");
        mChangeOverTime.targetGoals.Add(new Goal("Eat", +4f));
        mChangeOverTime.targetGoals.Add(new Goal("Sleep", +1f));
        mChangeOverTime.targetGoals.Add(new Goal("Pee", +2f));


        Debug.Log("Starting. One hour passes every " + TICK_LENGTH + " seconds.");
        InvokeRepeating("Tick", 0f, TICK_LENGTH);

        Debug.Log("Hit S to view Stats. Press Space to do something.");
    }

    void Tick()
    {
        ticks++;
        Debug.Log("Hours Passed: " + ticks);
        foreach (Goal goal in mGoals)
        {
            goal.value += mChangeOverTime.GetGoalChange(goal);
            goal.value = Mathf.Max(goal.value, 0);
        }
    }

    void PrintGoals()
    {
        string goalString = "";
        foreach (Goal goal in mGoals)
        {
            goalString += goal.name + ": " + goal.value + "; ";
        }
        goalString += "Discontentment: " + CurrentDiscontentment();
        Debug.Log(goalString);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

            Action bestThingToDo = ChooseAction(mActions, mGoals);
            Debug.Log("I think I will " + bestThingToDo.name);

            foreach (Goal goal in mGoals)
            {
                goal.value += bestThingToDo.GetGoalChange(goal);
                goal.value = Mathf.Max(goal.value, 0);
            }

            Debug.Log("New Stats: ");
            PrintGoals();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Current Stats: ");
            PrintGoals();
        }
    }

    Action ChooseAction(Action[] actions, Goal[] goals)
    {
        Action bestAction = null;
        float bestValue = float.PositiveInfinity;

        foreach (Action action in actions)
        {
            float thisValue = Discontentment(action, goals); ;
            if (thisValue < bestValue)
            {
                bestValue = thisValue;
                bestAction = action;
            }
        }

        return bestAction;
    }

    float Discontentment(Action action, Goal[] goals)
    {
        float discontentment = 0f;

        foreach (Goal goal in goals)
        {
            float newValue = goal.value + action.GetGoalChange(goal);
            newValue = Mathf.Max(newValue, 0);

            discontentment += goal.GetDiscontentment(newValue);
        }

        return discontentment;
    }

    float CurrentDiscontentment()
    {
        float total = 0f;
        foreach (Goal goal in mGoals)
        {
            total += (goal.value * goal.value);
        }
        return total;
    }
}
