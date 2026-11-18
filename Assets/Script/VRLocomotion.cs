using UnityEngine;
using UnityEngine.XR;

public class VRLocomotion : MonoBehaviour
{
    public float velocidad = 4f;
    public float velocidadCorrer = 7f;
    public float gravedad = -9.81f;
    public float alturaSalto = 1.2f;
    public float giroGrados = 30f;
    public float tiempoEntreGiros = 0.35f;
    public float alturaPersonaje = 1.7f;
    public float radioPersonaje = 0.3f;

    CharacterController controller;
    Transform cabeza;
    Vector3 puntoInicial;
    float velocidadVertical;
    float ultimoGiro;
    bool botonSaltoAnterior;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
            controller.height = alturaPersonaje;
            controller.radius = radioPersonaje;
            controller.center = new Vector3(0f, alturaPersonaje * 0.5f, 0f);
            controller.skinWidth = 0.05f;
            controller.stepOffset = 0.3f;
            controller.slopeLimit = 45f;
        }

        puntoInicial = transform.position;
        cabeza = BuscarCabeza();
    }

    void Update()
    {
        if (controller == null) return;

        if (cabeza == null) cabeza = BuscarCabeza();
        if (cabeza == null) return;

        Vector2 ejeIzquierdo = LeerEje(XRNode.LeftHand);
        Vector2 ejeDerecho = LeerEje(XRNode.RightHand);

        Vector3 adelante = Vector3.ProjectOnPlane(cabeza.forward, Vector3.up);
        if (adelante.sqrMagnitude < 0.001f) adelante = Vector3.ProjectOnPlane(cabeza.up, Vector3.up);
        adelante.Normalize();
        Vector3 derecha = Vector3.Cross(Vector3.up, adelante);

        Vector3 movimiento = adelante * ejeIzquierdo.y + derecha * ejeIzquierdo.x;
        if (movimiento.sqrMagnitude > 1f) movimiento.Normalize();

        float velocidadActual = Boton(XRNode.LeftHand, CommonUsages.gripButton) ? velocidadCorrer : velocidad;
        controller.Move(movimiento * velocidadActual * Time.deltaTime);

        if (controller.isGrounded && velocidadVertical < 0f) velocidadVertical = -2f;
        velocidadVertical += gravedad * Time.deltaTime;
        controller.Move(Vector3.up * velocidadVertical * Time.deltaTime);

        bool botonSalto = Boton(XRNode.RightHand, CommonUsages.primaryButton);
        if (botonSalto && !botonSaltoAnterior && controller.isGrounded)
        {
            velocidadVertical = Mathf.Sqrt(2f * -gravedad * alturaSalto);
        }
        botonSaltoAnterior = botonSalto;

        if (Mathf.Abs(ejeDerecho.x) > 0.7f && Time.time - ultimoGiro > tiempoEntreGiros)
        {
            transform.RotateAround(cabeza.position, Vector3.up, Mathf.Sign(ejeDerecho.x) * giroGrados);
            ultimoGiro = Time.time;
        }

        if (transform.position.y < puntoInicial.y - 20f)
        {
            controller.enabled = false;
            transform.position = puntoInicial;
            controller.enabled = true;
            velocidadVertical = 0f;
        }
    }

    Transform BuscarCabeza()
    {
        GameObject centro = GameObject.Find("CenterEyeAnchor");
        if (centro != null) return centro.transform;

        Camera camara = Camera.main;
        return camara != null ? camara.transform : null;
    }

    Vector2 LeerEje(XRNode mano)
    {
        InputDevice dispositivo = InputDevices.GetDeviceAtXRNode(mano);
        if (dispositivo.isValid && dispositivo.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 valor))
        {
            return valor;
        }
        return Vector2.zero;
    }

    bool Boton(XRNode mano, InputFeatureUsage<bool> uso)
    {
        InputDevice dispositivo = InputDevices.GetDeviceAtXRNode(mano);
        return dispositivo.isValid && dispositivo.TryGetFeatureValue(uso, out bool valor) && valor;
    }
}
