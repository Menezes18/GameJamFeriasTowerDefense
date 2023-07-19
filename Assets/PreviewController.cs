using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;

public class PreviewController : MonoBehaviour
{
    public float raycastDistance = 5f;
    public Transform raycastOrigin;
    public GameObject blockPrefab; // Prefab do bloco que será instanciado
    public GameObject previewBlock;
    private bool canPlaceBlock = true; // Flag para evitar múltiplas instâncias ao segurar o botão
    
    

    void Start()
    {
        
        if (raycastOrigin == null)
        {
            
            Debug.LogError("A referência do raycastOrigin não foi atribuída. Arraste o bloco de origem do Raycast para esta variável no Inspector.");
        }
    }

    void Update()
    {
        if (raycastOrigin.parent != transform)
        {
            
            raycastOrigin.parent = transform;
            raycastOrigin.localPosition = Vector3.forward * raycastDistance;
            raycastOrigin.localRotation = Quaternion.identity;
        }

        if (canPlaceBlock)
        {
            Ray ray = new Ray(raycastOrigin.position, raycastOrigin.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, raycastDistance))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.green);

                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    // Instanciar o bloco na posição atingida pelo Raycast
                    Instantiate(blockPrefab, hit.point, Quaternion.identity);
                    canPlaceBlock = false;
                    StartCoroutine(ResetBlockPlacement());
                }
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.red);
            }
        }
    }

    IEnumerator ResetBlockPlacement()
    {
        // Pequeno atraso para evitar múltiplas instâncias em um único clique prolongado
        yield return new WaitForSeconds(0.2f);
        canPlaceBlock = true;
    }

    void OnDrawGizmos()
    {
        if (raycastOrigin == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(raycastOrigin.position + raycastOrigin.forward * raycastDistance, 0.1f);
    }
}
