namespace NeutralDefines
{
    namespace State
    {
        
        using PlayerDefines.States;
        using Unity.VisualScripting;

        [System.Serializable]
        public class StateMachine
        {
            
            private PlayerStates[] allStates = new PlayerStates[0]; 
            public PlayerStates[] AllStates
            {
                get { return allStates; }
            }
            private PlayerStates currentState;
            public PlayerStates CurrentState
            {
                get { return currentState; }
            }
            
            public StateMachine(PlayerStates[] defaultStates)
            {
                allStates = defaultStates;            
            }
            
            public void ChangeState(string newStateName)
            {
                if (currentState == null) 
                {
                    currentState = SearchState("idleState");
                    currentState.Enter();
                    return;
                }
                currentState?.Exit();                   //이전 상태값을 빠져나간다
                currentState = SearchState(newStateName);               //인수로 받아온 상태값을 입력
                currentState?.Enter();                  //다음 상태값
            }
            private PlayerStates SearchState(string stateName)
            {
                sbyte i = 0;
                for (; i < allStates.Length; i++)
                {
                    if (allStates[i].stateName == stateName)
                    {
                        break;
                    }
                }
                return allStates[i];
            }
        }
    }
}
