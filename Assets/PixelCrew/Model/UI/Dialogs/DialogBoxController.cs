using Assets.PixelCrew.Model.Data;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PixelCrew.Model.UI.Dialogs
{
    public class DialogBoxController : MonoBehaviour
    {
        [SerializeField] private Text _text;
        [SerializeField] private GameObject _container;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _textSpeed = 0.09f;

        [Header("Avatar")]
        [SerializeField] private Image _avatarImage;
        [SerializeField] private Image _avatarBackground;
        [SerializeField] private Sprite _defaultPlayerAvatar;
        [SerializeField] private RectTransform _avatarGroupRect;

        [Header("Alignment")]
        [SerializeField] private RectTransform _containerRect;
        [SerializeField] private Vector2 _containerPosLeft;
        [SerializeField] private Vector2 _containerPosRight;

        [Header("Sounds")]
        [SerializeField] private AudioClip _typing;
        [SerializeField] private AudioClip _open;
        [SerializeField] private AudioClip _close;

        private static readonly int IsOpen = Animator.StringToHash("IsOpen");

        private DialogData _data;
        private int _currentSentence;
        private AudioSource _sfxSource;
        private Coroutine _typingRoutine;
        private Sprite _npcAvatar;

        private void Start()
        {
            _sfxSource = GameObject.FindWithTag("SfxAudioSource").GetComponent<AudioSource>();
        }

        public void ShowDialog(DialogData data, Sprite npcAvatar)
        {
            _data = data;
            _npcAvatar = npcAvatar;
            _currentSentence = 0;
            _text.text = string.Empty;

            UpdateAvatarAndAlignment(_data.IsPlayerSpeaking(_currentSentence));

            _container.SetActive(true);
            _sfxSource.PlayOneShot(_open);
            _animator.SetBool(IsOpen, true);
        }

        private IEnumerator TypeDialogText()
        {
            _text.text = string.Empty;
            string sentence = _data.GetSentence(_currentSentence);

            foreach (char letter in sentence)
            {
                _text.text += letter;
                _sfxSource.PlayOneShot(_typing);
                yield return new WaitForSeconds(_textSpeed);
            }
            _typingRoutine = null;
        }

        private void UpdateAvatarAndAlignment(bool isPlayerSpeaking)
        {
            _avatarImage.sprite = isPlayerSpeaking ? _defaultPlayerAvatar : _npcAvatar;

            if (_avatarBackground != null)
                _avatarBackground.gameObject.SetActive(true);

            if (_containerRect != null)
            {
                if (isPlayerSpeaking)
                {
                    _containerRect.anchoredPosition = _containerPosLeft;
                    SetAnchorAndPivot(_avatarGroupRect, new Vector2(1.25f, 0.96f), new Vector2(1.25f, 0.96f));
                }
                else
                {
                    _containerRect.anchoredPosition = _containerPosRight;
                    SetAnchorAndPivot(_avatarGroupRect, new Vector2(-0.25f, 0.96f), new Vector2(-0.25f, 0.96f));
                }
                if (_avatarGroupRect != null)
                    _avatarGroupRect.anchoredPosition = Vector2.zero;
            }
        }

        private void SetAnchorAndPivot(RectTransform rect, Vector2 anchor, Vector2 pivot)
        {
            if (rect == null) return;
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = pivot;
        }

        public void OnSkip()
        {
            if (_typingRoutine == null) return;
            StopTypeAnimation();
            _text.text = _data.GetSentence(_currentSentence);
        }

        private void StopTypeAnimation()
        {
            if (_typingRoutine != null)
                StopCoroutine(_typingRoutine);
            _typingRoutine = null;
        }

        public void OnContinue()
        {
            StopTypeAnimation();
            _currentSentence++;

            if (_currentSentence >= _data.Count)
            {
                HideDialogBox();
            }
            else
            {
                OnStartDialogAnimation();
            }
        }

        private void HideDialogBox()
        {
            _animator.SetBool(IsOpen, false);
            _sfxSource.PlayOneShot(_close);
            if (_avatarBackground != null)
                _avatarBackground.gameObject.SetActive(false);
        }

        private void OnStartDialogAnimation()
        {
            UpdateAvatarAndAlignment(_data.IsPlayerSpeaking(_currentSentence));
            _typingRoutine = StartCoroutine(TypeDialogText());
        }

        private void OnCloseAnimationComplete() { }
    }
}