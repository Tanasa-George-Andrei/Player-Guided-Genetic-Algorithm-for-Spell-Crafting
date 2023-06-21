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
    //[SerializeField]
    //private GameObject SMStart;
    //[SerializeField]
    //private GameObject SMStart;
    [SerializeField]
    private GameObject SMCandidateSelect;
    [SerializeField]
    private GameObject SMSpellFinish;
    private SpellCandidate choiceCandidate = null;
    //[SerializeField]
    //private GameObject SMFinish;

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
    private GameObject SMCandidateSelectContent;
    [SerializeField]
    private GameObject SMSpellFinishContent;

    private List<CandidateUI> candidateUIData = new List<CandidateUI>();
    private List<SpellCandidate> candidateData;
    private int spellAlgID = 0;

    public void SetToInactiveAll()
    {
        HUD.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverScreen.SetActive(false);
        //SMStart.SetActive(false);
        SMCandidateSelect.SetActive(false);
        //SMFinish.SetActive(false);
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
            candidateUIData[index].isSelected = !candidateUIData[index].isSelected;
            candidateUIData[index].outline.enabled = candidateUIData[index].isSelected;
            choiceCandidate = candidateData[index];
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

    public void ChangeToSpellFinish()
    {
        ChangeToInterface(SMSpellFinish);
        isPaused = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ExitCandidateSelect()
    {
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
            count = candidateUIData.Count;
            for (int i = 0; i < count; i++)
            {
                candidateUIData[i].root.SetActive(false);
                candidateUIData[i].button.onClick.RemoveAllListeners();
                candidateUIData[i].isSelected = false;
            }
            action.NextIteration(candidateData, spellAlgID);
            ChangeToHUD();
            //canSend = false;
        }
    }

    public void FinishSpell()
    {
        action.FinishSpellCrafting(choiceCandidate);
        int count = candidateUIData.Count;
        for (int i = 0; i < count; i++)
        {
            candidateUIData[i].root.SetActive(false);
            candidateUIData[i].button.onClick.RemoveAllListeners();
            candidateUIData[i].isSelected = false;
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
        
    }
}
