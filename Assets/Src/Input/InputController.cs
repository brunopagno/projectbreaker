using UnityEngine;

public class InputController
{
    private static int _index = 0;

    private string _type;

    public int Index { get; private set; }
    public string Name { get; private set; }

    public InputController()
    {
        Index = _index;
        _index += 1;

        Name = "Player " + Index;
        if (Index == 0)
        {
            _type = "K";
        }
        else
        {
            _type = Index.ToString();
        }
    }

    public float GetAxis(string axis)
    {
        return Input.GetAxis(axis + "_" + _type);
    }

    public bool GetButton(string button)
    {
        return Input.GetButton(button + "_" + _type);
    }

    public bool GetButtonDown(string button)
    {
        return Input.GetButtonDown(button + "_" + _type);
    }

}
