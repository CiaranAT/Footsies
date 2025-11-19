using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

namespace Footsies
{
    /// <summary>
    /// Machine-Learning playing agent for computer opponent
    /// </summary>
    public class PlayingAgent : Agent
    {
                //currently copied code from BattleAI

        public class FightState
        {
            public float distanceX;
            public bool isOpponentDamage;
            public bool isOpponentGuardBreak;
            public bool isOpponentBlocking;
            public bool isOpponentNormalAttack;
            public bool isOpponentSpecialAttack;
        }

        private BattleCore battleCore;

        private Queue<int> moveQueue = new Queue<int>();
        private Queue<int> attackQueue = new Queue<int>();

        // previous fight state data
        private FightState[] fightStates = new FightState[maxFightStateRecord];
        public static readonly uint maxFightStateRecord = 10;
        private int fightStateReadIndex = 5;

        public PlayingAgent(BattleCore core)
        {
            battleCore = core;
        }
        private float GetDistanceX()
        {
            return Mathf.Abs(battleCore.fighter2.position.x - battleCore.fighter1.position.x);
        }

        private int GetAttackInput()
        {
            return (int)InputDefine.Attack;
        }

        private int GetForwardInput()
        {
            return (int)InputDefine.Left;
        }

        private int GetBackwardInput()
        {
            return (int)InputDefine.Right;
        }

                //Start of Playing Agent Implementation

        public Transform Target;
        public override void OnEpisodeBegin()
        {
            //// If the Agent fell, zero its momentum
            //if (this.transform.localPosition.y < 0)
            //{
            //    this.rBody.angularVelocity = Vector3.zero;
            //    this.rBody.linearVelocity = Vector3.zero;
            //    this.transform.localPosition = new Vector3(0, 0.5f, 0);
            //}

            //// Move the target to a new spot
            //Target.localPosition = new Vector3(Random.value * 8 - 4,
            //                                   0.5f,
            //                                   Random.value * 8 - 4);
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            // Opponent Position 
            sensor.AddObservation(battleCore.fighter1.position.x);

            //Agent Position
            sensor.AddObservation(battleCore.fighter2.position.x);

            //Distance Between Fighters
            sensor.AddObservation(GetDistanceX());
        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            // Get the action index for movement
            int movement = actionBuffers.DiscreteActions[0];
            // Get the action index for attacking
            int attack = actionBuffers.DiscreteActions[1];

            // Look up the index in the movement action list:
            if (movement == 1) { moveQueue.Enqueue(GetForwardInput()); } 
            if (movement == 2) { moveQueue.Enqueue(GetBackwardInput()); }

            // Look up the index in the attack action list:
            // attack 1 == no input
            if (attack == 2) { moveQueue.Enqueue(GetAttackInput()); }

            //// Rewards

            //If Playing Agent hits Player, add reward and end episode 
            if (battleCore.fighter1.isInHitStun) 
            {
                SetReward(1.0f);
                EndEpisode();
            }

            //If Playing Agent is too far away from Player, give negative reward and end episode
            if (GetDistanceX() < 10f)
            {
                SetReward(-1.0f);
                EndEpisode();
            }
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var discreteActionsOut = actionsOut.DiscreteActions;

            if (InputManager.Instance.GetButton(InputManager.Command.p2Left))
            {
                discreteActionsOut[1] = 1; //move forwards
            }
            else if (InputManager.Instance.GetButton(InputManager.Command.p2Right))
            {
                discreteActionsOut[1] = 2; //move backwards
            }

            if (InputManager.Instance.GetButton(InputManager.Command.p2Attack)) {
                discreteActionsOut[1] = 2; //attack
            }
            else {
                discreteActionsOut[1] = 1; //nothing
            }
        }
    }
}