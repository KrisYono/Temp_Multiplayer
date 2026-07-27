using UnityEngine;
using UnityEngine.UI;

public class QTEManager : MonoBehaviour
{
    public Slider qteProgressBar;     // QTE 进度条
    public float fillSpeed = 30f;     // QTE 填充速度
    private bool isQTEActive = false; // 是否 QTE 激活

    private void Start()
    {
        if (qteProgressBar != null)
        {
            qteProgressBar.gameObject.SetActive(false);
            Debug.Log("QTE Progress Bar initialized and set to inactive.");
        }
        else
        {
            Debug.LogError("QTE Progress Bar is not assigned in QTEManager!");
        }
    }

    private void Update()
    {
        if (isQTEActive && Input.GetKey(KeyCode.E))
        {
            qteProgressBar.value += fillSpeed * Time.deltaTime;

            if (qteProgressBar.value >= qteProgressBar.maxValue)
            {
                CompleteQTE();
            }
        }
    }

    public void StartQTE()
    {
        if (qteProgressBar == null)
        {
            Debug.LogError("QTE Progress Bar is not assigned in QTEManager!");
            return;
        }

        isQTEActive = true;
        qteProgressBar.value = 0;
        qteProgressBar.gameObject.SetActive(true);
        Debug.Log("QTE started and progress bar is now active.");
    }

    public void StopQTE()
    {
        isQTEActive = false;
        if (qteProgressBar != null)
        {
            qteProgressBar.gameObject.SetActive(false);
            Debug.Log("QTE stopped and progress bar is now inactive.");
        }
    }

    private void CompleteQTE()
    {
        isQTEActive = false;
        if (qteProgressBar != null)
        {
            qteProgressBar.gameObject.SetActive(false);
        }
        Debug.Log("QTE Completed!");
    }
}