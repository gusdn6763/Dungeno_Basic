using UnityEngine;
using UnityEngine.UI;
using static Define;

public class PlayerPartUi : UIScript
{
    public Player CurrentPlayer { get; set; }

    [SerializeField] private Text headText;
    [SerializeField] private Text chestText;
    [SerializeField] private Text leftArmText;
    [SerializeField] private Text rightArmText;
    [SerializeField] private Text leffLegText;
    [SerializeField] private Text rightLegText;
    public void Init(Player player)
    {
        CurrentPlayer = player;
    }

    public void StatusUpdate()
    {
        int count = CurrentPlayer.Parts.Count;
        for (int i = 0; i < count; i++)
        {
            Part part = CurrentPlayer.Parts[i];

            switch (part.PartType)
            {
                case E_PartType.Head:
                    headText.text = part.CurrentHp + "/" + part.fullHp;
                    break;
                case E_PartType.Chest:
                    chestText.text = part.CurrentHp + "/" + part.fullHp;
                    break;
                case E_PartType.LeftArm:
                    leftArmText.text = part.CurrentHp + "/" + part.fullHp;
                    break;
                case E_PartType.RightArm:
                    rightArmText.text = part.CurrentHp + "/" + part.fullHp;
                    break;
                case E_PartType.LeftLeg:
                    leffLegText.text = part.CurrentHp + "/" + part.fullHp;
                    break;
                case E_PartType.RightLeg:
                    rightLegText.text = part.CurrentHp + "/" + part.fullHp;
                    break;
            }
        }
    }
}
