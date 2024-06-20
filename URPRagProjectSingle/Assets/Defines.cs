namespace NeutralDefines
{
    namespace State
    {
        
        using PlayerDefines.States;
        public class StateMachine
        {
            
            private PlayerStates[] allStates = new PlayerStates[0]; 
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
                sbyte i = 0;
                for (; i < allStates.Length; i++)
                {
                    if (allStates[i].stateName == newStateName)
                    {
                        break;
                    }
                }
                currentState?.Exit();                   //���� ���°��� ����������
                currentState = allStates[i];                //�μ��� �޾ƿ� ���°��� �Է�
                currentState?.Enter();                  //���� ���°�
            }
        }
    }
}
