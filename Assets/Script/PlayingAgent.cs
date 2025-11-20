using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Footsies
{
    /// <summary>
    /// Machine-Learning playing agent for computer opponent
    /// </summary>
    public class PlayingAgent : Agent
    {
      
        private BattleCore battleCore;
        private int playingAgentInput;

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
        public void Initialize(BattleCore core) //get reference to battle core, used to access game data
        {
            battleCore = core;
        }

        public int getNextAIInput() //called from BattleCore to update the playing agent fighter
        {
            return playingAgentInput;
        }

        public override void OnEpisodeBegin()
        {
            battleCore.callBattleStart();
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

            //refresh input to 0 so that inputs aren't held
            playingAgentInput = 0;

            // Look up the index in the movement action list:
            if (movement == 1) { playingAgentInput = GetForwardInput(); }
            if (movement == 2) { playingAgentInput = GetBackwardInput(); }
            //movement 3 == no movement

            // Look up the index in the attack action list:
            if (attack == 1) { playingAgentInput = GetAttackInput(); }
            //attack 2 == no jump

            //// Rewards

            //positive reward when agent hits player 
            if (battleCore.fighter1.isInHitStun) 
            {
                SetReward(0.5f);
            }

            Debug.Log(GetDistanceX());

            //negative reward when agent is far away from the player
            if (GetDistanceX() > 6f && battleCore.roundState == BattleCore.RoundStateType.Fight)
            {
                SetReward(-1.0f);
                EndEpisode();
            }

            //End Episode when match is over
            if (battleCore.roundState == BattleCore.RoundStateType.End) 
            {
                SetReward(1.0f); //add reward, as we are assuming player 1 isn't playing against the AI during current training
                EndEpisode();
            }
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var discreteActionsOut = actionsOut.DiscreteActions;

            if (InputManager.Instance.GetButton(InputManager.Command.p2Left))
            {
                discreteActionsOut[0] = 1; //move forwards
                Debug.Log("forwards button pressed");
            }
            else if (InputManager.Instance.GetButton(InputManager.Command.p2Right))
            {
                discreteActionsOut[0] = 2; //move backwards
                Debug.Log("backwards button pressed");
            }

            if (InputManager.Instance.GetButton(InputManager.Command.p2Attack))
            {
                discreteActionsOut[1] = 1; //attack
                Debug.Log("attack button pressed");
            }
        }
    }
}