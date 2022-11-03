#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;


public class ChangeScene : Editor {

    [MenuItem("Open Scene/Form_Loading #1")]
    public static void OpenLoading()
    {
        OpenScene("Form_Loading");
    }
    /*
    [MenuItem("Open Scene/1_SausageWarIO #2")]
    public static void Open1_SausageWarIO()
    {
        OpenScene("1_SausageWarIO");
    }*/
    
    [MenuItem("Open Scene/2_WackyRun #2")]
    public static void Open2_WackyRun()
    {
        OpenScene("2_WackyRun");
    }

    [MenuItem("Open Scene/3_SnakeBlock #3")]
    public static void Open3_SnakeBlock() {
        OpenScene("3_SnakeBlock");
    }

    [MenuItem("Open Scene/4_PlatformPush #4")]
    public static void Open4_PlatformPush() {
        OpenScene("4_PlatformPush");
    }
    [MenuItem("Open Scene/5_HitAndRun #5")]
    public static void Open5_HitAndRun() {
        OpenScene("5_HitAndRun");
    }

    [MenuItem("Open Scene/6_SidestepSlope #6")]
    public static void Open6_SidestepSlope() {
        OpenScene("6_SidestepSlope");
    }

    [MenuItem("Open Scene/7_SkewerScurry #7")]
    public static void Open7_SkewerScurry() {
        OpenScene("7_SkewerScurry");
    }

    [MenuItem("Open Scene/8_GhostChaser #8")]
    public static void Open8_GhostChaser() {
        OpenScene("8_GhostChaser");
    }

    [MenuItem("Open Scene/9_OnTheCuttingBoard #9")]
    public static void Open9_OnTheCuttingBoard() {
        OpenScene("9_OnTheCuttingBoard");
    }

    [MenuItem("Open Scene/10_Slither #0")]
    public static void Open10_Slither() {
        OpenScene("10_Slither");
    }

    [MenuItem("Open Scene/Form_Home #10")]
    public static void OpenForm_Home() {
        OpenScene("Form_Home");
    }
    
    private static void OpenScene (string sceneName) {
		if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo ()) {
			EditorSceneManager.OpenScene ("Assets/_Game/_Scenes/" + sceneName + ".unity");
		}
	}
}
#endif