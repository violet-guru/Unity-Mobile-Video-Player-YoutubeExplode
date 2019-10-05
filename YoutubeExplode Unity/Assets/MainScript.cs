using Infinity;
using MainLogic;
using UnityEngine;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
    public Button ThisButton;


    // Start is called before the first frame update
    void Start()
    {
        ThisButton.onClick.AddListener(TaskOnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void TaskOnClick()
    {
        var log = new SuperLog(new UnityLog(), false);
        log.Send(true, Hi.AutoTestNumber);
        var youVideos = new YoutubeHelper().GetVideos("X1x5crID83c", log);
    }
}
