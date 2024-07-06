using UnityEngine;
using UnityEngine.UI;

public class PlayerPartUi : UIScript
{
    public Player currentPlayer { get; set; }

    [SerializeField] private Text headText;
    [SerializeField] private Text chestText;
    [SerializeField] private Text leftArmText;
    [SerializeField] private Text rightArmText;
    [SerializeField] private Text leffLegText;
    [SerializeField] private Text rightLegText;
    public void Init(Player player)
    {
        currentPlayer = player;
    }

    public void StatusUpdate()
    {
        int count = currentPlayer.Parts.Count;
        for (int i = 0; i < count; i++)
        {
            Part part = currentPlayer.Parts[i];

            switch (part.partType)
            {
                case PartType.Head:
                    headText.text = part.CurrentHp + "/" + part.fullHp;
                    break;
                case PartType.Chest:
                    chestText.text = part.CurrentHp + "/" + part.fullHp;
                    break;
                case PartType.LeftArm:
                    leftArmText.text = part.CurrentHp + "/" + part.fullHp;
                    break;
                case PartType.RightArm:
                    rightArmText.text = part.CurrentHp + "/" + part.fullHp;
                    break;
                case PartType.LeftLeg:
                    leffLegText.text = part.CurrentHp + "/" + part.fullHp;
                    break;
                case PartType.RightLeg:
                    rightLegText.text = part.CurrentHp + "/" + part.fullHp;
                    break;
            }
        }
    }
}
