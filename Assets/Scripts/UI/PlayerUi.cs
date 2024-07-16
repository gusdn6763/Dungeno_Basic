using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUi : UIScript
{
    public Player CurrentPlayer { get; set; }

    [Header("UI")]
    [SerializeField] private IntroduceText messagePrefab;
    [SerializeField] private Transform messagePosition;

    [SerializeField] private AttributeSlider hpSlider;
    [SerializeField] private AttributeSlider hungrySlider;
    [SerializeField] private AttributeSlider fatigueSlider;

    [SerializeField] private TextMeshProUGUI currentLocation;
    [SerializeField] private Image currentImage;
    [SerializeField] private TextMeshProUGUI currentTime;

    [SerializeField] private TextMeshProUGUI previewLocation;
    [SerializeField] private Image previewImage;
    [SerializeField] private TextMeshProUGUI previewTime;

    private List<IntroduceText> previewList = new List<IntroduceText>();

    public void Init(Player player)
    {
        CurrentPlayer = player;

        currentLocation.text = Managers.Area.GetAreaName();
        currentTime.text = TimeConverter.AddTime(0);

        hpSlider.Initialize(CurrentPlayer.Hp);
        hungrySlider.Initialize(CurrentPlayer.Hungry);
        fatigueSlider.Initialize(CurrentPlayer.Fatigue);
        ClearPreview();
    }

    public void ClearPreview()
    {
        previewLocation.text = string.Empty;
        previewImage.enabled = false;
        previewTime.text = string.Empty;

        hpSlider.PreviewValue = 0;
        hungrySlider.PreviewValue = 0;
        fatigueSlider.PreviewValue = 0;
    }

    public void ShowStatus()
    {
        hpSlider.PreviewValue += CurrentPlayer.Hp;
        hungrySlider.PreviewValue += CurrentPlayer.Hungry;
        fatigueSlider.PreviewValue += CurrentPlayer.Fatigue;
    }

    public void CreateMessage(string message)
    {
        ChangeColor();

        IntroduceText introduceText = Instantiate(messagePrefab, messagePosition);
        introduceText.text.text = message;

        previewList.Add(introduceText);
    }

    public void CreateMessage(List<string> messages)
    {
        if (messages.Count > 0)
        {
            ChangeColor();

            foreach (string message in messages)
            {
                IntroduceText introduceText = Instantiate(messagePrefab, messagePosition);
                introduceText.text.text = message;
                previewList.Add(introduceText);
            }
        }
    }
    public void ChangeColor()
    {
        if (previewList.Count > 0)
            foreach (IntroduceText text in previewList)
                text.ColorChange();

        previewList.Clear();
    }

    public void AddTime(int Time)
    {
        currentTime.text = TimeConverter.AddTime(Time);
    }
}
