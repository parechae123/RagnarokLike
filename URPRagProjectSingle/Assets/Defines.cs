namespace NeutralDefines
{
    namespace State
    {
        
        using PlayerDefines.States;
        using Unity.VisualScripting;
        using UnityEngine;

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
        [System.Serializable]
        public class CursorStates
        {
            [SerializeField] private Texture2D defaultCursorIMG;
            [SerializeField] private Texture2D noneClickAbleIMG;
            [SerializeField] private Texture2D grabCursorIMG;
            [SerializeField] private Texture2D attackAbleCursorIMG;

            public void SetDefaultCursor()
            {
                Cursor.SetCursor(defaultCursorIMG, Vector2.left + Vector2.up, CursorMode.Auto);
            }
            public cursorState CurrentCursorState 
            {
                get;
                private set;
            }
            public void changeState(cursorState nextCursorState)
            {
                if (nextCursorState == CurrentCursorState) return;
                CurrentCursorState = nextCursorState;
                switch (CurrentCursorState)
                {
                    case cursorState.defaultCurser:
                        Cursor.SetCursor(defaultCursorIMG, Vector2.left + Vector2.up, CursorMode.Auto);
                        break;
                    case cursorState.noneClickAbleState:
                        Cursor.SetCursor(noneClickAbleIMG, Vector2.left + Vector2.up, CursorMode.Auto);
                        break;
                    case cursorState.grabCursor:
                        Cursor.SetCursor(grabCursorIMG, Vector2.left + Vector2.up, CursorMode.Auto);
                        break;
                    case cursorState.attackAble:
                        Cursor.SetCursor(attackAbleCursorIMG, Vector2.left + Vector2.up, CursorMode.Auto);
                        break;

                }
            }
        }
        public enum cursorState
        {
            defaultCurser, noneClickAbleState, grabCursor, attackAble
        }

    }
}
