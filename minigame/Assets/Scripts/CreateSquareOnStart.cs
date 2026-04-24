using UnityEngine;

public static class CreateSquareOnStart
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateSquare()
    {
        const string squareName = "SimpleSquare";

        if (GameObject.Find(squareName) != null)
        {
            return;
        }

        var square = GameObject.CreatePrimitive(PrimitiveType.Quad);
        square.name = squareName;
        square.transform.position = new Vector3(0f, 0f, 0f);
        square.transform.localScale = new Vector3(2f, 2f, 1f);

        var renderer = square.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.green;
        }
    }
}
