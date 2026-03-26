using Google.Protobuf.WellKnownTypes;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Barracuda;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
//using static UnityEditor.PlayerSettings; //editor only

namespace Footsies
{
    /// <summary>
    /// Machine-Learning playing agent for computer opponent
    /// </summary>
    public class PlayingAgent : Agent
    {
 
        public struct GameObservation
        {
            public GameObservation(float fight_dist, float opp_pos, int opp_state)
            {
                ////agent's self data
                //agentPosition = agent_pos;
                //agentState = agent_state;

                //general match data
                fightersDistance = fight_dist;

                //opponent data
                opponentPosition = opp_pos;
                opponentState = opp_state;
            }

            //public float agentPosition { get; set; }
            //public int agentState { get; set; }
            public float fightersDistance { get; set; }
            public float opponentPosition { get; set; }
            public int opponentState { get; set; }

        }

        private BattleCore battleCore;
        private bool isP2;
        private bool isInitialised = false;
        private int playingAgentInput;
        // Observations are held in a queue to later be sent to the playing agent, the aim of this is to mimic human reaction time delay 
        private Queue<GameObservation> delayedObservationQueue = new Queue<GameObservation>();
        //how many observations must be in the queue before being sent to the playing agent
        public static readonly uint MAX_OBSERVATION_RECORD = 10; //average human reaction time ranges from 200ms to 250ms, observations use fixed step every 20ms, so there must be 10 updates to simulate reaction delay
        private static readonly uint OBSERVATION_SPACE_SIZE = 5;

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

        private int GetNeutralInput()
        {
            return (int)InputDefine.None;
        }

        private Fighter GetThisFighter()
        {
            return isP2 == false ? battleCore.fighter1 : battleCore.fighter2;
        }

        private Fighter GetOpponentFighter()
        {
            return isP2 == true ? battleCore.fighter1 : battleCore.fighter2;
        }

        private void AddPaddedObservations(VectorSensor sensor)
        {
            for (int i = 0; i < OBSERVATION_SPACE_SIZE; i++)
            {
                sensor.AddObservation(0);
            }
        }


        //Start of Playing Agent Implementation
        public void Initialize(BattleCore core, bool is_this_P2) //get reference to battle core, used to access game data
        {
            battleCore = core;
            isP2 = is_this_P2;
            isInitialised = true;
        }

        public int getNextAIInput() //called from BattleCore to update the playing agent fighter
        {
            return playingAgentInput;
        }

        public override void OnEpisodeBegin()
        {
            //clear observation queue for new round, avoids adding observations from the previous round
            delayedObservationQueue.Clear();
            battleCore.resetBattle();
            
        }

        public void giveRoundOverReward(bool isWinner)
        {
            if (isWinner)
            {
                //SetReward(10.0f); 
                //AddReward(-1.0f);
                EndEpisode();
            }
            else
            {
                //AddReward(-1.0f);
                EndEpisode();
            }
        }

        public void giveHitConfirmReward()
        {
            AddReward(100.0f);
            EndEpisode();
        }

        public void giveHitConfirmPenalty()
        {
            AddReward(-5.0f);
            EndEpisode();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            if (isInitialised)
            {
                float this_agent_pos = GetThisFighter().position.x;
                int this_agent_state = GetThisFighter().currentActionID;

                float fighters_dist = GetDistanceX();

                float opponent_pos = GetOpponentFighter().position.x;
                int opponent_state = GetOpponentFighter().currentActionID;
                bool is_opponent_in_hitstun = GetOpponentFighter().isInHitStun;

                if (!isP2) //invert observations for p1 agent so that obervation values are the same on both player sides
                {
                    this_agent_pos *= -1; opponent_pos *= -1;
                }

                GameObservation newDelayedObservation = new GameObservation(fighters_dist, opponent_pos, opponent_state);

                delayedObservationQueue.Enqueue(newDelayedObservation);

                if (delayedObservationQueue.Count >= MAX_OBSERVATION_RECORD)
                {
                    GameObservation delayedObservation = delayedObservationQueue.Dequeue();

                    //Agent's self observations (these observations are not delayed as they are variables directly controlled by the agent)
                    sensor.AddObservation(this_agent_pos);
                    sensor.AddObservation(this_agent_state);

                    //the following observations are delayed by human reaction speeds as they are variables that the agent cannot control

                    //Game state observations
                    sensor.AddObservation(delayedObservation.fightersDistance);

                    //Observations of opponent
                    //sensor.AddObservation(delayedObservation.opponentPosition);
                    sensor.AddObservation(delayedObservation.opponentState);
                    // this observation is not delayed as the putting the opponent in hitstun is controllable by the agent and is a reaction point for follow up attacks
                    sensor.AddObservation(is_opponent_in_hitstun);
                }
                else AddPaddedObservations(sensor);


                Debug.Log("Fighter distance: " + fighters_dist);
                Debug.Log("is player 2 " + isP2.ToString() + " - position: " + this_agent_pos + " - state: " + this_agent_state);

                // Continuous negative rewards

                if (battleCore.roundState == BattleCore.RoundStateType.Fight)
                {
                    //Larger continous negative reward when agent goes to one side of level
                    if (this_agent_pos < -4 || this_agent_pos > 4)
                    {
                        AddReward(-0.3f);
                    }
                    ////Larger continous negative reward if agent is too far from or too close to the opponent
                    //if ((this_agent_state == 100 || this_agent_state == 105) && fighters_dist < 2.5)
                    //{
                    //    AddReward(+0.5f);
                    //}
                    else if (this_agent_state == 1)
                    {
                        AddReward((+1.0f / fighters_dist) * 0.1f);
                    }
                    else if (fighters_dist > 3.0)
                    {
                        AddReward(-0.3f);
                    }
                    else { AddReward(-0.3f); }

                    //if((this_agent_state == 100 || this_agent_state == 105 || this_agent_state == 0) && fighters_dist > 2.5){
                    //    AddReward(-0.02f);
                    //}

                    //if(this_agent_state == 115)
                    //{
                    //    AddReward(-1.0f);
                    //    EndEpisode();
                    //}
                }
            }
            else AddPaddedObservations(sensor);
        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            if (isInitialised)
            {
                // Get the action index for movement
                int movement = actionBuffers.DiscreteActions[0];
                // Get the action index for attacking
                int attack = actionBuffers.DiscreteActions[1];

                //refresh input to 0 so that inputs aren't held
                playingAgentInput = 0;

                // Look up the index in the movement action list:
                if (isP2)
                {
                    if (movement == 1) { playingAgentInput |= GetForwardInput(); }
                    if (movement == 2) { playingAgentInput |= GetBackwardInput(); }
                }
                else //inverted movement if player 1, so that "forwards" is the same input for both player sides
                {
                    if (movement == 1) { playingAgentInput |= GetBackwardInput(); }
                    if (movement == 2) { playingAgentInput |= GetForwardInput(); }
                }

                if (movement == 3) { playingAgentInput |= GetNeutralInput(); }

                // Look up the index in the attack action list:
                if (attack == 1) { playingAgentInput |= GetAttackInput(); }
                if (attack == 2) { playingAgentInput |= GetNeutralInput(); }

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