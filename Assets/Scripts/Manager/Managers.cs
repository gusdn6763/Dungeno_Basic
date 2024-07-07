using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers instance; // 유일성이 보장된다
    private static Managers Instance { get { Init(); return instance; } } // 유일한 매니저를 갖고온다

    [SerializeField] private GameManager gameManager;
    [SerializeField] private ObjectManager objectManager;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private BattleManager battleManager;

    public static GameManager Game { get { return Instance?.gameManager; } }
    public static ObjectManager Object { get { return Instance?.objectManager; } }
    public static PlayerManager Player { get { return Instance?.playerManager; } }
    public static UIManager UI { get { return Instance?.uiManager; } }

    public static BattleManager Battle {get { return Instance?.battleManager; } }

    private void Awake()
    {
        Init();
    }

    public static void Init()
    {
        if (instance == null)
        {
            GameObject go = GameObject.Find("Managers");
            if (go == null)
            {
                go = new GameObject { name = "Managers" };
                go.AddComponent<Managers>();
            }

            instance = go.GetComponent<Managers>();
            DontDestroyOnLoad(instance);

            Application.targetFrameRate = 60;

            Game.Init();
            Player.Init();
        }
    }
}
