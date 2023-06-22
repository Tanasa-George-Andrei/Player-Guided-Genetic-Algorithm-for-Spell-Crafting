using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{

    private class CandidateUI
    {
        public bool isSelected;
        public GameObject root;
        public Button button;
        public GameObject background;
        public Outline outline;
        public TMP_Text textMesh;

        public CandidateUI(bool _selected, GameObject _root, Button _button, GameObject _background, Outline _outline, TMP_Text _text)
        {
            isSelected = _selected;
            root = _root;
            button = _button;
            background = _background;
            outline = _outline;
            textMesh = _text;
        }
    }

    public static PlayerUIManager Instance { get; private set; }
    public bool IsPaused { get => isPaused; private set => isPaused = value; }

    [SerializeField]
    private PlayerAction action;

    [SerializeField]
    private GameObject HUD;
    [SerializeField]
    private Image spellMakerIndicator;

    [SerializeField]
    private GameObject SMCandidateSelect;
    [SerializeField]
    private GameObject SMSpellFinish;
    [SerializeField]
    private GameObject SMComponentSelect;
    [SerializeField]
    private GameObject SMElementSelect;
    private int choiceCandidateIndex = -1;

    private readonly GeneticSpellComponent[] allComponents 
        = new GeneticSpellComponent[] {new GCreateMProjectile(), new GPropel(),
            new GDashDir(), new GTeleportDir(), new GFork(),
            new GExplode(), new GBounceModifier(), new GChangeSpellElement(),
            new GApplyElementEffect()};
    private class ComponentUIData
    {
        public GameObject root;
        public Button button;
        public TMP_Text textMesh;

        public ComponentUIData(GameObject _root, Button _button, TMP_Text _textMesh)
        {
            root = _root;
            button = _button;
            textMesh = _textMesh;
        }
    }

    private List<GeneticSpellComponent> selectedComponents = new List<GeneticSpellComponent>();
    private List<ComponentUIData> selectedComponentsUI = new List<ComponentUIData>();

    [SerializeField]
    private GameObject gameOverScreen;

    [SerializeField]
    private GameObject pauseMenu;

    private bool isPaused = false;
    private Coroutine pauseWaitCoroutine;


    private GameObject currentUI;

    [SerializeField]
    private GameObject candidateCardPrefab;
    [SerializeField]
    private GameObject componentCardPrefab;
    [SerializeField]
    private GameObject SMCandidateSelectContent;
    [SerializeField]
    private GameObject SMSpellFinishContent;
    [SerializeField]
    private GameObject SMComponentSelectContent;
    [SerializeField]
    private GameObject SMSelectedComponentsContent;
    [SerializeField]
    private GameObject SMElementSelectContent;

    private List<CandidateUI> candidateUIData = new List<CandidateUI>();
    private List<SpellCandidate> candidateData;
    private int spellAlgID = 0;

    [SerializeField]
    private ElementData[] elements;
    private Outline SelectedElementOutline = null;
    private int selectedElementIndex = -1;

    public void SetToInactiveAll()
    {
        HUD.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverScreen.SetActive(false);
        SMCandidateSelect.SetActive(false);
        SMSpellFinish.SetActive(false);
        SMComponentSelect.SetActive(false);
        SMElementSelect.SetActive(false);
    }

    public void ChangeToInterface(GameObject _interface)
    {
        currentUI.SetActive(false);
        _interface.SetActive(true);
        currentUI = _interface;
    }

    public void ChangeToHUD()
    {
        ChangeToInterface(HUD);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if(pauseWaitCoroutine != null)
        {
            StopCoroutine(pauseWaitCoroutine);
        }
        pauseWaitCoroutine = StartCoroutine(WaitForPause());
    }

    public void SetSpellCandidates(List<SpellCandidate> _candidateData, int _ID, bool _finished)
    {
        candidateData = new List<SpellCandidate>(_candidateData);
        int count = candidateData.Count - candidateUIData.Count;
        GameObject temp;
        for (int i = 0; i < count; i++)
        {
            temp = Instantiate(candidateCardPrefab, _finished ? SMSpellFinishContent.transform : SMCandidateSelectContent.transform);
            candidateUIData.Add(new CandidateUI(false, temp, temp.GetComponent<Button>(), temp.transform.GetChild(0).gameObject, temp.GetComponentInChildren<Outline>(), temp.GetComponentInChildren<TMP_Text>()));
        }
        spellAlgID = _ID;
        count = candidateUIData.Count;
        for (int i = 0; i < count; i++)
        {
            candidateUIData[i].root.SetActive(false);
            candidateUIData[i].button.onClick.RemoveAllListeners();
            candidateUIData[i].isSelected = false;
        }
        count = candidateData.Count;
        for (int i = 0; i < count; i++)
        {
            candidateUIData[i].root.transform.parent = _finished ? SMSpellFinishContent.transform : SMCandidateSelectContent.transform;
            candidateUIData[i].root.SetActive(true);
            candidateUIData[i].textMesh.text = candidateData[i].displayString;
            candidateUIData[i].outline.enabled = false;
            int index = i;
            if(!_finished) 
            {
                candidateUIData[i].button.onClick.AddListener(() => ClickedOnCandidateUI(index));
            }
            else
            {
                candidateUIData[i].button.onClick.AddListener(() => ClickedOnSpellChoiceUI(index));
            }
        }
        //canSend = true;
    }
    public void ClickedOnCandidateUI(int index)
    {
        if (index >= 0 && index < candidateUIData.Count)
        {
            candidateUIData[index].isSelected = !candidateUIData[index].isSelected;
            candidateUIData[index].outline.enabled = candidateUIData[index].isSelected;
        }
    }
    public void ClickedOnSpellChoiceUI(int index)
    {
        if (index >= 0 && index < candidateUIData.Count)
        {
            if (index != choiceCandidateIndex)
            {
                if(choiceCandidateIndex != -1)
                {
                    candidateUIData[choiceCandidateIndex].isSelected = false;
                    candidateUIData[choiceCandidateIndex].outline.enabled = false;
                }
                choiceCandidateIndex = index;
                candidateUIData[index].isSelected = true;
                candidateUIData[index].outline.enabled = true;
            }
            else
            {
                choiceCandidateIndex = -1;
                candidateUIData[index].isSelected = false;
                candidateUIData[index].outline.enabled = false;
            }
        }
    }

    public void ClickedOnSpellComponentUI(int index)
    {
        if (index >= 0 && index < allComponents.Length)
        {
            selectedComponents.Add(allComponents[index].Clone());
            UpdateSelectedComponents();
        }
    }

    public void ClickedOnSelectedSpellComponentUI(GeneticSpellComponent _comp)
    {
        selectedComponents.Remove(_comp);
        UpdateSelectedComponents();
    }

    public void ClickedOnElementUI(int index, Outline outline)
    {
        if (index >= 0 && index < elements.Length)
        {
            if(selectedElementIndex != index)
            {
                selectedElementIndex = index;
                outline.enabled = true;
                if (SelectedElementOutline != null)
                {
                    SelectedElementOutline.enabled = false;
                }
                SelectedElementOutline = outline;
            }
            else
            {
                selectedElementIndex = -1;
                outline.enabled = false;
                SelectedElementOutline = null;
            }
        }
    }

    public void UpdateSelectedComponents()
    {
        int count = selectedComponents.Count - selectedComponentsUI.Count;
        GameObject temp;
        for (int i = 0; i < count; i++)
        {
            temp = Instantiate(componentCardPrefab, SMSelectedComponentsContent.transform);
            selectedComponentsUI.Add(new ComponentUIData(temp, temp.GetComponent<Button>(), temp.GetComponentInChildren<TMP_Text>()));
        }
        count = selectedComponentsUI.Count;
        for (int i = 0; i < count; i++)
        {
            selectedComponentsUI[i].root.SetActive(false);
            selectedComponentsUI[i].button.onClick.RemoveAllListeners();
        }
        count = selectedComponents.Count;
        for (int i = 0; i < count; i++)
        {
            selectedComponentsUI[i].root.SetActive(true);
            selectedComponentsUI[i].textMesh.text = selectedComponents[i].GetComponentName();
            GeneticSpellComponent hold = selectedComponents[i];
            selectedComponentsUI[i].button.onClick.AddListener(() => ClickedOnSelectedSpellComponentUI(hold));
        }
    }

    public void SetComponentInventory()
    {
        GameObject temp;
        for (int i = 0; i < allComponents.Length; i++)
        {
            temp = Instantiate(componentCardPrefab, SMComponentSelectContent.transform);
            int index = i;
            temp.GetComponent<Button>().onClick.AddListener(() => ClickedOnSpellComponentUI(index));
            temp.GetComponentInChildren<TMP_Text>().text = allComponents[i].GetComponentName();
        }
    }

    public void SetElements()
    {
        GameObject temp;
        for (int i = 0; i < elements.Length; i++)
        {
            temp = Instantiate(candidateCardPrefab, SMElementSelectContent.transform);
            int index = i;
            Outline outline = temp.GetComponentInChildren<Outline>();
            temp.GetComponent<Button>().onClick.AddListener(() => ClickedOnElementUI(index, outline));
            temp.GetComponentInChildren<TMP_Text>().text = elements[i].name;
        }
    }

    public void ChangeToCandidateSelect()
    {
        ChangeToInterface(SMCandidateSelect);
        isPaused = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ChangeToElementSelect()
    {
        ChangeToInterface(SMElementSelect);
        isPaused = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ChangeToSpellFinish()
    {
        ChangeToInterface(SMSpellFinish);
        isPaused = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ChangeToComponentSelect()
    {
        UpdateSelectedComponents();
        ChangeToInterface(SMComponentSelect);
        isPaused = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ExitCandidateSelect()
    {
        ChangeToHUD();
    }

    public void ForwardComponents()
    {
        action.SetComponents(selectedComponents);
        ChangeToHUD();
    }

    public void ForwardElements()
    {
        if(selectedElementIndex >= 0 && selectedElementIndex < elements.Length)
        {
            action.SetElement(elements[selectedElementIndex]);
            selectedElementIndex = -1;
            SelectedElementOutline.enabled = false;
            SelectedElementOutline = null;
        }
        ChangeToHUD();
    }

    public void ForwardCandidates()
    {
        //pauseWaitCoroutine = StartCoroutine(WaitForPause());
        int count = candidateData.Count;
        int selectCount = 0;
        for (int i = 0; i < count; i++)
        {
            candidateData[i].isChosen = candidateUIData[i].isSelected;
            selectCount++;
        }
        if (selectCount > 0 /*&& canSend*/)
        {
            action.NextIteration(candidateData, spellAlgID);
            ChangeToHUD();
            //canSend = false;
        }
    }

    public void StopSpellCrafting()
    {
        action.StopSpellCrafting();
        choiceCandidateIndex = -1;
        selectedComponents.Clear();
        ChangeToHUD();
    }

    public void ForceFinishSpellCrafting()
    {
        action.ForceFinishSpellCrafting();
        ChangeToHUD();
    }

    public void FinishSpell()
    {
        if (choiceCandidateIndex >= 0 && choiceCandidateIndex < candidateData.Count)
        {
            action.FinishSpellCrafting(candidateData[choiceCandidateIndex]);
            choiceCandidateIndex = -1;
            selectedComponents.Clear();
        }
        ChangeToHUD();
    }

    private void GameOver()
    {
        ChangeToInterface(gameOverScreen);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private IEnumerator WaitForPause()
    {
        yield return null;
        isPaused = false;
    }

    public void Pause()
    {
        if(currentUI == gameOverScreen)
        {
            return;
        }
        if(currentUI != HUD)
        {
            ChangeToHUD();
            return;
        }
        ChangeToInterface(pauseMenu);
        isPaused = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        currentUI = HUD;
        SetToInactiveAll();
        ChangeToHUD();
    }

    private void Start()
    {
        SetComponentInventory();
        SetElements();
    }
}
