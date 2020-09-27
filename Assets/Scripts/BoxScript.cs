using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxScript : MonoBehaviour
{
    public List<Transform> edges;
    public LayerMask collideMask;
    private Collider2D mCollider;
    private int contacts;
    void Start()
    {
        mCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        contacts = 0;
        foreach (Transform edge in edges)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(edge.position, 0.1f, collideMask);
            foreach (Collider2D collider in colliders)
            {
                if (collider != mCollider)
                {
                    contacts += 1;
                    break;
                }
            }
        }
        if (contacts > 2)
        {
            Destroy(gameObject);
        }
    }
}
