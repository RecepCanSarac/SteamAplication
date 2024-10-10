using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OutlineSelection : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnClassTypeText))]
    public string selectClassType;

    public Text selectClassTypeText;

    private Transform highlight;
    private Transform selection;
    private RaycastHit raycastHit;

    void Update()
    {
        if (Camera.main.gameObject.activeSelf == false)
        {
            return;
        }
        
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

                selectClassType = selection.gameObject.GetComponent<House>().type.ToString();
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

    public void SetText()
    {
        CmdGetText(selectClassType);
    }

    void OnClassTypeText(string oldValue, string newValue)
    {
        RpcGetText(newValue);
    }

    [Command(requiresAuthority = false)]
    void CmdGetText(string classType)
    {
        selectClassType = classType;

        RpcGetText(classType);
    }

    [ClientRpc]
    void RpcGetText(string classType)
    {
        selectClassType = classType;
        selectClassTypeText.text = classType;
    }
}