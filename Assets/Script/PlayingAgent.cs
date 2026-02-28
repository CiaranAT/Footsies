using Google.Protobuf.WellKnownTypes;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace Footsies
{
    /// <summary>
    /// Machine-Learning playing agent for computer opponent
    /// </summary>
    public class PlayingAgent : Agent
    {
 
        public struct GameObservation
        {
            public GameObservation(float oppPos, float agentPos, float fightDist, int oppState)
            {
                opponentPosition = oppPos;
                agentPosition = agentPos;
                fightersDistance = fightDist;
                opponentState = oppState;
            }

            public float opponentPosition { get; set; }
            public float agentPosition { get; set; }
            public float fightersDistance { get; set; }
            public int opponentState { get; set; }

        }

        private BattleCore battleCore;
        private bool isP2;
        private bool isInitialised = false;
        private int playingAgentInput;
        // Observations are held in a queue to later be sent to the playing agent, the aim of this is to mimic human reaction time delay 
        private Queue<GameObservation> observationQueue = new Queue<GameObservation>();
        //how many observations must be in the queue before being sent to the playing agent
        public static readonly uint maxObservationRecord = 13; //average human reaction time is 250ms and observations use fixed step every 20ms, so there must be 13 updates to simulate reaction delay

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
            observationQueue.Clear();

            //temporarily removed for in-person feasibility demo
            //battleCore.callBattleStart();
        }

        public void giveRoundOverReward(bool isWinner)
        {
            if (isWinner)
            {
                SetReward(1.0f); 
                EndEpisode();
            }
            else
            {
                SetReward(-1.0f);
                EndEpisode();
            }
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            if (isInitialised)
            {
                Vector2 pos = battleCore.fighter1.position;
                Vector2 oppPos = battleCore.fighter2.position;
                float dist = GetDistanceX();

                GameObservation newObservation = new GameObservation(oppPos.x, pos.x, dist, GetOpponentFighter().currentActionID);

                observationQueue.Enqueue(newObservation);

                if (observationQueue.Count >= maxObservationRecord)
                {
                    GameObservation delayedObservation = observationQueue.Dequeue();

                    //Opponent position 
                    sensor.AddObservation(delayedObservation.opponentPosition);

                    //Agent position
                    sensor.AddObservation(delayedObservation.agentPosition);

                    //Distance between fighters
                    sensor.AddObservation(delayedObservation.fightersDistance);

                    //Opponent's current action
                    sensor.AddObservation(delayedObservation.opponentState);
                }

                Debug.Log(GetDistanceX());
                Debug.Log(GetThisFighter().position.x + " & is player 2 " + isP2.ToString());

                // Continuous negative rewards

                if (battleCore.roundState == BattleCore.RoundStateType.Fight)
                {
                    //Larger continous negative reward when agent goes to one side of level
                    if (GetThisFighter().position.x < -2.5 || GetThisFighter().position.x > 2.5)
                    {
                        SetReward(-0.15f);
                    }
                    else { SetReward(-0.03f); }

                    //Larger continous negative reward if agent is too far or close from opponent
                    if (dist > 4.0)
                    {
                        SetReward(-0.15f);
                    }
                    else if (dist < 2.0)
                    {
                        SetReward(-0.15f);
                    }
                }
            }
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
                if (movement == 1) { playingAgentInput |= GetForwardInput(); }
                if (movement == 2) { playingAgentInput |= GetBackwardInput(); }
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