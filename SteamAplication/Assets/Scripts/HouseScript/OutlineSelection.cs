using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OutlineSelection : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnClassType))]
    public string selectedClassType;

    [SyncVar(hook = nameof(OnClassTypeText))]
    public string selectClassType;
    
    public Text selectClassTypeText;
    
    private Transform highlight;
    private Transform selection;
    private RaycastHit raycastHit;
    void Update()
    {
        if (!isClient) return;
        
        // Highlight
        if (highlight != null)
        {
            highlight.gameObject.GetComponent<Outline>().enabled = false;
            highlight = null;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit))
        {
            highlight = raycastHit.transform;
            if (highlight.CompareTag("Selectable") && highlight != selection)
            {
                if (highlight.gameObject.GetComponent<Outline>() != null)
                {
                    highlight.gameObject.GetComponent<Outline>().enabled = true;
                }
                else
                {
                    Outline outline = highlight.gameObject.AddComponent<Outline>();
                    outline.enabled = true;
                    highlight.gameObject.GetComponent<Outline>().OutlineColor = Color.magenta;
                    highlight.gameObject.GetComponent<Outline>().OutlineWidth = 7.0f;
                }
            }
            else
            {
                highlight = null;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (highlight)
            {
                if (selection != null)
                {
                    selection.gameObject.GetComponent<Outline>().enabled = false;
                }
                selection = raycastHit.transform;
                selection.gameObject.GetComponent<Outline>().enabled = true;
                selection.gameObject.GetComponent<House>().isSelect = true;
                
                selectedClassType = selection.gameObject.GetComponent<House>().type;
                selectClassType = selection.gameObject.GetComponent<House>().type.ToString();
                SetClassType();
                SetText();
                
                Debug.Log(selection.gameObject.GetComponent<House>().isSelect);
                highlight = null;
            }
            else
            {
                if (selection)
                {
                    selection.gameObject.GetComponent<Outline>().enabled = false;
                    selection.gameObject.GetComponent<House>().isSelect = false;
                    Debug.Log(selection.gameObject.GetComponent<House>().isSelect);
                    selection = null;
                }
            }
        }
    }

    public void SetClassType()
    {
        if (isClient)
        {
            CmdGetClassType(selectedClassType);
        }
    }
    public void SetText()
    {
        if (isClient)
        {
            CmdGetText(selectClassType);
        }
    }

    void OnClassType(string oldValue, string newValue)
    {
        RpcGetClassType(newValue);
    }

    void OnClassTypeText(string oldValue, string newValue)
    {
        RpcGetText(newValue);
    }

    [Command]
    void CmdGetClassType(string classType)
    {
        if (!isServer) return;
        
        selectedClassType = classType; 
        
        RpcGetClassType(classType);
    }

    [Command]
    void CmdGetText(string classType)
    {
        if (!isServer) return;
        
        selectClassType = classType;

        RpcGetText(classType);
    }

    [ClientRpc]
    void RpcGetClassType(string classType)
    {
        selectedClassType = classType;
        selectClassTypeText.text = selectedClassType.ToString();
    }

    [ClientRpc]
    void RpcGetText(string classType)
    {
        selectClassType = classType;
        selectClassTypeText.text = classType;
    }

}
