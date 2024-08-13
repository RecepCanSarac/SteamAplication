using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClassItem : MonoBehaviour
{
    public SOClass userClass;

    public Image icon;
    public TextMeshProUGUI ClassName;
    public ClassType type;
    public TextMeshProUGUI NumberText;
    public Button SetClassButton;

    private ClassGenerator ClassGenerator;
    public void Setup(string _name, Sprite _icon, ClassType _type)
    {
        ClassGenerator = GameObject.Find("ClassListUI").GetComponent<ClassGenerator>();

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
        _ClassGenerator.Userclasses.Add(this.userClass);
        _ClassGenerator.UpdateStackedClasses(this.userClass);
    }
}
