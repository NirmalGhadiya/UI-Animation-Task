using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameplayAnimation : MonoBehaviour
{
    #region PUBLIC_VARS

    public Transform transformCenterLionImage;


    public Vector3 defaultPositionCenterLionImage;
    public Vector3 targetPositionCenterLionImage;
    public Vector3 defaultScaleCenterLionImage;
    public Vector3 targetScaleCenterLionImage;

    [Header("Icon Panels")] public List<GameObject> listObjectsIconPanels;
    public CanvasGroup canvasGroupIconPanel;

    [Header("Elements Icons")] public List<Transform> listTransformIcons;
    public List<Transform> listTransformIconImages;
    public List<float> listIconsDefaultRotation;
    public List<float> listIconsTargetOpenRotation;
    public List<float> listIconsTargetScale;

    public float animationTime = 1;

    #endregion

    #region PRIVATE_VARS

    public List<int> _listIconsCurrentAngleIndex = new List<int> {0, 1, 2, 3, 4};
    private int _currentIndex = 0;
    private bool _isPanelOpened = false;
    private bool _isAnimationPlaying = false;
    private bool _isIconAnimationPlaying = false;

    #endregion

    #region PROTECTED_VARS

    #endregion

    #region UNITY_CALLBACKS

    #endregion

    #region PUBLIC_FUNCTIONS

    #endregion

    #region PRIVATE_FUNCTIONS

    private void StartLionOpenAnimation()
    {
        StartCoroutine(DoAnimateCenterLionImagePosition());
        for (int i = 0; i < listTransformIcons.Count; i++)
        {
            StartCoroutine(DoAnimateIconsImageRotation(i, listIconsTargetOpenRotation[_listIconsCurrentAngleIndex[i]]));
        }
    }

    private void StartLionCloseAnimation()
    {
        StartCoroutine(DoAnimateCenterLionImagePosition(true));
        for (int i = 0; i < listTransformIcons.Count; i++)
        {
            StartCoroutine(DoAnimateIconsImageRotation(i, listIconsDefaultRotation[i], true));
        }
    }

    private int GetCircularDifference(int currentIndex, int index)
    {
        int count = listIconsTargetOpenRotation.Count;
        int forwardDiff = (index - currentIndex + count) % count;
        int backwardDiff = forwardDiff - count;

        return (forwardDiff <= -backwardDiff) ? forwardDiff : backwardDiff;
    }

    private void MoveIcons(int index)
    {
        Debug.LogError(_currentIndex + " | " + index);
        if (!_isPanelOpened || _currentIndex == index || _isIconAnimationPlaying)
            return;

        StopAllCoroutines();

        float endAngle;
        int diff = GetCircularDifference(_currentIndex, index);

        for (int i = 0; i < _listIconsCurrentAngleIndex.Count; i++)
        {
            bool canWhole = false;
            _listIconsCurrentAngleIndex[i] += diff;
            if (_listIconsCurrentAngleIndex[i] > 4)
            {
                _listIconsCurrentAngleIndex[i] = _listIconsCurrentAngleIndex[i] % 5;
                canWhole = true;
            }
            else if (_listIconsCurrentAngleIndex[i] < 0)
            {
                _listIconsCurrentAngleIndex[i] = 5 + _listIconsCurrentAngleIndex[i];
                canWhole = true;
            }

            endAngle = listIconsTargetOpenRotation[_listIconsCurrentAngleIndex[i]] + (canWhole ? 360 : 0) * ((diff > 0) ? 1 : -1);
            StartCoroutine(DoAnimateIconsSelectImageRotation(i, endAngle));
        }

        _currentIndex = index;
        ToggleIconsPanels();
    }

    private void ToggleIconsPanels()
    {
        for (int i = 0; i < listObjectsIconPanels.Count; i++)
        {
            listObjectsIconPanels[i].SetActive(false);
        }

        listObjectsIconPanels[_currentIndex].SetActive(true);
    }

    #endregion

    #region PROTECTED_FUNCTIONS

    #endregion

    #region OVERRIDE_FUNCTIONS

    #endregion

    #region VIRTUAL_FUNCTIONS

    #endregion

    #region CO-ROUTINES

    private IEnumerator DoAnimateCenterLionImagePosition(bool isReverse = false)
    {
        float i = 0;
        float rate = 1 / animationTime;

        while (i <= 1)
        {
            i += Time.deltaTime * rate;

            transformCenterLionImage.localPosition = Vector3.Lerp(defaultPositionCenterLionImage, targetPositionCenterLionImage, isReverse ? (1 - i) : i);
            transformCenterLionImage.localScale = Vector3.Lerp(defaultScaleCenterLionImage, targetScaleCenterLionImage, isReverse ? (1 - i) : i);
            canvasGroupIconPanel.alpha = Mathf.Lerp(0, 1, isReverse ? (1 - i) : i);
            yield return null;
        }
    }

    private IEnumerator DoAnimateIconsSelectImageRotation(int index, float endAngle)
    {
        _isIconAnimationPlaying = true;
        
        float i = 0;
        float rate = 1 / animationTime;

        float startAngle = listTransformIcons[index].localEulerAngles.z;
        Vector3 oneVector = Vector3.one;
        Vector3 currentScale = listTransformIconImages[index].localScale;

        while (i <= 1)
        {
            i += Time.deltaTime * rate;
            float interpolatedAngle = Mathf.Lerp(startAngle, endAngle, i);

            listTransformIcons[index].rotation = Quaternion.Euler(0, 0, interpolatedAngle);
            listTransformIconImages[index].localScale = Vector3.Lerp(currentScale, (oneVector * listIconsTargetScale[_listIconsCurrentAngleIndex[index]]), i);
            listTransformIconImages[index].rotation = Quaternion.identity;

            yield return null;
        }

        listTransformIconImages[index].localScale = (oneVector * listIconsTargetScale[_listIconsCurrentAngleIndex[index]]);
        listTransformIcons[index].rotation = Quaternion.Euler(0, 0, endAngle);
        
        _isIconAnimationPlaying = false;
    }

    private IEnumerator DoAnimateIconsImageRotation(int index, float endAngle, bool isRevers = false)
    {
        float i = 0;
        float rate = 1 / animationTime;

        float startAngle = listTransformIcons[index].localEulerAngles.z;
        Vector3 oneVector = Vector3.one;

        if (index != _listIconsCurrentAngleIndex[index])
        {
            if ((index - _listIconsCurrentAngleIndex[index]) > 0)
            {
                endAngle += 360 * (isRevers ? -1 : 1);
            }
        }

        while (i <= 1)
        {
            i += Time.deltaTime * rate;
            float interpolatedAngle = Mathf.Lerp(startAngle, endAngle, i);

            listTransformIcons[index].rotation = Quaternion.Euler(0, 0, interpolatedAngle);
            listTransformIconImages[index].localScale = Vector3.Lerp(oneVector, (oneVector * listIconsTargetScale[_listIconsCurrentAngleIndex[index]]), isRevers ? (1 - i) : i);
            listTransformIconImages[index].rotation = Quaternion.identity;

            yield return null;
        }

        listTransformIconImages[index].localScale = isRevers ? oneVector : (oneVector * listIconsTargetScale[_listIconsCurrentAngleIndex[index]]);
        listTransformIcons[index].rotation = Quaternion.Euler(0, 0, endAngle);
    }

    #endregion

    #region UI_CALLBACKS

    public void OnButtonClick_LionOpen()
    {
        if (_isPanelOpened)
            return;

        _isPanelOpened = true;
        StopAllCoroutines();
        StartLionOpenAnimation();
    }

    public void OnButtonClick_LionClose()
    {
        if (!_isPanelOpened)
            return;

        _isPanelOpened = false;
        StopAllCoroutines();
        StartLionCloseAnimation();
    }

    public void OnButtonClick_Wind(int index)
    {
        MoveIcons(index);
    }

    #endregion
}