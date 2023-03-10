using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintLevelImages : MonoBehaviour
{
    private List<Ways> ways;
    public GameObject redArrow;
    public GameObject wayUI;

    [SerializeField] int level;

    Color c;

    private void OnEnable()
    {
        ThemeChanger.instance.RandomColor();
        c = ThemeChanger.current.drawingLineColor;

        readJson();
        createWay();
        RepositionImage();
    }

    private void OnDisable()
    {
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
    }

    private void RepositionImage()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).localPosition = new Vector3(0, 0, 0);
            transform.GetChild(i).localScale = new Vector3(60, 60, 60);

            if (transform.GetChild(i).childCount > 0)
            {
                transform.GetChild(i).GetChild(0).localScale = new Vector3(0.93f, 0.93f, 0.93f);
                transform.GetChild(i).GetChild(1).localScale = new Vector3(0.93f, 0.93f, 0.93f);
            }

            if (transform.GetChild(i).GetComponent<LineRenderer>() != null)
            {
                AnimationCurve curve = new AnimationCurve();
                curve.AddKey(0, 0.35f);
                transform.GetChild(i).GetComponent<LineRenderer>().widthCurve = curve;

                transform.GetChild(i).GetComponent<LineRenderer>().startColor = c;
                transform.GetChild(i).GetComponent<LineRenderer>().endColor = c;
            }
        }
    }

    void createWay()
    {
        for (int i = 0; i < ways.Count; i++)
        {
            GameObject go = Instantiate(wayUI) as GameObject;
            Ways way = ways[i];

            go.GetComponent<WaysUI>().setWayModel(way);
            go.GetComponent<WaysUI>().createUI();

            if (way.direction > 1)
            {
                GameObject red = Instantiate(redArrow) as GameObject;

                float angle = AngleBetweenVector2(GridManager.GetGridManger().GetPosForGrid(way.startingGridPosition),
                    GridManager.GetGridManger().GetPosForGrid(way.endGridPositon));

                red.transform.rotation = Quaternion.Euler(0, 0, angle);
                red.transform.position = go.GetComponent<WaysUI>().pointOnLine();
                red.transform.parent = transform;
            }

            go.transform.parent = transform;
        }
    }

    private void readJson()
    {
        ways = new List<Ways>();
        string levelPath = string.Format("Package_{0}/level_{1}", LevelData.worldSelected, level);

        TextAsset file = Resources.Load(levelPath) as TextAsset;
        string dataAsJson = file.ToString();

        string[] jsonObjects = dataAsJson.Trim().Split(new char[] { '=' });

        for (int i = 0; i < jsonObjects.Length; i++)
        {
            Ways way = JsonUtility.FromJson<Ways>(jsonObjects[i]);
            ways.Add(way);
        }

    }

    private float AngleBetweenVector2(Vector2 vec1, Vector2 vec2)
    {
        Vector2 diference = vec2 - vec1;
        float sign = (vec2.y < vec1.y) ? -1.0f : 1.0f;
        return Vector2.Angle(Vector2.right, diference) * sign;
    }

}
