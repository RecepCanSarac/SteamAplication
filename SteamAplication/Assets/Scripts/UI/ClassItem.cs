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

    private void Awake()
    {
        ClassGenerator = GameObject.Find("ClassListUI").GetComponent<ClassGenerator>();
    }

    public void Setup(string _name, Sprite _icon, ClassType _type)
    {
        ClassName.text = _name;
        icon.sprite = _icon;
        type = _type;

        if (SetClassButton != null)
        {
            SetClassButton.onClick.AddListener(() =>
            {
                FromOldListNewList(ClassGenerator);
            });
        }
    }

    public void FromOldListNewList(ClassGenerator _ClassGenerator)
    {
        if (isServer)
        {
            _ClassGenerator.Userclasses.Add(this.userClass);
            _ClassGenerator.UpdateStackedClasses(this.userClass);
        }
    }

    private void OnStackCountChanged(int oldCount, int newCount)
    {
        NumberText.text = newCount > 1 ? newCount.ToString() : "1";
    }
}
