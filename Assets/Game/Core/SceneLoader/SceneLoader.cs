using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private int m_sceneIndex;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            LoadScene();
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(m_sceneIndex);
    }
}
