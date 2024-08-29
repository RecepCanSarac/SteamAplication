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
    [SyncVar(hook = nameof(OnStackCountChanged))]
    private int stackCount;
    
    private CustomNetworkManager manager;
    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Awake()
    {
        ClassGenerator = GameObject.Find("GUIPanel").GetComponent<ClassGenerator>();
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
                FromOldListNewList(ClassGenerator, Manager);
            });
        }
    }

    public void FromOldListNewList(ClassGenerator _ClassGenerator, CustomNetworkManager manager)
    {
        //_ClassGenerator.Userclasses.Add(this.userClass.ToString());
        if (isLocalPlayer)
        {
            manager.AddListClass(this.userClass);
            _ClassGenerator.SetList();
        }
        _ClassGenerator.UpdateStackedClasses(this.userClass);
    }


    private void OnStackCountChanged(int oldCount, int newCount)
    {
        NumberText.text = newCount > 1 ? newCount.ToString() : "1";
    }
}
