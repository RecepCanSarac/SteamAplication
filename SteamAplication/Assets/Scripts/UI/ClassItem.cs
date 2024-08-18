using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClassItem : NetworkBehaviour
{
    public SOClass userClass;
    public Image icon;
    public TextMeshProUGUI ClassName;
    public ClassType type;
    public TextMeshProUGUI NumberText;
    public Button SetClassButton;

    private ClassGenerator ClassGenerator;
    ClassManagment manager;
    [SyncVar(hook = nameof(OnStackCountChanged))]
    private int stackCount;

    private void Awake()
    {
        ClassGenerator = GameObject.Find("GUIPanel").GetComponent<ClassGenerator>();
        manager = GameObject.Find("ClassManagment").GetComponent<ClassManagment>();
    }

    public void Setup(string _name, ClassType _type)
    {
        ClassName.text = _name;
        //icon.sprite = _icon;
        type = _type;

        if (SetClassButton != null)
        {
            SetClassButton.onClick.AddListener(() =>
            {
                FromOldListNewList(ClassGenerator, manager);
            });
        }
    }

    public void FromOldListNewList(ClassGenerator _ClassGenerator, ClassManagment manager)
    {
        _ClassGenerator.Userclasses.Add(this.userClass);
        if (isServer)
        {
            Debug.Log("Adding class to the management list: " + this.userClass.name);
            manager.CMDAddListClass(this.userClass);
        }
        _ClassGenerator.UpdateStackedClasses(this.userClass);
    }


    private void OnStackCountChanged(int oldCount, int newCount)
    {
        NumberText.text = newCount > 1 ? newCount.ToString() : "1";
    }
}
