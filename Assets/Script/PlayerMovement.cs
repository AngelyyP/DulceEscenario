using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float velocidad = 5f;
    public float gravedad = -9.8f;
    public float salto = 1.5f;

    public Transform sueloCheck;
    public float distanciaSuelo = 0.4f;
    public LayerMask sueloMask;

    Vector3 velocidadVertical;
    bool enSuelo;

    CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        enSuelo = Physics.CheckSphere(sueloCheck.position, distanciaSuelo, sueloMask);

        if (enSuelo && velocidadVertical.y < 0)
        {
            velocidadVertical.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 movimiento = transform.right * x + transform.forward * z;

        controller.Move(movimiento * velocidad * Time.deltaTime);

        velocidadVertical.y += gravedad * Time.deltaTime;

        controller.Move(velocidadVertical * Time.deltaTime);
    }
}