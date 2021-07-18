using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftRightSwipe : MonoBehaviour
{
    [SerializeField] GameObject cardObject;
    [SerializeField] GameObject[] items;

    public List<MarketItem> marketItems;

    int index = 0;

    float defaultHeight;
    void Start()
    {
        defaultHeight = cardObject.transform.localPosition.x;
    }

    public void UpdateDay()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (i == 0 && index == 0)
                continue;

            if (i == 2 && index == marketItems.Count - 1)
                continue;

            Transform child = items[i].transform.GetChild(0);

            child.GetChild(0).GetComponent<Text>().text = marketItems[index + i - 1].title;

            child.GetChild(1).GetComponent<Text>().text = marketItems[index + i - 1].date;

            child.GetChild(2).GetComponent<Text>().text = marketItems[index + i - 1].content;

            child.GetChild(4).GetComponent<Text>().text = marketItems[index + i - 1].authorName;
        }
    }

    //inside class
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;

    bool mouseDown = false;
    bool interpolating = false;
    float t;
    float startPos;
    float endPos;

    public void Update()
    {
        if (interpolating == false)
        {
            /*if (Input.touches.Length > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                //save began touch 2d point
                firstPressPos = new Vector2(t.position.x, t.position.y);
            }
            if (t.phase == TouchPhase.Ended)
            {
                //save ended touch 2d point
                secondPressPos = new Vector2(t.position.x, t.position.y);

                //create vector from the two points
                currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

                //normalize the 2d vector
                currentSwipe.Normalize();

                //swipe upwards
                if (currentSwipe.y > 50)
                {
                    Debug.Log("up swipe");
                }
                //swipe down
                if (currentSwipe.y < 50)
                {
                    Debug.Log("down swipe");
                }
            }
        }*/
            if (Input.GetMouseButtonDown(0))
            {
                //save began touch 2d point
                firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                mouseDown = true;
            }
            if (Input.GetMouseButton(0) && mouseDown)
            {
                //save ended touch 2d point
                secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                //create vector from the two points
                currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

                if ((currentSwipe.x < -0.25 && index == marketItems.Count - 1) == false && (currentSwipe.x > 0.25 && index == 0) == false)
                    cardObject.transform.localPosition = new Vector3(defaultHeight + currentSwipe.x, 0, 0);
            }
            if (Input.GetMouseButtonUp(0) && mouseDown)
            {
                mouseDown = false;
                //save ended touch 2d point
                secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                //create vector from the two points
                currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

                if ((currentSwipe.x < -0.25 && index == marketItems.Count - 1) == false && (currentSwipe.x > 0.25 && index == 0) == false)
                    startPos = defaultHeight + currentSwipe.x;
                else startPos = defaultHeight;
                interpolating = true;
                t = 0;

                //normalize the 2d vector
                currentSwipe.Normalize();

                //swipe upwards
                if (currentSwipe.x > 0.25 && index != 0)
                {
                    endPos = 1075;
                    index--;
                }
                //swipe down
                else if (currentSwipe.x < -0.25 && index != marketItems.Count - 1)
                {
                    endPos = -1075;
                    index++;
                }
                else
                {
                    endPos = 0;
                }
            }
        }
        else
        {
            //Tale.Interpolate(startPos, endPos, value => cardObject.transform.localPosition = value, 0.35f);
            //Tale.Exec(() => interpolating = false);
            t += Time.deltaTime * 4f;

            if (t > 1)
            {
                t = 1;
                interpolating = false;

                cardObject.transform.localPosition = new Vector3(0, 0, 0);

                UpdateDay();
            }
            else
                cardObject.transform.localPosition = new Vector3(Interpolate(startPos, endPos, t), 0, 0);
        }
    }

    public static float Interpolate(float start, float end, float t)
    {
        t = Mathf.Clamp01(t);
        return start - (start - end) * t;
    }
}
 