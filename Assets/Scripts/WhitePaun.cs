using UnityEngine;

public class WhitePawn : MonoBehaviour
{
    private bool isPawnSelected = false;
    private GameObject[] validMoveSquares;

    // Define the direction in which the pawn is allowed to move (forward along the positive Z-axis)
    private Vector3 moveDirection = Vector3.forward;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    isPawnSelected = true;
                    ShowValidMoveSquares();
                    print("Pawn is selected");
                }
                else if (isPawnSelected && hit.collider.CompareTag("ValidMoveSquare"))
                {
                    if (CanMoveToSquare(hit.collider.gameObject))
                    {
                        MovePawn(hit.collider.gameObject);
                    }
                    else
                    {
                        Debug.Log("Invalid move");
                    }

                    isPawnSelected = false;
                    HideValidMoveSquares();
                }
                else
                {
                    isPawnSelected = false;
                    HideValidMoveSquares();
                }
            }
        }
    }

    bool CanMoveToSquare(GameObject targetSquare)
    {
        Vector3 toTarget = targetSquare.transform.position - transform.position;
        float dotProduct = Vector3.Dot(toTarget, moveDirection);
        return dotProduct >= 0f; // Check if the movement direction is forward or the same direction
    }

    void ShowValidMoveSquares()
    {
        // Find the squares in the allowed move direction in front of the selected pawn
        Collider[] hitColliders = Physics.OverlapBox(transform.position + moveDirection, Vector3.one * 0.9f, Quaternion.identity, LayerMask.GetMask("ChessSquare"));

        validMoveSquares = new GameObject[hitColliders.Length];

        for (int i = 0; i < hitColliders.Length; i++)
        {
            GameObject hitObject = hitColliders[i].gameObject;
            Renderer renderer = hitObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.green;
                validMoveSquares[i] = hitObject;
            }
        }
    }

    void HideValidMoveSquares()
    {
        if (validMoveSquares != null)
        {
            foreach (GameObject square in validMoveSquares)
            {
                if (square != null)
                {
                    Renderer renderer = square.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material.color = Color.white;
                    }
                }
            }
        }
    }

    void MovePawn(GameObject targetSquare)
    {
        // Check if there's an obstruction in the path of the moving piece
        Vector3 direction = targetSquare.transform.position - transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction.normalized, out hit, direction.magnitude))
        {
            // If the ray hits anything other than the chessboard, disallow the move
            if (!hit.collider.CompareTag("Chessboard"))
            {
                Debug.Log("Cannot move: obstruction in the way");
                return;
            }
        }

        // Move the pawn to the target square
        transform.position = targetSquare.transform.position;
    }
}
