using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyStarter : MonoBehaviour
{
    public KeyBindOrganizator keyBindOrgOBJ;
    // Start is called before the first frame update
    void Start()
    {
        keyBindOrgOBJ.ReadOrCreateKeyBind();
    }
}
