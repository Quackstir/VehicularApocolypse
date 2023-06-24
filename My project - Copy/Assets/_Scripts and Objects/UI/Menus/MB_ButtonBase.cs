using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VehicleApocolypse
{
    public class MB_ButtonBase : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Tooltip("Hold duration in seconds")]
        [Range(0.3f, 5f)] public float holdDuration = 0.5f;
        public UnityEvent onLongPress;

        [SerializeField]
        public UnityAction[] onPress;

        private bool isPointerDown = false;
        private bool isLongPressed = false;
        private DateTime pressTime;

        private Button button;
        private Image image;

        private WaitForSeconds delay;
        double elapsedSeconds;

        private void Awake()
        {
            button = GetComponent<Button>();
            image = GetComponent<Image>();
            delay = new WaitForSeconds(0.1f);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPointerDown = true;
            pressTime = DateTime.Now;
            StartCoroutine(Timer());
        }

        //public void OnPressedDown()


        public void OnPointerUp(PointerEventData eventData)
        {
            isPointerDown = false;
            isLongPressed = false;
        }

        public void getOnClick()
        {

        }

        private IEnumerator Timer()
        {
            while (isPointerDown && !isLongPressed)
            {
                elapsedSeconds = (DateTime.Now - pressTime).TotalSeconds;

                image.fillAmount = Mathf.MoveTowards(0, 1, ((float)elapsedSeconds / holdDuration));

                Debug.Log(elapsedSeconds);

                if (elapsedSeconds >= holdDuration)
                {
                    isLongPressed = true;
                    if (button.interactable)
                        onLongPress?.Invoke();

                    yield break;
                }

                yield return delay;
            }
        }
    }
}
