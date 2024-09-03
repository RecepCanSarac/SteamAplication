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
    public Button ClassDeleteButton;
    private ClassGenerator ClassGenerator;


    private CustomNetworkManager manager;
    public CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }
            return manager = NetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Awake()
    {
        ClassGenerator = GameObject.Find("GUIPanel").GetComponent<ClassGenerator>();
    }
    private void Start()
    {
        if (ClassDeleteButton != null)
            ClassDeleteButton.onClick.AddListener(() =>
            {
                RemoveClassItem(ClassGenerator, Manager);
            });
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
        if (manager.GamePlayers[0].isLocalPlayer == true)
        {
            _ClassGenerator.SetList();
            manager.AddListClass(this.userClass);

            _ClassGenerator.UpdateStackedClasses(this.userClass);
        }
    }

    public void RemoveClassItem(ClassGenerator _ClassGenerator, CustomNetworkManager manager)
    {
        if (manager.GamePlayers[0].isLocalPlayer == true)
        {
            _ClassGenerator.RemoveFromStackedClasses(this.userClass);
            _ClassGenerator.SetList();
        }
    }
    
}
