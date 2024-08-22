using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPreview : MonoBehaviour
{
    public GameObject placeholder;
    public GameObject prefab;
    public GameObject container;


    private void OnEnable()
    {
        UpdateCharacters();
    }

    void Start()
    {
        UpdateCharacters();
        GameManager.Instance.onSelectedCharactersChanged += UpdateCharacters;
    }

    [EasyButtons.Button]
    void UpdateCharacters()
    {
    
        var selectedCharacters = GameManager.Instance.selectedCharacters;
        Utils.DestroyAllChildren(container.transform);
        if (selectedCharacters == null || selectedCharacters.Length < 1)
        {
            placeholder.SetActive(true);
            return;
        }

        placeholder.SetActive(false);
        foreach (var item in selectedCharacters)
        {
            var obj = Instantiate(prefab, container.transform);
            var img = obj.transform.Find("Mask/Image");
            var im = img.GetComponent<Image>();
            im.sprite = item.characterSprite;
            im.SetNativeSize();
        }
    }

}