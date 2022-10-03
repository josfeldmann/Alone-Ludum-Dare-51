using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NavigationObject : MonoBehaviour, IPointerEnterHandler {
    public NavigationObject up, down, left, right;
    public UnityEvent OnSelect;

    public UnityEvent OnAPressed;
    

    internal NavigationObject GetRightObject() {
        if (right == null) return right;
        if (right.gameObject.activeInHierarchy) return right;
        NavigationObject target = right;
        index = 0;
        while (target.right != null && index < indexMax) {
            if (target.right.gameObject.activeInHierarchy) {
                return target.right;
            }
            target = target.right;
            index++;
        }
        return null;
    }
    static int index = 0;
    static int indexMax = 20;
    internal NavigationObject GetLeftObject() {
        if (left == null) return left;
        if (left.gameObject.activeInHierarchy) return left;
        NavigationObject target = left;
        index = 0;
        while (target.left != null && index < indexMax) {
            if (target.left.gameObject.activeInHierarchy) {
                return target.left;
            }
            target = target.left;
            index++;
        }
        return null;
    }


    internal NavigationObject GetDownObject() {
        if (down == null) return down;
        if (down.gameObject.activeInHierarchy) return down;
        NavigationObject target = down;
        index = 0;
        while (target.down != null && index < indexMax) {
            if (target.down.gameObject.activeInHierarchy) {
                return target.down;
            }
            target = target.down;
            index++;
        }
        return null;
    }

    public void StopNavigation() {
        NavigationManager.StopNavigation();
    }

    public void RestartNavigation() {
        NavigationManager.SetSelectedObject(this);
    }

    internal NavigationObject GetUpObject() {
        if (up == null) return up;
        if (up.gameObject.activeInHierarchy) return up;
        NavigationObject target = up;
        index = 0;
        while (target.up != null && index < indexMax) {
            if (target.up.gameObject.activeInHierarchy) {
                return target.up;
            }
            target = target.up;
            index++;
        }
        return null;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (NavigationManager.navigation) {
            NavigationManager.SetSelectedObject(this);
        }
    }

    Button button;

    private void Awake() {
        
        
    }

    public void ADown() {
        Debug.Log("ADOWN " + Time.time);
        if (button != null) button.onClick.Invoke();
        if (OnAPressed != null) OnAPressed.Invoke();
        else {
            Debug.LogError("ON A PRESSED IS NULL FOR SOME REASON");
        }
    }

    public void BDown() {

    }



    public void SetButton() {
        if (button == null) button = GetComponent<Button>();
    }

    bool checkedyet = false;

    public void MakeButtonFix() {
        if (button == null && !checkedyet) {
            checkedyet = true;
            button = GetComponent<Button>();
        }
        Navigation n = new Navigation();
        n.mode = Navigation.Mode.Explicit;
        if (up)n.selectOnUp = up.button;
        if (down)n.selectOnDown = down.button;
        if (right)n.selectOnRight = right.button;
        if (left)n.selectOnLeft = left.button;
        if (button) button.navigation = n;
    }


}
