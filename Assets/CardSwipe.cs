using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Helpers.Json;
using System.IO;

public class CardSwipe : MonoBehaviour
{
    [SerializeField] GameObject cardObject;
    [SerializeField] float topY = 1550;
    [SerializeField] float bottomY = -1550;
    [SerializeField] GameObject[] cards;

    float defaultHeight;

    int index = 0;
    public List<WeekDay> days = new List<WeekDay>();

    public List<MarketItem> items = new List<MarketItem>();

    void Start()
    {
        defaultHeight = cardObject.transform.localPosition.y;
    }

    public void UpdateDay()
    {
        for(int i=0; i<cards.Length;i++)
        {
            if (i == 0 && index == 0)
                continue;

            if (i == 2 && index == days.Count - 1)
                continue;

            Transform child = cards[i].transform.GetChild(0);

            child.GetChild(0).GetComponent<Text>().text = days[index + i - 1].name;

            child.GetChild(1).GetComponent<Text>().text = days[index + i - 1].start+":00";

            child.GetChild(3).localEulerAngles = new Vector3(0, 0, 90 - 30 * (int)days[index + i - 1].start);
        }
    }

    public void UpdateCard()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            if (i == 0 && index == 0)
                continue;

            if (i == 2 && index == items.Count - 1)
                continue;

            Transform child = cards[i].transform.GetChild(0);

            child.GetChild(0).GetComponent<Text>().text = items[index + i - 1].title;

            child.GetChild(1).GetComponent<Text>().text = items[index + i - 1].date;

            child.GetChild(2).GetComponent<Text>().text = items[index + i - 1].content;

            child.GetChild(3).GetComponent<Text>().text = items[index + i - 1].authorName;
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
        {/*if (Input.touches.Length > 0)
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

                if (days.Count != 0)
                {
                    if ((currentSwipe.y < -0.25 && index == 0) == false && (currentSwipe.y > 0.25 && index == days.Count - 1) == false)
                        cardObject.transform.localPosition = new Vector3(0, defaultHeight + currentSwipe.y, 0);
                }
                else if ((currentSwipe.y < -0.25 && index == 0) == false && (currentSwipe.y > 0.25 && index == items.Count - 1) == false)
                    cardObject.transform.localPosition = new Vector3(0, defaultHeight + currentSwipe.y, 0);
            }
            if (Input.GetMouseButtonUp(0) && mouseDown)
            {
                mouseDown = false;
                //save ended touch 2d point
                secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                //create vector from the two points
                currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

                if (days.Count != 0)
                {
                    if ((currentSwipe.y < -0.25 && index == 0) == false && (currentSwipe.y > 0.25 && index == days.Count - 1) == false)
                        startPos = defaultHeight + currentSwipe.y;
                    else startPos = defaultHeight;
                }
                else
                {
                    if ((currentSwipe.y < -0.25 && index == 0) == false && (currentSwipe.y > 0.25 && index == items.Count - 1) == false)
                        startPos = defaultHeight + currentSwipe.y;
                    else startPos = defaultHeight;
                }
                interpolating = true;
                t = 0;

                //normalize the 2d vector
                currentSwipe.Normalize();

                //swipe upwards
                if (days.Count != 0)
                {
                    if (currentSwipe.y > 0.25 && index != days.Count - 1)
                    {
                        endPos = topY;
                        index++;
                    }
                    //swipe down
                    else if (currentSwipe.y < -0.25 && index != 0)
                    {
                        endPos = bottomY;
                        index--;
                    }
                    else
                    {
                        endPos = 0;
                    }
                }
                else
                {
                    if (currentSwipe.y > 0.25 && index != items.Count - 1)
                    {
                        endPos = topY;
                        index++;
                    }
                    //swipe down
                    else if (currentSwipe.y < -0.25 && index != 0)
                    {
                        endPos = bottomY;
                        index--;
                    }
                    else
                    {
                        endPos = 0;
                    }
                }    
            }
        }
        else
        {
            t += Time.deltaTime * 4f;

            if(t > 1)
            {
                t = 1;
                interpolating = false;

                cardObject.transform.localPosition = new Vector3(0, 0, 0);

                if (days.Count != 0)
                    UpdateDay();
                else UpdateCard();
            }
            else
                cardObject.transform.localPosition = new Vector3(0, Interpolate(startPos, endPos, t), 0);
        }
    }

    public static float Interpolate(float start, float end, float t)
    {
        t = Mathf.Clamp01(t);
        return start - (start - end) * t;
    }
}
