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
                currentState?.Exit();                   //���� ���°��� ����������
                currentState = SearchState(newStateName);               //�μ��� �޾ƿ� ���°��� �Է�
                currentState?.Enter();                  //���� ���°�
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
