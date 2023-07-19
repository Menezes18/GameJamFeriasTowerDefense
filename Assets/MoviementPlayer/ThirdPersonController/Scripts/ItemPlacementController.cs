using UnityEngine;
using UnityEngine.InputSystem;

public class ItemPlacementController : MonoBehaviour
{
 public Transform blockPrefab;   // O prefab do bloco que será instanciado
    public LayerMask raycastLayer;  // A camada dos blocos onde o Raycast será lançado

    private Transform previewBlock; // Referência para a pré-visualização do bloco

    // Método chamado quando o jogador pressiona o botão esquerdo do mouse ou toca na tela (para dispositivos móveis)
    public void OnPlaceBlock(InputAction.CallbackContext context)
    {
        if (context.performed && previewBlock != null)
        {
            // Instancia o bloco na posição da pré-visualização e limpa a pré-visualização
            Instantiate(blockPrefab, previewBlock.position, previewBlock.rotation);
            Destroy(previewBlock.gameObject);
        }//
    }

    private void Update()
    {
        // Lançar um Raycast a partir da posição do mouse ou do toque na tela (para dispositivos móveis)
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastLayer))
        {
            // Se o Raycast atingir um objeto com a camada desejada
            if (hit.collider.gameObject.layer == raycastLayer)
            {
                // Posiciona a pré-visualização do bloco no ponto de colisão
                if (previewBlock == null)
                {
                    previewBlock = Instantiate(blockPrefab, hit.point, Quaternion.identity);
                }
                else
                {
                    previewBlock.position = hit.point;
                }

                // Alinhar a pré-visualização do bloco com a normal do ponto de colisão
                Vector3 normal = hit.normal;
                previewBlock.up = normal;
            }
            else
            {
                // Se o Raycast não atingir um objeto com a camada desejada, destruir a pré-visualização do bloco
                Destroy(previewBlock?.gameObject);
            }
        }
        else
        {
            // Se o Raycast não atingir nenhum objeto, destruir a pré-visualização do bloco
            Destroy(previewBlock?.gameObject);
        }
    }
}