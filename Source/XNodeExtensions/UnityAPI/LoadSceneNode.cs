using SiphoinUnityHelpers.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI
{
    public class LoadSceneNode : BaseNodeInteraction
    {
        [SerializeField, SceneField] private string _scene;

        [SerializeField] private LoadSceneMode _mode;

        [SerializeField] private bool _isAsync;

        public override void Execute()
        {
            if (_isAsync)
            {
                SceneManager.LoadSceneAsync(_scene, _mode);
            }

            else
            {
                SceneManager.LoadScene(_scene, _mode);
            }
        }
    }
}
