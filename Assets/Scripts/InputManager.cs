// Unity Only File
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public KinematicPlane kp;
    public ModuleEngine engine;
    public Camera cam;
    public EquipManager equipManager;
    public AIClient aiClient;

    private AIPilotControls controls = null;
    private Vector2 trim = Vector2.zero;


    public void Start()
    {
        controls = new AIPilotControls();
        controls.PYR.Enable();

        controls.PYR.Pause.performed += (ctx) => kp.paused = !kp.paused;
        controls.PYR.WeaponFire.performed += (ctx) => equipManager.tryFire = true;
        controls.PYR.CMS.performed += (ctx) => aiClient.FireChaffFlare();
    }

    // Update is called once per frame
    public void Update()
    {
        var input = Vector3.zero;

        var pitch = controls.PYR.Pitch.ReadValue<float>();
        var roll = controls.PYR.Roll.ReadValue<float>();
        var throttle = controls.PYR.Throttle.ReadValue<float>();

        float deadzone = 0.15f;
        if (Mathf.Abs(pitch) <= deadzone) pitch = 0;
        if (Mathf.Abs(roll) <= deadzone) roll = 0;

        var brk = controls.PYR.Break.ReadValue<float>();
        kp.brake = brk;

        var slew = controls.PYR.Slew.ReadValue<float>();
        if (slew > 0.5f) transform.position += transform.forward * 100;

        var trimChange = controls.PYR.Trim.ReadValue<Vector2>();
        trim += trimChange * Time.fixedDeltaTime * 0.1f;

        engine.throttle += throttle * Time.fixedDeltaTime;
        input = new Vector3(pitch + trim.y, 0, -roll + trim.x);

        if (Input.GetKey(KeyCode.W)) input.x = 1;
        if (Input.GetKey(KeyCode.S)) input.x = -1;
        if (Input.GetKey(KeyCode.A)) input.z = 1;
        if (Input.GetKey(KeyCode.D)) input.z = -1;
        if (Input.GetKey(KeyCode.Space)) transform.position += transform.forward * 1000;
        if (Input.GetKey(KeyCode.LeftShift)) engine.throttle += 0.01f;
        if (Input.GetKey(KeyCode.LeftControl)) engine.throttle -= 0.01f;
        if (Input.GetKeyDown(KeyCode.P)) kp.paused = !kp.paused;

        var lookLimit = 45;
        var look = controls.PYR.Look.ReadValue<Vector2>();

        if (Mathf.Abs(look.x) <= deadzone) look.x = 0;
        if (Mathf.Abs(look.y) <= deadzone) look.y = 0;

        // cam.transform.localEulerAngles = new Vector3(-3.5f + -look.y * lookLimit, look.x * lookLimit, 0);

        kp.input = input;
    }
}
